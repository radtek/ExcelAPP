using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ExcelClient
{
    public  class JTPUBQRDEFMgr
    {
        WsGetDataClient.WSGetData mgr;
        private string ProcessID=DevQryPubFun.GSYDBSrc;
        public JTPUBQRDEFMgr(WsGetDataClient.WSGetData vsmgr ,string vsPressID)
        {
            mgr=vsmgr;
         
        }
        public JTPUBQRDEFEty getEty(string id)
        {
            JTPUBQRDEFEty ety = new JTPUBQRDEFEty();
            string sql = "select * from JTPUBQRDEF where JTPUBQRDEF_ID='{0}'";
            sql = string.Format(sql, id);
            DataTable dt = getDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                ety.JTPUBQRDEF_ID = dt.Rows[0]["JTPUBQRDEF_ID"].ToString();
                ety.JTPUBQRDEF_BH = dt.Rows[0]["JTPUBQRDEF_BH"].ToString();
                ety.JTPUBQRDEF_MC = dt.Rows[0]["JTPUBQRDEF_MC"].ToString();
                ety.JTPUBQRDEF_TITLE = dt.Rows[0]["JTPUBQRDEF_TITLE"].ToString();
                ety.JTPUBQRDEF_SUBTIL = dt.Rows[0]["JTPUBQRDEF_SUBTIL"].ToString();
                ety.JTPUBQRDEF_TYPE = dt.Rows[0]["JTPUBQRDEF_TYPE"].ToString();
                ety.JTPUBQRDEF_DBSRC = dt.Rows[0]["JTPUBQRDEF_DBSRC"].ToString();
                ety.JTPUBQRDEF_SQL = dt.Rows[0]["JTPUBQRDEF_SQL"].ToString();
                ety.JTPUBQRDEF_ORA = dt.Rows[0]["JTPUBQRDEF_ORA"].ToString(); 
                ety.JTPUBQRDEF_RQFIELD = dt.Rows[0]["JTPUBQRDEF_RQFIELD"].ToString();
                ety.JTPUBQRDEF_WHERE = dt.Rows[0]["JTPUBQRDEF_WHERE"].ToString();
            }
            return ety;
        }

        public DataTable getWhereDt(string qryID)
        {
            string sql = "select * from JTPUBQRPARAMDEF where PARAMDEF_QRYID='{0}'  order by PARAMDEF_ORD ";
            sql = string.Format(sql, qryID);
            DataTable dt = getDataTable(sql);
            foreach (DataRow row in dt.Rows)
                row["PARAMDEF_ISUSR"] = row["PARAMDEF_ISUSR"].ToString() == "1" ? "是" : "否";
            return dt;
        }
        private void add(JTPUBQRDEFEty ety)
        {
            string sql = @"insert into  JTPUBQRDEF
(JTPUBQRDEF_ID,JTPUBQRDEF_BH,JTPUBQRDEF_MC,JTPUBQRDEF_TITLE,JTPUBQRDEF_SUBTIL,JTPUBQRDEF_TYPE,JTPUBQRDEF_SQL,JTPUBQRDEF_DWFIELD,JTPUBQRDEF_RQFIELD,JTPUBQRDEF_WHERE,JTPUBQRDEF_DBSRC,JTPUBQRDEF_ORA)
values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')";
            sql = string.Format(sql, ety.JTPUBQRDEF_ID, ety.JTPUBQRDEF_BH, ety.JTPUBQRDEF_MC, ety.JTPUBQRDEF_TITLE, ety.JTPUBQRDEF_SUBTIL,
                ety.JTPUBQRDEF_TYPE, ety.JTPUBQRDEF_SQL.Replace("'", "''"), ety.JTPUBQRDEF_DWFIELD,
                ety.JTPUBQRDEF_RQFIELD, ety.JTPUBQRDEF_WHERE, ety.JTPUBQRDEF_DBSRC, ety.JTPUBQRDEF_ORA.Replace("'", "''"));

            WebSvrGetData.execsql(this.ProcessID, sql, mgr);
        }
        private void Update(JTPUBQRDEFEty ety)
        {
            string sql = @"begin 
update JTPUBQRDEF  set JTPUBQRDEF_BH='{1}',JTPUBQRDEF_MC='{2}',JTPUBQRDEF_TITLE='{3}',JTPUBQRDEF_SUBTIL='{4}',JTPUBQRDEF_TYPE='{5}',
JTPUBQRDEF_SQL='{6}',JTPUBQRDEF_DWFIELD='{7}',JTPUBQRDEF_RQFIELD ='{8}',JTPUBQRDEF_WHERE='{9}',JTPUBQRDEF_DBSRC='{10}',JTPUBQRDEF_ORA='{11}'
where JTPUBQRDEF_ID='{0}' ;";

            sql = string.Format(sql, ety.JTPUBQRDEF_ID, ety.JTPUBQRDEF_BH, ety.JTPUBQRDEF_MC, ety.JTPUBQRDEF_TITLE, ety.JTPUBQRDEF_SUBTIL,
                ety.JTPUBQRDEF_TYPE, ety.JTPUBQRDEF_SQL.Replace("'", "''"), ety.JTPUBQRDEF_DWFIELD, ety.JTPUBQRDEF_RQFIELD,
                ety.JTPUBQRDEF_WHERE, ety.JTPUBQRDEF_DBSRC, ety.JTPUBQRDEF_ORA.Replace("'", "''"));

            sql += string.Format("DELETE FROM EAOTGS WHERE F_ID='{0}';", ety.JTPUBQRDEF_ID);
            sql += "INSERT INTO EAOTGS(F_ID,F_GSBH,F_OTBH,F_TEXT,F_OTBZ,F_JS,F_ALIGN)";
            sql += string.Format("Values('{0}','01','01','{1}','H','1','L');", ety.JTPUBQRDEF_ID, ety.JTPUBQRDEF_SUBTIL);
            sql += " end; ";
            WebSvrGetData.execsql(this.ProcessID, sql, mgr);
        }
        public void Save(JTPUBQRDEFEty ety,DataTable dtwhere )
        {
            if (string.IsNullOrEmpty(ety.JTPUBQRDEF_ID))
            {
                ety.JTPUBQRDEF_ID = System.Guid.NewGuid().ToString().Replace("-", "");
                add(ety);
            }
            else
            {
                Update(ety);
            }
            updateWhereData(ety.JTPUBQRDEF_ID,dtwhere);
        }
        private void updateWhereData(string qryID, DataTable dtwhere)
        {
            string sql =string.Format( "begin delete from   JTPUBQRPARAMDEF where PARAMDEF_QRYID='{0}'; ",qryID);
            for (int i = 0; i < dtwhere.Rows.Count; i++)
            {
                if (dtwhere.Rows[i]["PARAMDEF_NAME"].ToString().Trim() != "")
                {
                    string id = System.Guid.NewGuid().ToString().Replace("-", "");

                    sql += string.Format(@"insert into JTPUBQRPARAMDEF (PARAMDEF_ID,PARAMDEF_QRYID,PARAMDEF_ORD ,
PARAMDEF_NAME,PARAMDEF_TYPE,PARAMDEF_CMP,PARAMDEF_ISUSR,PARAMDEF_CMPSTR)
values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", id, qryID, dtwhere.Rows[i]["PARAMDEF_ORD"].ToString(), dtwhere.Rows[i]["PARAMDEF_NAME"].ToString(),
     dtwhere.Rows[i]["PARAMDEF_TYPE"].ToString(), dtwhere.Rows[i]["PARAMDEF_CMP"].ToString(),
     dtwhere.Rows[i]["PARAMDEF_ISUSR"].ToString()=="是"?"1":"0", dtwhere.Rows[i]["PARAMDEF_CMPSTR"].ToString());
                }
            }
            sql += " end;";
            WebSvrGetData.execsql(this.ProcessID, sql, mgr);
        }

        public void delete(string id)
        {
            string sql= @"begin delete from EATIGSDEVQRY where F_ID='{0}';
delete from EAOTGS where F_ID='{0}';
delete from EAZBGS where F_ID='{0}'; 
delete from JTPUBQRDEF where JTPUBQRDEF_ID='{0}'; 
delete from JTPUBQRPARAMDEF where PARAMDEF_QRYID='{0}'; end;";
            sql=string.Format(sql,id);
            WebSvrGetData.execsql(this.ProcessID, sql, mgr);
        }


        public DataTable getDataTable(string sql)
        {
            DataTable dt = new DataTable();
            DataSet vsds = StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByte(this.ProcessID, sql));
            if (vsds.Tables.Count > 0)
                return vsds.Tables[0];
            else
                return dt;
        }

    }
}
