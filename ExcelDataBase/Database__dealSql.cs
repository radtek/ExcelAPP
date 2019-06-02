using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
 
namespace ExcelDataBase
{
    /// <summary>
    /// sql处理
    /// </summary>
    public partial class Database
    {

        private void dealCMD(IDbCommand cmd)
        {
            if (CommandType.Text == cmd.CommandType)
            {
                cmd.CommandText = dealSQL(cmd, cmd.CommandText);
            }
            else if (CommandType.StoredProcedure == cmd.CommandType)
            {
                cmd.CommandText = dealProc(cmd, cmd.CommandText);
            }
        }
        private string dealSQL(IDbCommand cmd, string sql)
        {
            //if(DataBaseCache.EnabledSession)
            //sql =GSYS.Session.GSIDPSession.ReplaceSession(sql); //todo
            string vsre = sql;
            string dbuserid = getDbUserFormCmd(cmd);
            if (cmd.GetType().Name == "SqlCommand" && cmd.CommandText.IndexOf("JTPUBQRDEF") == -1)
            {
                vsre = vsre.Replace("||", "+");
            }
            else if (cmd.GetType().Name == "OracleCommand")
            {

                vsre = vsre.Replace("GETDATE()", "sysdate");
            }
            vsre = dealDateTime(cmd, vsre);
            //cmd.GetType() //if(System.Type.
            vsre = vsre.Replace("@DBUSER$", dbuserid);

            DealCmdParameters(cmd);
            return vsre;
        }
        private void DealCmdParameters(IDbCommand cmd)
        {
            string dbuserid = getDbUserFormCmd(cmd);
            foreach (IDbDataParameter param in cmd.Parameters)
            {
                if (param.Value != null && param.Value.GetType() == Type.GetType("System.String"))
                {

                    if (cmd.GetType().Name == "SqlCommand")
                    {
                        /*此处不能替换||为+，否则增加保存时会有问题。如果是通用的，需要在存储过程时在sqlserver中注意书写。*/
                        if (cmd.CommandText.ToUpper() == "GSIDPPUB_PAGEPROC" || cmd.CommandText.ToUpper() == "GSIDPPUB_PAGETOPKEYPROC")
                        {
                            param.Value = param.Value.ToString().Replace("||", "+");
                            //param.Value = GSYS.Session.GSIDPSession.ReplaceSession(param.Value.ToString()); //todo
                        }
                    }
                    else if (cmd.GetType().Name == "OracleCommand")
                    {

                        if (cmd.CommandText.ToUpper() == "GSIDPPUB.PAGEPROC" || cmd.CommandText.ToUpper() == "GSIDPPUB.PAGETOPKEYPROC")
                        {
                            //param.Value = GSYS.Session.GSIDPSession.ReplaceSession(param.Value.ToString()); //todo
                        }
                        cmd.CommandText = cmd.CommandText.Replace("@" + param.ParameterName, ":" + param.ParameterName);
                        param.Value = param.Value.ToString().Replace("GETDATE()", "sysdate");
                    }
                    param.Value = dealDateTime(cmd, param.Value.ToString());
                    param.Value = param.Value.ToString().Replace("@DBUSER$", dbuserid);
                    param.Size = param.Value.ToString().Length + 100;
                }
            }
        }

        private string dealDateTime(IDbCommand cmd, string sql)
        {

            string vsre = "";
            while (sql.IndexOf("@`") != -1)
            {
                vsre += sql.Substring(0, sql.IndexOf("@`"));
                string dateval = sql.Substring(sql.IndexOf("@`") + 2, sql.IndexOf("`@") - sql.IndexOf("@`") - 2);
                // sql.substr(sql.indexOf("@`") + 2, sql.indexOf("`@") - sql.indexOf("@`") - 2);
                sql = sql.Substring(sql.IndexOf("`@") + 2);
                string val = dateval;
                string fmt = "yyyy-MM-dd hh24:mi:ss";
                if (dateval.IndexOf("|") != -1)
                {
                    string[] dtarr = dateval.Split('|');
                    val = dtarr[0];
                    if (dtarr[1].Trim() != "") fmt = dtarr[1];
                }
                if (cmd.GetType().Name == "SqlCommand")
                {
                    vsre += " '" + val + "' ";
                }
                else if (cmd.GetType().Name == "OracleCommand")
                {
                    //select to_date('2013-01-01 01:01:01','yyyy-MM-dd hh24:mi:ss') from dual
                    vsre += " to_date('" + val + "','" + fmt + "') ";
                }
                //vsre = "";

            }
            vsre += sql;
            return vsre;
        }


        private string dealProc(IDbCommand cmd, string procName)
        {
            string vsre = procName;
            // cmd.CommandText =  GSYS.Session.GSIDPSession.ReplaceSession(cmd.CommandText);  //todo
            if (cmd.GetType().Name == "SqlCommand" && cmd.CommandText.IndexOf("JTPUBQRDEF") == -1)
            {
                cmd.CommandText = cmd.CommandText.Replace("||", "+");
            }
            DealCmdParameters(cmd);
            string dbuserid = getDbUserFormCmd(cmd);
            vsre = vsre.Replace("@DBUSER$", dbuserid);
            return vsre;
        }
        private string getDbUserFormCmd(IDbCommand cmd)
        {
            string vsre = "";
            vsre = cmd.Connection.ConnectionString;
            vsre = vsre.Substring(vsre.ToUpper().IndexOf("USER ID=") + 8);
            vsre = vsre.Substring(0, vsre.IndexOf(';'));
            vsre = vsre.Trim().ToUpper();
            if (vsre == "SA")
            {
                vsre = "dbo";
            }
            return vsre;
        }



    }
}
