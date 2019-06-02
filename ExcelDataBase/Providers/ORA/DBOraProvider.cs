 
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OracleClient; 
using System.Data;

namespace ExcelDataBase.ORA
{
    public class DBOraProvider : DBProvider
    {
        
        public DBOraProvider()
            : this("System.Data.OracleClient")
        {
        }

        public DBOraProvider(string providerName)
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
            string sql = "Select  PRODUCT   ,VERSION  ,STATUS    FROM  Product_component_version where substr(product,1,6)='Oracle' ";
            //"SELECT  SERVERPROPERTY('productversion') AS [version], SERVERPROPERTY ('productlevel') AS [level], SERVERPROPERTY ('edition') AS [edition]";
            DbProperties props=null;
            System.Data.OracleClient.OracleConnectionStringBuilder builder = new System.Data.OracleClient.OracleConnectionStringBuilder(forDatabase.ConnectionString);
            string dbname = builder.UserID; 
            Dictionary<string, object> dict = forDatabase.ExecuteReadDict(sql);

            props = new DbProperties(dbname, "Oracle",  dict["VERSION"].ToString(), dict["STATUS"].ToString(),
                dict["PRODUCT"].ToString(), true, "||", _sqlToken, ".", "TO_DATE('{0}', 'YYYY-MM-DD HH24:MI:SS')", "SYS_GUID()",
                "sysdate", "SUBSTR", "nvl", "length");             
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
            return string.Format("Data Source={0};User Id={1};Password={2};  ",
                DBHost,DBUser, DBPwd);
        }
        public override IDbDataParameter CreateDBDataParameter()
        {
            return new OracleParameter();
        }
   
        private const string _paramToken="{0}";
        private const string _sqlToken = ":{0}";
        
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
            OracleParameter param = base.MakeParam(paramName, dataType, inout, sqltype, size, paramValue) as OracleParameter;
            if (SqlType.Sql == sqltype)
            {
                param.ParameterName = string.Format(_sqlToken, paramName);
            }
            else
            {
                param.ParameterName = string.Format(_paramToken, paramName);
            }
            OracleType dbtype = getDbType(dataType);
            //不支持的类型全部转换到BFile类型，赋值为null值,主要出现在oracle游标参数时
            if (OracleType.BFile == dbtype)
                param = null;
            else
                param.OracleType = dbtype;
            return param;
        }

        private OracleType getDbType(DataType dataType)
        {
            OracleType dbtype;
           
            switch(dataType){
                case DataType.Char :
                    dbtype = OracleType.Char;
                    break;
                case DataType.VarChar:
                    dbtype = OracleType.VarChar;
                    break;
                case DataType.Int:
                    dbtype = OracleType.Int32;
                    break;
                case DataType.Decimal:
                    dbtype = OracleType.Number;
                    break;
                case DataType.Blob:
                    dbtype = OracleType.Blob;
                    break;
                case DataType.DateTime:
                    dbtype = OracleType.DateTime;
                    break;
                case DataType.Clob:
                    dbtype = OracleType.Clob;
                    break;
                case DataType.Cursor:
                    dbtype = OracleType.Cursor;
                    break;
                default:
                    dbtype = OracleType.BFile;
                    break;
            }
            return dbtype;
        }
        

        public override string getDataType(string datatype){
            string vsre="";
            switch (datatype.ToLower())
            {
                case "char":
                case "varchar":
                case "int":
                case "blob":
                case "clob":
                    vsre=datatype;
                    break;
                case "datetime":
                    vsre = "timestamp";
                    break;
                case "decimal":
                    vsre = "number";
                    break;
            }
            return vsre;
        }
    

        
    }
}
