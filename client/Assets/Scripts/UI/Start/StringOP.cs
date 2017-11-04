using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

class StringOP {
    // ComputeMD5 计算字符串的MD5
    public static string ComputeMD5(string source) {
        using (MD5 md5Hash = MD5.Create()) {
            return GetMd5Hash(md5Hash, source);
        }
    }

    static string GetMd5Hash(MD5 md5Hash, string input) {

        // Convert the input string to a byte array and compute the hash.
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++) {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    public static bool LengthRange(string str, int minLen, int maxLen) {
        var len = str.Length;
        return minLen <= len && len <= maxLen;
    }
}

