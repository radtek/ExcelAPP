using ExcelClient.WsGetDataClient;
using System;
using System.Data;
using System.Text;
using System.Threading;

namespace ExcelClient
{
    public class WebSvrGetData
    {

        public WebSvrGetData()
        {
        }

        public const int pgNum = 10000;
        public const string encstr = "!@#@!";

        public static DataSet GetDataSet(string processID, string sql, string strOrderKey, WsGetDataClient.WSGetData mgr, string fieldStr)
        {
            DataSet ds = new DataSet();
            sql = encstr + DesEncrypt.Encrypt(sql);
            ds = StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByte(processID, sql));
            return ds;
        }
        public static DataTable getDataTable(string processID, WsGetDataClient.WSGetData mgr, string sql)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            sql = encstr + DesEncrypt.Encrypt(sql);
            ds = StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByte(processID, sql));
            if (ds.Tables.Count > 0)
                dt = ds.Tables[0];
            return dt;
        }
        public static DataSet getDataSet(string processID, string ProName, string[] paramArr, string[] valueArr, WsGetDataClient.WSGetData mgr)
        {
            DataSet ds = new DataSet();
            ds = StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByteProc(processID, ProName, paramArr, valueArr));
            return ds;
        }

        public static DataTable getDataTable(string processID, string ProName, string[] paramArr, string[] valueArr, WsGetDataClient.WSGetData mgr)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            ds = StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByteProc(processID, ProName, paramArr, valueArr));
            if (ds.Tables.Count > 0)
                dt = ds.Tables[0];
            return dt;
        }
        public static DataSet GetDataSet(string processID, string sql, string strOrderKey, WsGetDataClient.WSGetData mgr, string fieldStr, string IsRepeatDown)
        {
            DataSet ds = new DataSet();
            sql = encstr + DesEncrypt.Encrypt(sql);
            if (IsRepeatDown == "1")
            {
                string countsql = "select count(1) " + sql.Substring(sql.ToLower().IndexOf(" from "));
                DataSet dscount = mgr.getDataSet(processID, countsql);
                long dtcount = 0;
                if (dscount != null && dscount.Tables.Count > 0)
                    dtcount = Convert.ToInt64(dscount.Tables[0].Rows[0][0]);
                if (dtcount <= pgNum)
                {
                    ds = StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByte(processID, sql));
                }
                else
                {
                    WebSvrGetDataAsync getDataSync = new WebSvrGetDataAsync(processID, dtcount, sql, strOrderKey, mgr, fieldStr);
                    ds = getDataSync.returnDataSet();
                }
            }
            else
            {
                ds = StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByte(processID, sql));
            }
            return ds;
        }

        public static void execsql(string processID, string sql, WsGetDataClient.WSGetData mgr)
        {
            sql = encstr + DesEncrypt.Encrypt(sql);
            mgr.execsql(processID, sql);
        }

        public static void execsqlAysnc(string processID, string sql, WsGetDataClient.WSGetData mgr)
        {
            sql = encstr + DesEncrypt.Encrypt(sql);
            mgr.execsqlAsync(processID, sql);
            mgr.execsqlCompleted += new execsqlCompletedEventHandler((o, a) => {
                Console.WriteLine(o.ToString());
                //Response.Write(string.Format("完成时间：{0}。 结果{1}<Br>", DateTime.Now.ToString("mm:ss.ffff"), a.Result));
            });
        }
    }
    #region 同步获取
    public class WebSvrGetDataSync
    {
        private DataSet ds = null;
        private long loadNum = 0;

        public DataSet returnDataSet()
        {
            return ds;
        }
        public WebSvrGetDataSync(string processID, long dtCount, string sql, string strOrderKey, WsGetDataClient.WSGetData mgr)
        {

            sql = WebSvrGetData.encstr + DesEncrypt.Encrypt(sql);
            loadNum = dtCount % WebSvrGetData.pgNum == 0 ? dtCount / WebSvrGetData.pgNum : dtCount / WebSvrGetData.pgNum + 1;
            syncGetDataSet(processID, dtCount, sql, strOrderKey, mgr);
        }

        private void syncGetDataSet(string processID, long dtcount, string sql, string strOrderKey, WsGetDataClient.WSGetData mgr)
        {
            sql = WebSvrGetData.encstr + DesEncrypt.Encrypt(sql);
            string dbtype = mgr.getDBType(processID);
            string vssql = "";
            if (dbtype == "ora")
            {
                vssql = "select * from ( {0} )z1 where rownum>{1} and rownum<={2} ";
            }
            else
            {
                if (string.IsNullOrEmpty(strOrderKey)) throw new Exception("数据量过大，请联系开发人员调用异步加载方式！");
                vssql = @"select top {1} *  from 
                ( select top {2}  * from ({0}) z1 order by {3}) z2 
                order by {3} desc ";
            }
            for (int m = 0; m < loadNum; m++)
            {
                long longs = m * WebSvrGetData.pgNum;
                long longe = (m + 1) * WebSvrGetData.pgNum;
                if (dbtype == "ora")
                {
                    sql = string.Format(vssql, sql, longs + "", longe + "");
                }
                else
                {
                    sql = string.Format(vssql, sql, WebSvrGetData.pgNum, longe + "", strOrderKey);
                }
                DataSet dsre = StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByte(processID, sql));
                if (ds == null) ds = dsre.Copy();
                else
                {
                    foreach (DataRow row in dsre.Tables[0].Rows)
                    {
                        ds.Tables[0].Rows.Add(row.ItemArray);
                    }
                }

            }

        }
    }
    #endregion

    public class WebSvrGetDataAsyncDelegate
    {
        public WebSvrGetDataAsyncDelegate()
        {
        }
        public DataSet getData(WsGetDataClient.WSGetData mgr, string processID, string sql)
        {
            sql = WebSvrGetData.encstr + DesEncrypt.Encrypt(sql);
            return StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByte(processID, sql));
        }

        public void execsql(WsGetDataClient.WSGetData mgr, string processID, string sql)
        {
            sql = WebSvrGetData.encstr + DesEncrypt.Encrypt(sql);
            mgr.execsql(processID, sql);
        }

    }

    public delegate DataSet AsyncDelegate(WsGetDataClient.WSGetData mgr, string processID, string sql);

    public class WebSvrGetDataAsync
    {
        /*  int threadID;
             AsyncDemo ad = new AsyncDemo();
             AsyncDelegate andl = new AsyncDelegate(ad.TestMethod);
             IAsyncResult ar = andl.BeginInvoke(3000, out threadID, null, null);
             Thread.Sleep(10);
             Console.WriteLine("Main Thread {0} Does Some Work",
                 AppDomain.GetCurrentThreadId());
             while (ar.IsCompleted == false)
             {
                         Thread.Sleep(10);
             }
             Console.WriteLine("其实很简单");
             string ret = andl.EndInvoke(out threadID, ar);

*/
        private DataSet ds = new DataSet();
        private long _dtCount = 0;
        private long loadNum = 0;
        AsyncDelegate[] delArr;
        IAsyncResult[] arArr;
        private string FieldStr;

        public WebSvrGetDataAsync(string processID, long dtCount, string sql, string strOrderKey, WsGetDataClient.WSGetData mgr, string fieldStr)
        {
            _dtCount = dtCount;
            FieldStr = fieldStr;
            sql = WebSvrGetData.encstr + DesEncrypt.Encrypt(sql);
            loadNum = dtCount % WebSvrGetData.pgNum == 0 ? dtCount / WebSvrGetData.pgNum : dtCount / WebSvrGetData.pgNum + 1;
            ansyncGetDataSet(processID, dtCount, sql, strOrderKey, mgr);
        }

        private void ansyncGetDataSet(string processID, long dtcount, string sql, string strOrderKey, WsGetDataClient.WSGetData mgr)
        {
            string dbtype = mgr.getDBType(processID);
            string vssql = "";
            string sqlexe = "";
            if (dbtype == "ora")
            {
                vssql = " select * from (select rownum rn {3} from ( {0} )z1)z2 where rn>{1} and rn<={2} ";
            }
            else
            {
                if (string.IsNullOrEmpty(strOrderKey)) throw new Exception("数据量过大，请联系开发人员调用异步加载方式！");
                vssql = @"select top {1} *  from 
                ( select top {2}  * from ({0}) z1 order by {3}) z2 
                order by {3} desc ";
            }
            if (loadNum > 0)
            {
                delArr = new AsyncDelegate[loadNum];
                arArr = new IAsyncResult[loadNum];
                bool[] blLoadDs = new bool[loadNum];
                for (int m = 0; m < loadNum; m++)
                {
                    blLoadDs[m] = false;
                    long longs = m * WebSvrGetData.pgNum;
                    long longe = (m + 1) * WebSvrGetData.pgNum;
                    if (m == loadNum - 1)
                    {
                        longe = _dtCount;
                    }
                    if (dbtype == "ora")
                    {
                        sqlexe = string.Format(vssql, sql, longs + "", longe + "", FieldStr);
                    }
                    else
                    {
                        sqlexe = string.Format(vssql, sql, Convert.ToString(longe - longs), longe + "", strOrderKey);
                    }
                    sqlexe = WebSvrGetData.encstr + DesEncrypt.Encrypt(sqlexe);
                    WebSvrGetDataAsyncDelegate delobj = new WebSvrGetDataAsyncDelegate();
                    delArr[m] = new AsyncDelegate(delobj.getData);
                    arArr[m] = delArr[m].BeginInvoke(mgr, processID, sqlexe, null, null);
                }

                bool delCompleted = false;
                while (delCompleted == false)
                {
                    delCompleted = true;
                    for (int n = 0; n < loadNum; n++)
                    {
                        IAsyncResult ar = arArr[n];
                        if (!ar.IsCompleted)
                        {
                            delCompleted = false;
                            break;
                        }
                        else
                        {
                            if (!blLoadDs[n])
                            {
                                blLoadDs[n] = true;
                                DataSet dsre = delArr[n].EndInvoke(ar);
                                ds.Merge(dsre);
                            }
                        }
                    }
                    Thread.Sleep(10);
                }
            }

        }



        
        public DataSet returnDataSet()
        {
            return ds;
        }
    }





}
