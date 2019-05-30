using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text; 
using System.IO.Compression;

namespace ExcelClient
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
        public static DataSet getDataSetFromByte(byte[] btarr)
        {
            string xml = System.Text.Encoding.UTF8.GetString(btarr, 0, btarr.Length);
            return getDataSet(xml);
        }
        public static DataSet getDataSetFromByteWithSchema(byte[][] btarr)
        {
            string[] xmlarr = new string[2];
            xmlarr[0] = System.Text.Encoding.UTF8.GetString(btarr[0], 0, btarr[0].Length);
            xmlarr[1] = System.Text.Encoding.UTF8.GetString(btarr[1], 0, btarr[1].Length);
            return getDataSetWithSchema(xmlarr);
        }
        public static DataSet getDataSetFromZipByte(byte[] btarr)
        {
            byte[] bta = unZipByte(btarr);
            return getDataSetFromByte(bta);
        }
        public static DataSet getDataSetFromZipByteWithSchema(byte[][] btarr)
        {
            btarr[0] = unZipByte(btarr[0]);
            btarr[1] = unZipByte(btarr[1]);
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
            bytearr[0] = System.Text.Encoding.UTF8.GetBytes(strarr[0]);
            bytearr[1] = System.Text.Encoding.UTF8.GetBytes(strarr[1]);
            return getDataSetFromZipByteWithSchema(bytearr);
        }
        //getDataFormatByte getZipDataFormatByte
        public static DataSet getDataSetFromDataFormat(byte[] byarr)
        {
            // 反序列化的过程
            MemoryStream ms = new MemoryStream(byarr);
            IFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(ms);
            DataSet ds = (DataSet)obj;
            ms.Close();
            ms.Dispose();
            return ds;
        }
        public static DataSet getDataSetFromZipDataFormat(byte[] byarr)
        {
            byte[] rearr = unZipByte(byarr);
            return getDataSetFromDataFormat(rearr);
        }
        //public static byte[] unZipByte(byte[] btarr)
        //{

        //    System.IO.MemoryStream inms = new System.IO.MemoryStream(btarr);
        //    System.IO.MemoryStream outms = new System.IO.MemoryStream();
        //    //do the compressions/uncompressions 
        //    ICSharpCode.SharpZipLib.BZip2.BZip2.Decompress(inms, outms);
        //    byte[] result = outms.ToArray();
        //    return result;
        //}
        //public static byte[] unZipByte(byte[] btarr)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    //调用免费开源控件方法
        //    Stream s2 = new ZOutputStream(ms);
        //    s2.Write(btarr, 0, btarr.Length);
        //    s2.Close();
        //    byte[] writeData = (byte[])ms.ToArray();
        //    ms.Flush();
        //    ms.Close();
        //    return writeData;
        //}
        public static byte[] zipByte(byte[] btarr)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream gZipStream = new GZipStream(ms, CompressionMode.Compress);
            gZipStream.Write(btarr, 0, btarr.Length);
            gZipStream.Close();
            gZipStream.Dispose();

            ms.Flush();
            byte[] data = ms.ToArray();
            ms.Close();
            ms.Dispose();
            return data;
        }
        public static byte[] unZipByte(byte[] btarr)
        {
            System.IO.MemoryStream inms = new System.IO.MemoryStream(btarr);
            System.IO.MemoryStream outms = new System.IO.MemoryStream();
            Decompress(inms, outms);
            byte[] result = outms.ToArray();
            return result;
        }
        private static void Decompress(Stream source, Stream destination)
        {
            using (GZipStream input = new GZipStream(source, CompressionMode.Decompress))
            {
                Pump(input, destination);
            }
        }



        private static void Pump(Stream input, Stream output)
        {
            byte[] bytes = new byte[4096];
            int n;
            while ((n = input.Read(bytes, 0, bytes.Length)) != 0)
            {
                output.Write(bytes, 0, n);
            }
        }
        

        public static string replaceUrl(string url)
        {
            int indnum = url.IndexOf("://");
            url = url.Substring(indnum + 4);
            indnum = url.IndexOf('/');
            url = url.Substring(indnum + 1);
            return url;
        }

        /*AsyncCallback asyncCallback = new AsyncCallback(CallBackExec);
        IAsyncResult asyncResult = serviceA.BeginHelloWorld(asyncCallback, null);
        asyncResult.AsyncWaitHandle.WaitOne();        
        */
       


    }
}


