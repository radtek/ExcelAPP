using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Xml;
using System.Text;
using System.IO;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Configuration;
using System.Globalization;
using System.Web.Security;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Dapper;
using Newtonsoft.Json;
using ExcelAPPWeb.Model;
namespace ExcelAPPWeb
{
    public class YzDrpInterFace
    {
        string Url = ConfigurationSettings.AppSettings["InterFaceUrl"];
        string appid = ConfigurationSettings.AppSettings["InterFaceAppID"];
        string usercode = ConfigurationSettings.AppSettings["InterFaceUserCode"];
        Encoding encode = Encoding.Default;
        public  string DrpIMInterFace(string ywid, List<IDictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
            {
            var  reparm= JsonConvert.SerializeObject(new { Bills = list });
            string parms = "",ret="", classsetcode="";
                /*IMGM	库存单据
                  SalesOrder	销售订单
                  RPBills	收付款单
                  GoodsRecReq	入库通知单
                  IMGIGIR	要货申请单
                  Customers	往来单位
                  Materials	物料档案
                  BillsOfLading	销售交货单
                  SalesAgreements	销售协议
                  ARSKD	GS7收款单
                  ARYSD	GS7应收单
                  SalesInvoices	销售发票
                  */
            var time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            switch (ywid)
            {
                case "CJRK":
                    classsetcode = "IMGM"; //库存单据
                    break;
                case "YZDD":
                    classsetcode = "SalesOrder";//销售订单
                    break;
                case "YZTD":
                    classsetcode = "BillsOfLading";//销售交货单
                    break;
                case "YZFP":
                    classsetcode = " SalesInvoices";//销售发票
                    break;
                case "YZSK":
                    classsetcode = " ARSKD";//GS7收款单
                    break;
                case "YZYS":
                    classsetcode = " ARYSD";//GS7应收单
                    break;
                case "YZSFK":
                    classsetcode = "  RPBills";//收付款单
                    break;
                   
            }
           

           var  UrlS = Url + "/cwbase/api/ToDataAPI.ashx";
            parms = "token=9999999999999999999&time=" + time + "&classsetcode=" + classsetcode + "&format=JSON&param=" + reparm + "&configid=&appid=" + appid + "&usercode=" + usercode + "&logindate=&comid=";
            ret= GetJson(UrlS, parms, encode);
            DataSet seter = null;
            seter = Json2DataSet(ret);
            if (((seter != null) && (seter.Tables.Count > 0)) && (seter.Tables[0].Rows.Count >= 1))
            {
                for (int i = 0; i < seter.Tables[0].Rows.Count; i++)
                {

                    string strok = Convert.ToString(seter.Tables[0].Rows[i]["State"]).Trim();
                    if (strok == "0")
                    {
                        throw new Exception("错误！" + Convert.ToString(seter.Tables[0].Rows[i]["FailReason"]).Trim());
                    }
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    dict.Add("p_QueryCode", ywid);
                    dict.Add("p_GSDWBH", model.DWBH);
                    dict.Add("p_PKID", Convert.ToString(seter.Tables[0].Rows[i]["PKID"]).Trim());
                    dict.Add("p_BillID", Convert.ToString(seter.Tables[0].Rows[i]["BillID"]).Trim());
                    dict.Add("p_BillCode", Convert.ToString(seter.Tables[0].Rows[i]["BillCode"]).Trim());
                    Service.ProcHelper.ExecProc(dict, "Sp_Drp_YZInterFaceAF", db, transaction);
                }
            }
            return "ok";
        }

 
        public string DataSet2Json(DataSet ds)
        {
            return JsonConvert.SerializeObject(ds);
        }

        public DataSet Json2DataSet(string sjson)
        {
            return (DataSet)JsonConvert.DeserializeObject(sjson, typeof(DataSet));
        }

        private string get_token(ref string p_string, string p_seperator)
        {
            int length = 0;
            string str = "";
            length = p_string.IndexOf(p_seperator);
            if (length >= 0)
            {
                str = p_string.Substring(0, length);
                p_string = p_string.Substring(length + p_seperator.Length);
                return str;
            }
            str = p_string;
            p_string = "";
            return str;
        }
        public static string GetJson(string url, string parms, Encoding Encode)
        {
            HttpWebResponse response;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] bytes = Encode.GetBytes(parms);
            request.ContentLength = bytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            try
            {
                response = (HttpWebResponse)request.GetResponse();
               WriteLogFile("URL参数接口：" + url + parms);
            }
            catch (WebException exception)
            {
                response = (HttpWebResponse)exception.Response;
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encode);
            return reader.ReadToEnd();
        }

        #region 记录操作日志文件版本
        public static void WriteLogFile(string txt)
        {
            string path = @"C:\InterFace\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                DateTime now = DateTime.Now;
                StreamWriter writer = new StreamWriter(path + "GS_InterFace_Log" + now.ToString("yyyyMMdd") + ".txt", true, Encoding.Default);
                writer.WriteLine(string.Concat(new object[] { now, "      :      ", txt, "\r\n" }));
                writer.Close();
            }
            catch (Exception)
            {
            }
        }

        #endregion



    }
}