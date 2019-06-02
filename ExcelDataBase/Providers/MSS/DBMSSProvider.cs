 
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient; 
using System.Data;

namespace ExcelDataBase.MSS
{
    public class DBMSSPovider : DBProvider
    {
        
        public DBMSSPovider()
            : this("System.Data.SqlClient")
        {
        }

        public DBMSSPovider(string providerName)
            : base(providerName)
        {
        }


        /// <summary>
        /// 设置数据库属性
        /// </summary>
        /// <param name="forDatabase">数据库实例</param>
        /// <returns>数据库属性</returns> 
        protected override DbProperties GetPropertiesFromDb(Database forDatabase)
        {
            string sql = "SELECT  SERVERPROPERTY('productversion') AS [version], SERVERPROPERTY ('productlevel') AS [level], SERVERPROPERTY ('edition') AS [edition]";
            DbProperties props=null;
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(forDatabase.ConnectionString);
            string dbname = builder.InitialCatalog;
            if (string.IsNullOrEmpty(dbname))
                dbname = builder.DataSource;
            Dictionary<string, object> dict = forDatabase.ExecuteReadDict(sql);

            props = new DbProperties(dbname, "SQL Server", dict["VERSION"].ToString(), dict["LEVEL"].ToString(),
                dict["EDITION"].ToString(), false, "+",_sqlToken, "_", "cast({0} as datetime)", "CAST(NEWID() AS VARCHAR(36))",
                "GETDATE()", "SUBSTRING", "isnull", "len");             
            return props;

        }

      
        /// <summary>
        /// 获取数据库连接串
        /// </summary>
        /// <param name="DBHost"></param>
        /// <param name="DBName"></param>
        /// <param name="DBUser"></param>
        /// <param name="DBPwd"></param>
        /// <returns></returns>
        public override string getConnString(string DBHost, string DBName, string DBUser, string DBPwd)
        {
            return string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Pooling=true;",
                DBHost, DBName, DBUser, DBPwd);
        }
        public override IDbDataParameter CreateDBDataParameter()
        {
            return new SqlParameter();
        }
   
        private const string _paramToken="@{0}";
        private const string _sqlToken = "@{0}";
        
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="dataType"></param>
        /// <param name="inout"></param>
        /// <param name="size"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public override IDbDataParameter MakeParam(string paramName, DataType dataType, ParameterDirection inout,SqlType sqltype, int size, object paramValue)
        {
            SqlParameter param = base.MakeParam(paramName, dataType, inout,sqltype, size, paramValue) as SqlParameter;
            if (SqlType.Sql == sqltype)
            {
                param.ParameterName = string.Format(_sqlToken, paramName);
            }
            else
            {
                param.ParameterName = string.Format(_paramToken, paramName);
            }
            SqlDbType dbtype = getDbType(dataType);
            //不支持的类型全部转换到Structured类型，赋值为null值,主要出现在oracle游标参数时
            if (SqlDbType.Structured == dbtype)
                param = null;
            else
                param.SqlDbType = dbtype;
            return param;
        }

        private SqlDbType getDbType(DataType dataType)
        {
            SqlDbType dbtype  ;
           
            switch(dataType){
                case DataType.Char :
                    dbtype = SqlDbType.Char;
                    break;
                case DataType.VarChar:
                    dbtype = SqlDbType.VarChar;
                    break;
                case DataType.Int:
                    dbtype = SqlDbType.Int;
                    break;
                case DataType.Decimal:
                    dbtype = SqlDbType.Decimal;
                    break;
                case DataType.Blob:
                    dbtype = SqlDbType.Image;
                    break;
                case DataType.DateTime:
                    dbtype = SqlDbType.DateTime;
                    break;
                case DataType.Clob:
                    dbtype = SqlDbType.Text;
                    break;
                default:
                    dbtype = SqlDbType.Structured;
                    break;
            }
            return dbtype;
        }

        public override string getDataType(string datatype)
        {
            string vsre = "";
            switch (datatype.ToLower())
            {
                case "char":
                case "varchar":
                case "int":
                case "datetime": 
                case "decimal": 
                    vsre = datatype;
                    break;
                case "blob":
                    vsre = "varbinary(max)";
                    break;
                case "clob":
                    vsre = "varchar(max)";
                    break; 
            }
            return vsre;
        }
        
    }
}
