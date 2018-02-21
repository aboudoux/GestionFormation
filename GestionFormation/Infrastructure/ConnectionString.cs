using System;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GestionFormation.Infrastructure
{
    public class ConnectionString
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly byte[] Key = Encoding.ASCII.GetBytes("F2D26648F1AB4A459B4F8D609EBA4020");
        private readonly byte[] Iv = Encoding.ASCII.GetBytes("1BD3141C52E84DB0");
        
        public ConnectionString(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        }

        private static string _loadedConnectionString;
        public static string Get()
        {
            if (string.IsNullOrWhiteSpace(_loadedConnectionString))
            {
                var cs = new ConnectionString(new AppConfigConnectionStringProvider());
                _loadedConnectionString = cs.GetConnectionString();
            }

            return _loadedConnectionString;
        }

        public string GetConnectionString()
        {
            var connectionString = _connectionStringProvider.Read();

            if(string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("impossible d'obtenir la chaine de connexion");

            var builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;

            if (!builder.ContainsKey("password"))
                return connectionString;

            var password = builder["password"] as string;
            if (password.Contains("secret:"))
            {
                var decryptedPassword = DecryptStringFromBytes_Aes(Convert.FromBase64String(password.Remove(0, 7)),Key, Iv);
                builder["password"] = decryptedPassword;
                return builder.ConnectionString;
            }

            builder["password"] = "secret:" + Convert.ToBase64String(EncryptStringToBytes_Aes(password, Key, Iv));
            _connectionStringProvider.Write(builder.ConnectionString);

            return connectionString;
        }

        private byte[] EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }
        private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}


