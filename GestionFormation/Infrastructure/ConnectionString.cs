using System;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GestionFormation.Infrastructure
{
    public static class ConnectionString
    {
        private static readonly byte[] Key = Encoding.ASCII.GetBytes("F2D26648F1AB4A459B4F8D609EBA4020");
        private static readonly byte[] Iv = Encoding.ASCII.GetBytes("1BD3141C52E84DB0");
        public static IConnectionStringProvider ConnectionStringProvider { get; set; } = new AppConfigConnectionStringProvider();

        private static string _connectionString;
        public static string Get()
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
                return _connectionString;

            var connectionString = ConnectionStringProvider.Read();

            if(string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("impossible d'obtenir la chaine de connexion");

            var builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;

            if (!builder.ContainsKey("password"))
            {
                _connectionString = connectionString;
                return _connectionString;
            }

            var password = builder["password"] as string;
            if (password.Contains("secret:"))
            {
                var decryptedPassword = DecryptStringFromBytes_Aes(Convert.FromBase64String(password.Remove(0, 7)),Key, Iv);
                builder["password"] = decryptedPassword;
                _connectionString = builder.ConnectionString;
                return _connectionString;
            }

            _connectionString = connectionString;

            builder["password"] = "secret:" + Convert.ToBase64String(EncryptStringToBytes_Aes(password, Key, Iv));
            ConnectionStringProvider.Write(builder.ConnectionString);

            return _connectionString;
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] iv)
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

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }
        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }

    public interface IConnectionStringProvider
    {
        void Write(string connectionString);
        string Read();
    }

    public class AppConfigConnectionStringProvider : IConnectionStringProvider
    {
        public void Write(string connectionString)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings["local"].ConnectionString = connectionString;
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        public string Read()
        {
            return ConfigurationManager.ConnectionStrings["local"].ConnectionString;
        }
    }
}