/* using System;
 using System.IO;
 using System.Data;
 using System.Runtime.Serialization;
 using System.Runtime.Serialization.Formatters.Binary;
 
 namespace Common
 {
     public class DataFormatter
    {
        private DataFormatter() { }
         //// <summary>
        /// Serialize the Data of dataSet to binary format
        /// </summary>
        /// <param name="dsOriginal"></param>
       /// <returns></returns>
        static public byte[] GetBinaryFormatData(DataSet dsOriginal)
       {
            byte[] binaryDataResult = null;
           MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();
            dsOriginal.RemotingFormat = SerializationFormat.Binary;
           brFormatter.Serialize(memStream, dsOriginal);
           binaryDataResult = memStream.ToArray();
            memStream.Close();
           memStream.Dispose();
            return binaryDataResult;
       }
         //// <summary>
        /// Retrieve dataSet from data of binary format
        /// </summary>
        /// <param name="binaryData"></param>
        /// <returns></returns>
        static public DataSet RetrieveDataSet(byte[] binaryData)
        {
            DataSet dataSetResult = null;
            MemoryStream memStream = new MemoryStream(binaryData);
            IFormatter brFormatter = new BinaryFormatter();
            object obj = brFormatter.Deserialize(memStream);
            dataSetResult = (DataSet)obj;
            return dataSetResult;
        }
    }
} */

/*第二种方法是借助返回的IAsyncHandle的WaitHandle属性的WaitOne()方法实现线程同步后，执行代码，然后将要执行的代码执行后随时可调用EndInvoke方法。
 
示例代码基本没有什么改动，只是在BeginInvoke方法后执行一个ar.WaitHandle.WaitOne()方法，该方法将阻塞主线程使主线程等待返回ar的begininvoke调用的方法的线程执行完毕（不要怪我说话有点绕，我是说的详细，否则你弄不明白，仔细看这句话就明白了^o^）。
 
本着一切代码都是纸老虎的原则，请仔细分析下面的代码  其实和示例1基本无异 ，只是使用WaitOne()方法同步了一下线程而已
 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace sample
{
    class AsyncDemo
    {
        public string TestMethod(int callDuration, out int threadid)
        {
            Console.WriteLine("Test Method begins");
            Thread.Sleep(callDuration);
            threadid = AppDomain.GetCurrentThreadId();
            return "MyCallTime was" + callDuration.ToString();
        }
    }
    public delegate string AsyncDelegate(int callDuration, out int threadid);
    class Program
    {
        static void Main(string[] args)
        {
            int threadID;
            AsyncDemo ad = new AsyncDemo();
            AsyncDelegate andl = new AsyncDelegate(ad.TestMethod);
            IAsyncResult ar = andl.BeginInvoke(3000, out threadID, null, null);
            Thread.Sleep(10);
            Console.WriteLine("Main Thread {0} Does Some Work",
                AppDomain.GetCurrentThreadId());
            ar.AsyncWaitHandle.WaitOne();//执行该方法时主线程将等待辅助线程执行完毕，使两线程同步后再执行以下语句
            Console.WriteLine("其实很简单");//执行一些方法
            string ret = andl.EndInvoke(out threadID, ar);//使用EndInvoke来获取返回结果和传入ref和out的变量获取修改后的实例的位置（这我就不太好用语言来表述了，你自己心领神会吧）
            Console.WriteLine("The call executed on thread {0},with return value : {1}",
                threadID, ret);
            Console.ReadLine();
        }
    }
}
0 0 0 
(请您对文章做出评价)*/