using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace ExcelClient
{
    public class LocalDataOper
    {
        public static void saveToFile(DataSet ds, string path, string fileName)
        {
            string vsPath = LocalSavePath();
            if (!string.IsNullOrEmpty(path))
            {
                vsPath += "\\" + path;
                if (!System.IO.Directory.Exists(vsPath))
                {
                    System.IO.Directory.CreateDirectory(vsPath);
                }
            } 
            string[] strfile = new string[2];
            strfile[0] = vsPath + "\\Schema_" + fileName;
            strfile[1] = vsPath + "\\Data_" + fileName;
            ds.WriteXmlSchema(strfile[0]);
            ds.WriteXml(strfile[1]);

        }
        public static void saveToZipFile(DataSet ds,string path, string fileName)
        {
            string vsPath = LocalSavePath();
            if (!string.IsNullOrEmpty(path))
            {
                vsPath+= "\\" + path;
                if (!System.IO.Directory.Exists(vsPath ))
                {
                    System.IO.Directory.CreateDirectory(vsPath);
                }
            }
            string[] strfile = new string[2];
            strfile[0] = vsPath + "\\ZipSchema_" + fileName;
            strfile[1] = vsPath + "\\ZipData_" + fileName;
            byte[][] btarr = dataSetToZipBtye(ds);
            byteToFile(btarr[0], strfile[0]);
            byteToFile(btarr[1], strfile[1]);           
            
        }
        public static DataSet LoadFromFile(string path, string fileName)
        {
            string vsPath = LocalSavePath();
            if (!string.IsNullOrEmpty(path))
            {
                vsPath += "\\" + path;
                if (!System.IO.Directory.Exists(vsPath))
                {
                    System.IO.Directory.CreateDirectory(vsPath);
                }
            }
            string[] strfile = new string[2];
            strfile[0] = vsPath + "\\Schema_" + fileName;
            strfile[1] = vsPath + "\\Data_" + fileName;
            DataSet ds = new DataSet();
            ds.ReadXmlSchema(new StreamReader(strfile[0]));
            ds.ReadXml(new StreamReader(strfile[1]));
            return ds;
        }

        public static DataSet LoadFromZipFile(string path,string fileName)
        {
            string vsPath = LocalSavePath();
            if (!string.IsNullOrEmpty(path))
            {
                vsPath += "\\" + path;
                if (!System.IO.Directory.Exists(vsPath))
                {
                    System.IO.Directory.CreateDirectory(vsPath);
                }
            }
            byte[] btSchema = fileToByte(vsPath + "\\ZipSchema_" + fileName);
            byte[] btData = fileToByte(vsPath + "\\ZipData_" + fileName);
            DataSet ds = new DataSet();
         
            ds.ReadXmlSchema( unZipToStream(btSchema));
            ds.ReadXml(unZipToStream(btData));
            return ds;
        }


        public static string LocalSavePath()
        {
            string appPath = Application.StartupPath+"\\localTmp";
            if (!System.IO.Directory.Exists(appPath))
            {
                System.IO.Directory.CreateDirectory(appPath);
            }
            return appPath;
        }
        
        public static byte[][] dataSetToZipBtye(DataSet ds)
        {
            string[] arr = new string[2];
            arr[0] = ds.GetXmlSchema();
            arr[1] = ds.GetXml();
            byte[][] bytearr = new byte[2][];
            bytearr[0] = System.Text.Encoding.UTF8.GetBytes(arr[0]);
            bytearr[1] = System.Text.Encoding.UTF8.GetBytes(arr[1]);
            byte[][] zipbyte = new byte[2][];
            zipbyte[0] = StringToDataSet.zipByte(bytearr[0]);
            bytearr[0] = StringToDataSet.unZipByte(zipbyte[0]);
            zipbyte[1] = StringToDataSet.zipByte(bytearr[1]);
            return zipbyte;
        }
        public static void byteToFile(byte[] btarr,string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            try
            {
                string vsstr = Convert.ToBase64String(btarr);
                StreamWriter writer = new StreamWriter(fs, Encoding.Default);
                fs.Seek(0, SeekOrigin.Begin);
                writer.Write(vsstr);
                writer.Flush();
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }
        }
        public static byte[] fileToByte(string filePath)
        {
            string str = "";
            StreamReader sr = new StreamReader(filePath);
            try
            { 
                 str = sr.ReadToEnd();
            }
            finally
            {
                sr.Close();
                sr.Dispose();
            }
            byte[] btarr =Convert.FromBase64String(str);
            return btarr;
        }

        public static TextReader unZipToStream(byte[] btarr)
        {
           
            byte[] vsarr = StringToDataSet.unZipByte(btarr);
           string vsstr= System.Text.Encoding.UTF8.GetString(vsarr, 0, vsarr.Length);

           TextReader tr = new StringReader(vsstr);


           return tr;            
        }
    }
}
