using System;
using System.Data;

using System.Web;
using System.Web.Services;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using NPoco;
using ExcelAPPWeb.DB;
using System.Collections.Generic;
using ExcelAPPWeb.Service;
using ExcelDataBase;

namespace ExcelAPPWeb
{
    /// <summary>
    /// Summary description for WSGetData
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WSGetData : System.Web.Services.WebService
    {

        /// </summary> 
        /// <param name="List">泛型集合</param> 
        /// <returns></returns> 
        public DataSet ConvertToDataSet<T>(IList<T> list)
        {




            if (list == null || list.Count <= 0)
            {
                return null;
            }
            DataSet ds = new DataSet();
            DataTable dt = new DataTable(typeof(T).Name);
            DataColumn column;
            DataRow row;
            System.Reflection.PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (T t in list)
            {
                if (t == null)
                {
                    continue;
                }
                row = dt.NewRow();
                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    System.Reflection.PropertyInfo pi = myPropertyInfo[i];
                    string name = pi.Name;
                    if (dt.Columns[name] == null)
                    {
                        column = new DataColumn(name, pi.PropertyType);
                        dt.Columns.Add(column);
                    }
                    row[name] = pi.GetValue(t, null);
                }
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            return ds;
        }
        public const string encstr = "!@#@!";

        private string DecSql(string sql)
        {
            if (sql.IndexOf(encstr) != -1)
            {
                sql = DesEncrypt.Decrypt(sql.Substring(encstr.Length));
            }
            return sql;
        }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod(EnableSession = true)]
        public void LoginByPID(string PID)
        {
            // GS_PublicVar.Dev_LoginGS(PID);
        }
        [WebMethod(EnableSession = true)]
        public string getDBType(string PID)
        {
            ExcelDataBase.Database db = DbFunction.GetDB("");

            return db.DBType.Equals(DBType.ORA) ? "ORA" : "MSS";

            //GS_PublicVar.Dev_LoginGS(PID);
            //return Convert.ToString(CommonFunction.GlobalVariable["Genersoft.BS.Public.DbType"]).Trim();
        }
        [WebMethod(EnableSession = true)]
        public DataSet getDataSet(string processID, string sql)
        {
            string tmp = "";
            try
            {
                sql = DecSql(sql);

                return DbFunction.ExecuteDataSet(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(sql + tmp + " <br> " + ex.Message);
            }
        }

        //[WebMethod(EnableSession = true)]
        //public DataSet getDataSetProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        //{
        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    using (var conn = DataBaseManager.GetDbConnection())
        //    {
        //        //conn.Open();
        //        IDbTransaction transaction = conn.BeginTransaction();
        //        int i = 0;
        //        foreach (var key in paramArr)
        //        {
        //            dict.Add("key", valueArr[i]);
        //            i++;
        //        }


        //        var list = ProcHelper.GetProcDataOracle(dict, ProcName, conn, transaction);
        //        DataSet ds = ConvertToDataSet(list);
        //        return ds;
        //    }
        //}
        [WebMethod(EnableSession = true)]
        public DataSet getDataSetProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        {

            DataSet ds = new DataSet();
            // Database db = DataBaseManager.GetDB();
            ExcelDataBase.Database db = DbFunction.GetDB("");

            try
            {
                int paramLen = 0;
                DbParam[] myParam = null;

                if (paramArr != null)
                {
                    paramLen = paramArr.Length;
                }
                if (db.DBType == DataBaseType.ORA)
                    paramLen = paramLen + 1;

                if (paramLen > 0)
                    myParam = new DbParam[paramLen];


                if (paramArr != null)
                {
                    for (int i = 0; i < paramArr.Length; i++)
                    {
                        DbParam vsparam = new DbParam();

                        vsparam.dataType = DataType.VarChar;
                        vsparam.paramDirct = ParameterDirection.Input;
                        vsparam.paramName = paramArr[i];
                        vsparam.paramValue = valueArr[i];
                        vsparam.sqlType = SqlType.Proc;
                        myParam[i] = vsparam;
                    }
                }
                if (db.DBType == DataBaseType.ORA)
                {
                    DbParam vsparam = new DbParam();

                    vsparam.dataType = DataType.Cursor;
                    vsparam.paramDirct = ParameterDirection.Output;
                    vsparam.paramName = "Re_CURSOR";
                    vsparam.paramValue = "";
                    vsparam.sqlType = SqlType.Proc;
                    myParam[myParam.Length - 1] = vsparam;
                }

                ds = db.ExecuteDataSet(ProcName, myParam);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        [WebMethod(EnableSession = true)]
        public void execsql(string processID, string sql)
        {
            sql = DecSql(sql);
            DbFunction.ExecuteNonQuery(sql);

        }

        [WebMethod(EnableSession = true)]
        public string getDataXML(string processID, string sql)
        {
            sql = DecSql(sql);
            DataSet ds = getDataSet(processID, sql);
            return ds.GetXml();
        }
        [WebMethod(EnableSession = true)]
        public string getDataXMLProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        {
            DataSet ds = getDataSetProc(processID, ProcName, paramArr, valueArr);
            return ds.GetXml();
        }
        [WebMethod(EnableSession = true)]
        public string[] getDataXMLWithSchema(string processID, string sql)
        {
            sql = DecSql(sql);
            DataSet ds = getDataSet(processID, sql);
            string[] arr = new string[2];
            arr[0] = ds.GetXmlSchema();
            arr[1] = ds.GetXml();
            return arr;
        }
        [WebMethod(EnableSession = true)]
        public string[] getDataXMLWithSchemaProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        {
            DataSet ds = getDataSetProc(processID, ProcName, paramArr, valueArr);
            string[] arr = new string[2];
            arr[0] = ds.GetXmlSchema();
            arr[1] = ds.GetXml();
            return arr;
        }
        [WebMethod(EnableSession = true)]
        public byte[] getDataXmlByte(string processID, string sql)
        {
            sql = DecSql(sql);
            string xml = getDataXML(processID, sql);
            return System.Text.Encoding.UTF8.GetBytes(xml);
        }
        [WebMethod(EnableSession = true)]
        public byte[] getDataXmlByteProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        {
            string xml = getDataXMLProc(processID, ProcName, paramArr, valueArr);
            return System.Text.Encoding.UTF8.GetBytes(xml);
        }
        [WebMethod(EnableSession = true)]
        public byte[][] getDataXmlByteWithSchema(string processID, string sql)
        {
            sql = DecSql(sql);
            string[] xmlarr = getDataXMLWithSchema(processID, sql);
            byte[][] bytearr = new byte[2][];
            bytearr[0] = System.Text.Encoding.UTF8.GetBytes(xmlarr[0]);
            bytearr[1] = System.Text.Encoding.UTF8.GetBytes(xmlarr[1]);
            return bytearr;
        }
        [WebMethod(EnableSession = true)]
        public byte[][] getDataXmlByteWithSchemaProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        {
            string[] xmlarr = getDataXMLWithSchemaProc(processID, ProcName, paramArr, valueArr);
            byte[][] bytearr = new byte[2][];
            bytearr[0] = System.Text.Encoding.UTF8.GetBytes(xmlarr[0]);
            bytearr[1] = System.Text.Encoding.UTF8.GetBytes(xmlarr[1]);
            return bytearr;
        }
        [WebMethod(EnableSession = true)]
        public byte[] getDataZipByte(string processID, string sql)
        {
            sql = DecSql(sql);
            byte[] btarr = getDataXmlByte(processID, sql);
            return zipByte(btarr);
        }
        [WebMethod(EnableSession = true)]
        public byte[] getDataZipByteProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        {
            byte[] btarr = getDataXmlByteProc(processID, ProcName, paramArr, valueArr);
            return zipByte(btarr);
        }
        [WebMethod(EnableSession = true)]
        public byte[][] getDataZipByteWithSchema(string processID, string sql)
        {
            sql = DecSql(sql);
            byte[][] bytearr = getDataXmlByteWithSchema(processID, sql);
            bytearr[0] = zipByte(bytearr[0]);
            bytearr[1] = zipByte(bytearr[1]);
            return bytearr;
        }
        [WebMethod(EnableSession = true)]
        public byte[][] getDataZipByteWithSchemaProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        {
            byte[][] bytearr = getDataXmlByteWithSchemaProc(processID, ProcName, paramArr, valueArr);
            bytearr[0] = zipByte(bytearr[0]);
            bytearr[1] = zipByte(bytearr[1]);
            return bytearr;
        }

        [WebMethod(EnableSession = true)]
        public string getDataZipString(string processID, string sql)
        {
            sql = DecSql(sql);
            byte[] btarr = getDataXmlByte(processID, sql);
            byte[] rearr = zipByte(btarr);
            return System.Text.Encoding.UTF8.GetString(rearr);
        }
        [WebMethod(EnableSession = true)]
        public string getDataZipStringProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        {
            byte[] btarr = getDataXmlByteProc(processID, ProcName, paramArr, valueArr);
            byte[] rearr = zipByte(btarr);
            return System.Text.Encoding.UTF8.GetString(rearr);
        }

        [WebMethod(EnableSession = true)]
        public string[] getDataZipStringWithSchema(string processID, string sql)
        {
            sql = DecSql(sql);
            byte[][] bytearr = getDataXmlByteWithSchema(processID, sql);
            bytearr[0] = zipByte(bytearr[0]);
            bytearr[1] = zipByte(bytearr[1]);
            string[] rearr = new string[2];
            rearr[0] = System.Text.Encoding.UTF8.GetString(bytearr[0]);
            rearr[1] = System.Text.Encoding.UTF8.GetString(bytearr[1]);
            return rearr;
        }
        [WebMethod(EnableSession = true)]
        public string[] getDataZipStringWithSchemaProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        {
            byte[][] bytearr = getDataXmlByteWithSchemaProc(processID, ProcName, paramArr, valueArr);
            bytearr[0] = zipByte(bytearr[0]);
            bytearr[1] = zipByte(bytearr[1]);
            string[] rearr = new string[2];
            rearr[0] = System.Text.Encoding.UTF8.GetString(bytearr[0]);
            rearr[1] = System.Text.Encoding.UTF8.GetString(bytearr[1]);
            return rearr;
        }

        [WebMethod(EnableSession = true)]
        public byte[] getDataFormatByte(string processID, string sql)
        {
            sql = DecSql(sql);
            byte[] byarr = null;
            DataSet ds = getDataSet(processID, sql);
            ds.RemotingFormat = SerializationFormat.Binary;
            MemoryStream ms = new MemoryStream();
            IFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, ds);
            byarr = ms.ToArray();
            ds.Dispose();
            ms.Close();
            ms.Dispose();//释放资源
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return byarr;

        }
        [WebMethod(EnableSession = true)]
        public byte[] getDataFormatByteProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        {
            byte[] byarr = null;
            DataSet ds = getDataSetProc(processID, ProcName, paramArr, valueArr);
            ds.RemotingFormat = SerializationFormat.Binary;
            MemoryStream ms = new MemoryStream();
            IFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, ds);
            byarr = ms.ToArray();


            ds.Dispose();
            ms.Close();
            ms.Dispose();//释放资源
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return byarr;

        }

        [WebMethod(EnableSession = true)]
        public byte[] getZipDataFormatByte(string processID, string sql)
        {
            sql = DecSql(sql);
            byte[] byarr = getDataFormatByte(processID, sql);

            return zipByte(byarr);

        }
        [WebMethod(EnableSession = true)]
        public byte[] getZipDataFormatByteProc(string processID, string ProcName, string[] paramArr, string[] valueArr)
        {
            byte[] byarr = getDataFormatByteProc(processID, ProcName, paramArr, valueArr);
            return zipByte(byarr);

        }
        /*[WebMethod(Description="获取业务资料远程DATASET")]
      public byte[] SurrogateRead1()
      {
       DataSet ds;
       ds=SqlHelper.ExecuteDataset(cnn,CommandType.Text,"select * from t_busdocbase");
       sds = new DataSetSurrogate(ds); 
       MemoryStream s= new MemoryStream();
       BinaryFormatter bf = new BinaryFormatter();
       bf.Serialize(s,sds);
     
       byte[] e = s.ToArray();
       return e; 
  
      }
*/
        //public byte[] zipByte(byte[] btarr)
        //{

        //    System.IO.MemoryStream inms = new System.IO.MemoryStream(btarr);
        //    System.IO.MemoryStream outms = new System.IO.MemoryStream();
        //    //do the compressions/uncompressions 
        //    ICSharpCode.SharpZipLib.BZip2.BZip2.Compress(inms, outms, 1024);
        //    byte[] result = outms.ToArray();
        //    return result;
        //}
        //public byte[] zipByte(byte[] btarr)
        //{

        //    MemoryStream ms = new MemoryStream();
        //    //调用免费开源控件方法
        //    Stream s = new ZOutputStream(ms, 9);
        //    s.Write(btarr, 0, btarr.Length);
        //    s.Close();
        //    byte[] compressData = (byte[])ms.ToArray();
        //    ms.Flush();
        //    ms.Close();
        //    return compressData;

        //}

        public byte[] zipByte(byte[] btarr)
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

    }
}
