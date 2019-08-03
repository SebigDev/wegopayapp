using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace wegopay.common
{
    public class Security
    {
        public static string GenerateSecure(string password = "", params string[] salt)
        {
            // hash the salt, hash the password text
            // hash (salthash + pwdhash)

            // buildup salt with multiple values
            string saltVals = "";
            foreach (var i in salt)
            {
                saltVals += i + ";";
            }

            string pwd = password;
            string saltHash = !string.IsNullOrEmpty(saltVals) ? SHA512Hash(saltVals) : "";
            string pwdHash = SHA512Hash(pwd);
            string final = SHA512Hash(saltHash + pwdHash);

            return final;
        }

        public static bool ValidatePassword(string hash, string password, params string[] salt)
        {
            // buildup salt with multiple values
            string saltVals = "";
            foreach (var i in salt)
            {
                saltVals += i + ";";
            }

            string pwd = password;
            string saltHash = SHA512Hash(saltVals);
            string pwdHash = SHA512Hash(pwd);
            string final = SHA512Hash(saltHash + pwdHash);

            return (final == hash);
        }

        public static string SHA512Hash(string plainText)
        {
            string plainStr = plainText;
            byte[] tmpSource = { 0 };
            byte[] tmpHash;

            tmpSource = ASCIIEncoding.ASCII.GetBytes(plainStr);
            tmpHash = new SHA512CryptoServiceProvider().ComputeHash(new MemoryStream(tmpSource));
            return Convert.ToBase64String(tmpHash);
        }
    }
}
