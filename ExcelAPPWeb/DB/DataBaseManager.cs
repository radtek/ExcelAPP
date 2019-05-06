using NPoco;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ExcelAPPWeb.DB
{

    public enum DBType
    {
        ORA = 1,
        MSS = 0
    }
    public class DataBaseManager
    {

        public static Database GetDB()
        {
            Database Db = new Database("DataPlatformDB");
            return Db;
        }
        public static string GetToken()
        {

            var token = "@";
            string provider = ConfigurationManager.ConnectionStrings["DataPlatformDB"].ProviderName;
            switch (provider)
            {
                case "Oracle.ManagedDataAccess.Client":
                    token = ":";
                    break;
                default:
                    break;
            }
            return token;
        }

        public static DBType GetDBType()
        {
            DBType res = DBType.MSS;

            string provider = ConfigurationManager.ConnectionStrings["DataPlatformDB"].ProviderName;
            switch (provider)
            {
                case "Oracle.ManagedDataAccess.Client":
                    res = DBType.ORA;
                    break;
                default:
                    break;
            }
            return res;
        }

        public static IDbConnection GetDbConnection()
        {

            var connnectionStringName = ConfigurationManager.ConnectionStrings["DataPlatformDB"].ConnectionString;
            string provider = ConfigurationManager.ConnectionStrings["DataPlatformDB"].ProviderName;
            string connectionString = ConfigurationManager.ConnectionStrings["DataPlatformDB"].ConnectionString;
            IDbConnection connection;
            switch (provider)
            {
                case "MySql.Data.SqlClient":
                case "System.Data.SqlClient":
                    connection = new SqlConnection(connectionString);
                    break;
                case "Oracle.ManagedDataAccess.Client":
                    connection = new OracleConnection(connectionString);
                    break;
                default:
                    connection = new SqlConnection(connectionString);
                    break;
            }
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            return connection;
        }


    }
}