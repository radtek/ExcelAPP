using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ExcelClient
{
    public class BBGsHelp
    {
        public BBGsHelp()
        {
        }




        public static void saveBBTitle(string processID, WsGetDataClient.WSGetData mgr, DataTable dt,string psDWBH,string psYear,string psType,string psBH,string psMC)
        {
            psDWBH = string.IsNullOrEmpty(psDWBH) ? " " : psDWBH;
            string sql = @"insert into LSZBGS{12} (F_DWBH, F_ID, F_GSBH, F_GSMC,
    F_CAPT, F_CAPH, F_SCAPH, F_BTGD, F_ROWH, F_BWGD, F_SPACE, F_SYBZ)values
    ('{0}', '{1}', '{2}', '{3}','{4}', {5}, {6}, {7}, {8}, {9}, {10}, '{11}') ";
   
            if (dt.Rows.Count > 0)
            {
                DataRow row=dt.Rows[0];
                if (psType == "edit")
                {
                    sql = @" update LSZBGS{12} set F_GSMC='{3}',F_CAPT='{4}', F_CAPH={5}, F_SCAPH={6},
                    F_BTGD={7}, F_ROWH={8}, F_BWGD={9}, F_SPACE={10}, F_SYBZ='{11}'  where F_DWBH='{0}' and F_ID='{1}' and   F_GSBH='{2}' ";
                }
                else
                {
                    WebSvrGetData.execsql(processID, string.Format("delete from LSZBGS{0} where F_DWBH='{1}' and F_ID='{2}' and  F_GSBH='{3}' ", psYear, psDWBH, row["F_ID"].ToString(), psBH), mgr);
                     
                }
                sql = string.Format(sql, psDWBH, row["F_ID"].ToString(),psBH,
                     psMC,row["F_CAPT"].ToString(),row["F_CAPH"].ToString(),row["F_SCAPH"].ToString(),
                     row["F_BTGD"].ToString(),row["F_ROWH"].ToString(),row["F_BWGD"].ToString(),
                     row["F_SPACE"].ToString(),row["F_SYBZ"].ToString(),psYear);
                WebSvrGetData.execsql(processID, sql, mgr);
            }

        }
        public static void saveBBSubTitle(string processID, WsGetDataClient.WSGetData mgr, DataTable dt, string psDWBH, string psYear, string psType, string psBH)
        {
            psDWBH = string.IsNullOrEmpty(psDWBH) ? " " : psDWBH;
            string sql = @"insert into LSOTGS{8} (F_DWBH, F_ID, F_GSBH, F_OTBH, F_TEXT, F_OTBZ, F_JS, F_ALIGN)values
        ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');";
            string sqldel = "delete from LSOTGS{0} WHERE F_DWBH='{1}' AND  F_ID='{2}' AND F_GSBH='{3}' AND F_OTBH='{4}' ; ";
            if (dt.Rows.Count > 0)
            {
                if (psType == "edit")
                {
                    sql = @" update LSOTGS{8} set    F_TEXT='{4}', F_OTBZ='{5}', F_JS='{6}', F_ALIGN='{7}'
                     where F_DWBH='{0}' and  F_ID='{1}' and F_GSBH='{2}' and F_OTBH='{3}';";
                } 
                 string vssql="";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vsOTBH = i.ToString().PadLeft(2, '0');
                     DataRow row=dt.Rows[i];
                     vssql += string.Format(sqldel, psYear, psDWBH, row["F_ID"].ToString(), psBH, vsOTBH);
                     vssql += string.Format(sql, psDWBH, row["F_ID"].ToString(),psBH,
                         vsOTBH, row["F_TEXT"].ToString(), row["F_OTBZ"].ToString(), row["F_JS"].ToString(), row["F_ALIGN"].ToString(),psYear);
                    // OleDbHelp.execSql(vssql);
                }
                vssql = "begin " + vssql + "  end;";
                WebSvrGetData.execsql(processID, vssql, mgr);
            }
        }
        public static void insertDevRowGS(string processID, WsGetDataClient.WSGetData mgr, DataTable dt, string psDWBH, string psYear, string psBH)
        {
            psDWBH = string.IsNullOrEmpty(psDWBH) ? " " : psDWBH;
            string sql = @"insert into {20} (F_DWBH, F_ID, F_GSBH, F_TIBH, 
F_JS, F_FIELD,F_TEXT, F_TYPE,F_ALIGN, F_WIDTH, F_PROP, F_PREC, 
F_DISP, F_REAL, F_HJBZ, F_PXBZ, F_YHBZ, F_FHBZ, F_HZBZ, F_COLOR)values
('{0}', '{1}', '{2}', '{3}', 
'{4}', '{5}','{6}', '{7}','{8}', {9}, '{10}', '{11}', 
'{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}');";
            string sqldel = "delete from {0} WHERE F_DWBH='{1}' AND  F_ID='{2}' AND F_GSBH='{3}' AND F_TIBH='{4}' ; ";
            string vssql = "";
            string tbname = "LSTIGS" + psYear;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row["F_FIELD"].ToString() == "F_DT_FIELD" || row["F_FIELD"].ToString() == "F_NUM_FIELD"
                    || row["F_FIELD"].ToString() == "F_TXT_FIELD")
                {
                    tbname = "LSTIGSDEVQRY";
                }
                vssql += string.Format(sqldel,tbname, psDWBH, row["F_ID"].ToString(), psBH,
                    row["F_TIBH"].ToString());
                vssql += string.Format(sql, psDWBH, row["F_ID"].ToString(), psBH,
                    row["F_TIBH"].ToString(), row["F_JS"].ToString(), row["F_FIELD"].ToString(),
                    row["F_TEXT"].ToString(), row["F_TYPE"].ToString(), row["F_ALIGN"].ToString(),
                    row["F_WIDTH"].ToString(), row["F_PROP"].ToString(), row["F_PREC"].ToString(),
                    row["F_DISP"].ToString(), row["F_REAL"].ToString(), row["F_HJBZ"].ToString(),
                    row["F_PXBZ"].ToString(), row["F_YHBZ"].ToString(), row["F_FHBZ"].ToString(),
                    row["F_HZBZ"].ToString(), row["F_COLOR"].ToString(), tbname);
                //  OleDbHelp.execSql(vssql);
            }
            vssql = "begin " + vssql + "  end;";
            WebSvrGetData.execsql(processID, vssql, mgr);
        }
        public static void saveBBRowGS(string processID, WsGetDataClient.WSGetData mgr, DataTable dt, string psDWBH, string psYear, string psType, string psBH)
        {
            psDWBH = string.IsNullOrEmpty(psDWBH) ? " " : psDWBH;
            /*SELECT F_DWBH, F_ID, F_GSBH, F_TIBH, F_JS, F_FIELD, F_TEXT, F_TYPE, F_ALIGN, F_WIDTH, F_PROP, F_PREC, F_DISP, F_REAL, F_HJBZ, F_PXBZ, F_YHBZ, F_FHBZ, F_HZBZ, F_COLOR
FROM LSTIGS;*/
            string sql = @"insert into {20} (F_DWBH, F_ID, F_GSBH, F_TIBH, 
F_JS, F_FIELD,F_TEXT, F_TYPE,F_ALIGN, F_WIDTH, F_PROP, F_PREC, 
F_DISP, F_REAL, F_HJBZ, F_PXBZ, F_YHBZ, F_FHBZ, F_HZBZ, F_COLOR,F_TSTEXT,F_GROUP,F_TSHID,F_ISGD,F_PSUMTYPE,F_FORMAT)values
('{0}', '{1}', '{2}', '{3}', 
'{4}', '{5}','{6}', '{7}','{8}', {9}, '{10}', '{11}', 
'{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}','{21}','{22}','{23}','{24}','{25}','{26}');";
            if (dt.Rows.Count > 0)
            {
               /*增加修改编号功能，实现排序，所有的格式先删除，后增加，update语句不用了
                * 
                if (psType=="edit")
                {
                    sql = @" update {20}  set 
F_JS='{4}', F_FIELD='{5}',F_TEXT='{6}', F_TYPE='{7}',F_ALIGN='{8}', F_WIDTH={9}, F_PROP='{10}', F_PREC='{11}', 
F_DISP='{12}', F_REAL='{13}', F_HJBZ='{14}', F_PXBZ='{15}', F_YHBZ='{16}', F_FHBZ='{17}', F_HZBZ='{18}', F_COLOR='{19}' 
  where  F_DWBH='{0}' and F_ID='{1}' and  F_GSBH='{2}' and  F_TIBH='{3}' ;";
                } */
                string vssql=" ";

                string tbname = "LSTIGS" + psYear;
                vssql += string.Format("delete from {0} where  F_DWBH='{1}' and F_ID='{2}' and  F_GSBH='{3}'; ", tbname, psDWBH, dt.Rows[0]["F_ID"].ToString(), psBH);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    if (row["F_FIELD"].ToString() == "F_DT_FIELD" || row["F_FIELD"].ToString() == "F_NUM_FIELD"
                        || row["F_FIELD"].ToString() == "F_TXT_FIELD")
                    {
                        tbname = "LSTIGSDEVQRY";
                    }

                    vssql += string.Format(sql, psDWBH, row["F_ID"].ToString(), psBH,
                        row["F_TIBH"].ToString(), row["F_JS"].ToString(), row["F_FIELD"].ToString(),
                        row["F_TEXT"].ToString(), row["F_TYPE"].ToString(), row["F_ALIGN"].ToString(),
                        row["F_WIDTH"].ToString(), row["F_PROP"].ToString(), row["F_PREC"].ToString(),
                        row["F_DISP"].ToString(), row["F_REAL"].ToString(), row["F_HJBZ"].ToString(),
                        row["F_PXBZ"].ToString(), row["F_YHBZ"].ToString(), row["F_FHBZ"].ToString(),
                        row["F_HZBZ"].ToString(), row["F_COLOR"].ToString(), tbname,
                        row["F_TSTEXT"].ToString(), row["F_GROUP"].ToString(), row["F_TSHID"].ToString(),
                        row["F_ISGD"].ToString(), row["F_PSUMTYPE"].ToString(), row["F_FORMAT"].ToString());
                    //  OleDbHelp.execSql(vssql);
                }
                vssql = "begin " + vssql + "  end;";
                WebSvrGetData.execsql(processID, vssql, mgr);
            }
        }
    }
}
