using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GestionFormation.Infrastructure
{
    public static class PasswordExtentions
    {
        public static string GetHash(this string password)
        {
            var sha = new SHA1CryptoServiceProvider();
            var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(password));

            var result = new StringBuilder();
            foreach (var b in hash)
                result.Append(b.ToString("X2"));
            return result.ToString();
        }
    }
}
