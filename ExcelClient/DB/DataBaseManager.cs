using NPoco;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ExcelClient
{

    public class DBContext
    {
        public string DBType { get; set; }


        public IDbConnection Conn { get; set; }
    }
    public class DataBaseManager
    {

        public static Database GetDB()
        {
            Database Db = new Database("DataPlatformDB");
            return Db;
        }



        public static DBContext BuildContext()
        {
            DBContext context = new DBContext();
            var DataScource = ConfigurationManager.AppSettings["DataScource"].ToString();
            var DbType = ConfigurationManager.AppSettings["DbType"].ToString();
            var Catalog = ConfigurationManager.AppSettings["Catalog"].ToString();
            var UserId = ConfigurationManager.AppSettings["UserId"].ToString();
            var Password = ConfigurationManager.AppSettings["Password"].ToString();
            var ConnectString = "";
            var ProviderName = "";
            context.DBType = DbType;

            switch (DbType)
            {
                case "MSS":
                    ConnectString = $"Data Source={DataScource};Initial Catalog={Catalog};Persist Security Info=True;User ID={UserId};Password={Password}";
                    ProviderName = "System.Data.SqlClient";
                    break;
                case "ORA":
                    ConnectString = $"Data Source={DataScource};User ID={UserId};Password={Password}";
                    ProviderName = "Oracle.ManagedDataAccess.Client";
                    break;
                default:
                    break;
            }
            context.Conn = GetDbConnection(ConnectString, ProviderName);
            return context;
        }


        public DBContext GetDBContext()
        {


            return new DBContext();

        }

        public static IDbConnection GetDbConnection(string connectionString, string provider)
        {
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