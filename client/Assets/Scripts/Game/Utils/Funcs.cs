using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace AM.Game
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

        public static int _id=0;
        public static int NewId()
        {
            return ++_id;
        }


    }

    public class TimeDebug
    {
        public float Begin { get; set; }
        public TimeDebug()
        {
            Begin = Time.deltaTime;
        }
        public void Print(string msg)
        {
            Log.Info($"{msg} use time: {Time.deltaTime - Begin}");
        }
    }
}

