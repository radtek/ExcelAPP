using System;
using System.Security.Cryptography;
using System.Text;
using System.Data;

namespace ExcelClient
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

        public static string getHashStringFromDataTableField(DataTable dt)
        {
            string vsValue = "";
            string vsFields = "";
            foreach (DataColumn col in dt.Columns)
            {
                vsFields += col.ColumnName+",";
            }
            vsFields = vsFields.Trim(',');
            vsValue = getHashString(vsFields);
            return vsValue;
        }

        public static string getFieldNamesFromDataTable(DataTable dt, string psField)
        {
            string vsValue = "";
            foreach (DataRow row in dt.Rows)
            {
                if (row[psField] != DBNull.Value && !string.IsNullOrEmpty(row[psField].ToString().Trim()))
                {
                    vsValue += "," + row[psField].ToString();
                }
            }
            return vsValue;
        }


    }
}
