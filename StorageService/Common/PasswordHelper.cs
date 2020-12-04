using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Common
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            SHA256 sha256 = new SHA256Managed();
            var result = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            var sb = new StringBuilder();

            foreach (var t in result)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
