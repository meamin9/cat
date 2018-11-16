using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Automata.Game
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

        public static T Find<T>(this IList<T> list, T target) where T: class
        {
            int n = list.Count;
            for (var i = 0; i < n; ++i)
            {
                if (list[i] == target)
                {
                    return list[i];
                }
            }
            return null;
        }

        public static System.Func<T> Creator<T>() where T : new()
        {
            return () => { return new T(); };
        }


    }
}

