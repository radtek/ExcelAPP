using System;
using System.Security.Cryptography;
using System.Text;

namespace ExcelAPPWeb
{
    public class ComputeHash
    {
        public ComputeHash()
        {
        }
        private static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length - 1; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

        public static string getHashString(string sSourceData)
        {            
            byte[] tmpSource;
            byte[] tmpHash; 
            //Create a byte array from source data
            tmpSource = ASCIIEncoding.ASCII.GetBytes(sSourceData);
            //Compute hash based on source data
            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);             
            //return ByteArrayToString(tmpHash);
            return Convert.ToBase64String(tmpHash);
        }

    }
}
