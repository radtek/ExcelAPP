using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ExcelClient
{
    public  class LSTIGSDEVQRYMgr
    {

             WsGetDataClient.WSGetData mgr;
        private string ProcessID=DevQryPubFun.GSYDBSrc;

        public LSTIGSDEVQRYMgr(WsGetDataClient.WSGetData vsmgr, string vsPressID)
        {
            mgr=vsmgr; 
        }

        public void delete(string ID)
        {
            string sql = string.Format("delete from LSTIGS where F_ID='{0}'", ID);
            WebSvrGetData.execsql(this.ProcessID, sql, mgr);
             
        }

        public void Save(DataTable dt, string ID)
        {
            JTPUBQRDEFMgr defmgr = new JTPUBQRDEFMgr(mgr, ProcessID);
            JTPUBQRDEFEty ety = defmgr.getEty(ID);
            string sql = "begin ";
            sql += string.Format("DELETE FROM LSZBGS WHERE F_ID='{0}' and F_DWBH=' ';", ID);
            sql += "INSERT INTO LSZBGS(F_DWBH,F_ID,F_GSBH,F_GSMC,F_CAPT,F_CAPH,F_SCAPH,F_BTGD,F_ROWH,F_BWGD,F_SPACE,F_SYBZ) ";
            sql += string.Format("VALUES (' ','{0}','01','系统格式','{1}',30,30,20,20,0,10,'0');", ID, ety.JTPUBQRDEF_TITLE);
            sql += string.Format("DELETE FROM LSOTGS WHERE F_ID='{0}'   and F_DWBH=' ';", ID);
            sql += "INSERT INTO LSOTGS(F_DWBH,F_ID,F_GSBH,F_OTBH,F_TEXT,F_OTBZ,F_JS,F_ALIGN)";
            sql += string.Format("Values(' ','{0}','01','01','{1}','H','1','L');", ID, ety.JTPUBQRDEF_SUBTIL);
            sql += string.Format("delete from LSTIGS where F_ID='{0}' and F_DWBH=' ';", ID);
            if (dt.Rows.Count > 0)
            {
                /*F_DWBH,F_ID,F_FHBZ,F_GSBH,F_TIBH,F_JS,F_FIELD,F_TEXT,F_TYPE,F_ALIGN,
F_WIDTH,F_PREC,F_HJBZ,F_YHBZ*/
                string vsinsert = @"INSERT INTO LSTIGS(F_DWBH,F_ID,F_GSBH,F_TIBH,F_JS,F_FIELD,F_TEXT,F_TYPE,F_ALIGN,F_WIDTH,F_PROP,F_PREC,F_DISP,F_REAL,F_HJBZ,F_PXBZ,F_YHBZ,F_FHBZ,F_HZBZ,F_TSTEXT,F_GROUP,F_TSHID,F_ISGD,F_PSUMTYPE,F_FORMAT)
Values(' ','{0}','01','{1}','{2}','{3}','{4}','{5}','{6}',{7},' ','{8}',' ',' ','{9}','0','{10}','0','0','{11}','{12}','{13}','{14}','{15}','{16}');";
                foreach (DataRow row in dt.Rows)
                {
                    sql += string.Format(vsinsert, row["F_ID"].ToString(), row["F_TIBH"].ToString(),row["F_JS"].ToString(),
                        row["F_FIELD"].ToString(), row["F_TEXT"].ToString(), row["F_TYPE"].ToString(), row["F_ALIGN"].ToString(), row["F_WIDTH"].ToString(),
                        row["F_PREC"].ToString(), row["F_HJBZ"].ToString(), row["F_YHBZ"].ToString(),
 row["F_TSTEXT"].ToString(), row["F_GROUP"].ToString(), row["F_TSHID"].ToString(), row["F_ISGD"].ToString(), row["F_PSUMTYPE"].ToString(), row["F_FORMAT"].ToString());

                }

            }
            sql += " end;";
            WebSvrGetData.execsql(this.ProcessID, sql, mgr);
        }
    }
}
