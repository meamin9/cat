using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NSUnityUtil
{
    public static class Funcs
    {
        public static string MD5Hex(string source) {
            using (var md5Hash = MD5.Create()) {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                var sBuilder = new StringBuilder();
                for (var i = 0; i < data.Length; i++) {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
    }
}

