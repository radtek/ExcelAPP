using System;
using System.Data; 
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Genersoft.GSIDP.JZPubQry_Svr
{
     public class StringToDataSet
    {
         public static DataSet getDataSet(string xml)
         {              
             System.IO.StringReader sr = new System.IO.StringReader(xml);
             DataSet ds = new DataSet();           
             ds.ReadXml(sr);
             return ds;
         }
         public static DataSet getDataSetWithSchema(string[] xmlarr)
         { 
             DataSet ds = new DataSet();
             System.IO.StringReader sr = new System.IO.StringReader(xmlarr[0]);            
             ds.ReadXmlSchema(sr);
             sr = new System.IO.StringReader(xmlarr[1]);
             ds.ReadXml(sr);
             return ds;
         }
         public static DataSet getDataSetFromByte(byte[] btarr){
             string   xml=System.Text.UTF8Encoding.UTF8.GetString(btarr,0,btarr.Length);
             return getDataSet(xml);
         }
         public static DataSet getDataSetFromByteWithSchema(byte[][] btarr)
         {
             string[] xmlarr = new string[2];
             xmlarr[0] = System.Text.UTF8Encoding.UTF8.GetString(btarr[0], 0, btarr[0].Length);
             xmlarr[1] = System.Text.UTF8Encoding.UTF8.GetString(btarr[1], 0, btarr[1].Length);
             return getDataSetWithSchema(xmlarr);
         }
         public static DataSet getDataSetFromZipByte(byte[] btarr){
             byte[] bta = zipByte(btarr);
             return getDataSetFromByte(bta);
         }
         public static DataSet getDataSetFromZipByteWithSchema(byte[][] btarr)
         {
             btarr[0] = zipByte(btarr[0]);
             btarr[1] = zipByte(btarr[1]);
             return getDataSetFromByteWithSchema(btarr);
         }
         public static DataSet getDataSetFromZipString(string strdata)
         {
             byte[] dataarr = System.Text.Encoding.UTF8.GetBytes(strdata);
             return getDataSetFromZipByte(dataarr);

         }
         public static DataSet getDataSetFromZipStringWithSchema(string[] strarr)
         {
             byte[][] bytearr = new byte[2][];
             bytearr[0] = Convert.FromBase64String(strarr[0]);
             bytearr[1] = Convert.FromBase64String(strarr[1]);
             return getDataSetFromZipByteWithSchema(bytearr);
         }
        public static byte[] zipByte(byte[] btarr)
        {

            MemoryStream ms = new MemoryStream();
            GZipStream gZipStream = new GZipStream(ms, CompressionMode.Compress);
            gZipStream.Write(btarr, 0, btarr.Length);
            gZipStream.Close();
            ms.Flush();
            byte[] data = ms.ToArray();
            ms.Close();
            return data;
        }
    }
}
