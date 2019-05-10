using ExcelAPPWeb.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Dapper;

namespace ExcelAPPWeb.Service
{
    public class TableService
    {

        public static bool IsOra = DataBaseManager.GetDBType().Equals(DBType.ORA);

        public static bool CheckTable(string tableName)
        {
            var conn = DataBaseManager.GetDbConnection();
            if (IsOra)
            {
                var count = conn.QuerySingle<int>("select count(*)  from user_all_tables where Upper(table_name) =Upper('" + tableName + "')");
                return count > 0;
            }
            else
            {
                var count = conn.QuerySingle<int>("select count(*)  from sysobjects where Upper(name) = Upper('" + tableName + "') and type = 'U'");
                return count > 0;
            }
        }


        private static string GetFieldType(Model.EACmpCateCols col)
        {
            var res = "";
            switch (col.FType.ToLower())
            {
                case "char":
                    res = "char(" + col.FLength.ToString() + ")";
                    break;
                case "varchar":
                    res = "varchar(" + col.FLength.ToString() + ")";
                    break;
                case "int":
                    res = "int";
                    break;
                case "date":
                    res = "date";
                    break;
                case "number":
                    res = "number(20," + col.FPrec.ToString() + ")";
                    break;
                default:
                    break;
            }
            return res;
        }
        public static void CreateTable(Model.EACmpCategory model)
        {
            if (CheckTable(model.Tmp.TmpTab)) return;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("create table {0}(", model.Tmp.TmpTab);
            sql.Append("ID varchar(36) not null,");
            sql.Append("FLAG char(1),");
            sql.Append("YWID varchar(36),");
            sql.Append("GSDWBH varchar(60),");
            sql.Append("CreateTime varchar(20),");
            sql.Append("CreateUser varchar(36),");
            sql.Append("ColorZT char(1),");
            foreach (var item in model.Cols)
            {
                if (item.FCode != "FLAG" && item.FCode != "ID"
                    && item.FCode != "YWID"
                    && item.FCode != "GSDWBH"
                    && item.FCode != "CreateTime"
                    && item.FCode != "CreateUser"
                    && item.FCode != "ColorZT"
                    )
                    sql.AppendFormat("{0} {1},", item.FCode, GetFieldType(item));
            }

            sql.AppendFormat(" CONSTRAINT PK_{0} PRIMARY KEY (ID))", model.Tmp.TmpTab);

            var conn = DataBaseManager.GetDbConnection();
            conn.Execute(sql.ToString());
        }
    }
}