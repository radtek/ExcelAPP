using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Data;

namespace ExcelClient
{

    public class WebSvrThreadGetData
    {

        private Thread[] threads;
        
        private DataSet ds;
        private event EventHandler OnNumberClear;//数据删除完成引发的事件
        private string processID;
        private long pgnum;
        private string sql;
        int i = 0;
        WsGetDataClient.WSGetData mgr;
        frmDevBBShow frmshow;
        private string strOrderKey;
        //public static void Main()
        //{
        //    ThreadDemo demo = new ThreadDemo(1000);
        //    demo.Action();

        //}
        public WebSvrThreadGetData(string psprocessID, long psPgnum, string psSql, string psOrderKey, WsGetDataClient.WSGetData psMgr, frmDevBBShow psFrmshow)
        {
            processID = psprocessID;
            pgnum = psPgnum;
            sql = psSql;
            strOrderKey = psOrderKey;
            mgr = psMgr;
            frmshow = psFrmshow;
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
            threads = new Thread[pgnum];
            for (int m = 0; m < pgnum; m++)
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
                threads[m] = new Thread(new ParameterizedThreadStart(getData));
                threads[m].Start(sql);

            }
            OnNumberClear += new EventHandler(ThreadDemo_OnNumberClear);
        }

        /// <summary>
        /// 共同做的工作
        /// </summary>
        /// 
        private void getData(object sql)
        {
            while (true)
            {  
                Monitor.Enter(this);//锁定，保持同步            
                DataSet dsre = StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByte(processID, sql as string));
                i = i + 1;
                if (ds == null) ds = dsre.Copy();
                else
                {
                    foreach (DataRow row in dsre.Tables[0].Rows)
                    {
                        ds.Tables[0].Rows.Add(row.ItemArray);
                    }
                    if (i == pgnum)
                    {
                        OnNumberClear(this, new EventArgs());//引发完成事件
                    }
                }
                Monitor.Exit(this);//取消锁定
                Thread.Sleep(5);
            }
        }


        //执行完成之后，停止所有线程
        void ThreadDemo_OnNumberClear(object sender, EventArgs e)
        {
           // frmshow.AnsyncBindGrid(ds);
            foreach (Thread thread in threads)
            {
                thread.Abort();
            }
            
        }
    }

}
