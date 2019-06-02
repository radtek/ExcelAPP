 
using System;
using System.Collections.Generic;
using System.Text; 
using System.Data.Common;
using System.Data;
 
namespace ExcelDataBase
{
    /// <summary>
    /// 数据库提供者
    /// </summary> 
    public abstract class DBProvider
    {
        //
        // 静态
        //

        #region static 变量

        private static object _lock; //threadsafe lock
        
        #endregion

        #region static 实现

        /// <summary>
        /// 静态 初始化DBProvider
        /// </summary>
        static DBProvider()
        {
            _lock = new object();
        }

        #endregion

        #region private static DBProviderConfigSection ConfiguredProviders {get;}

        /// <summary>
        /// 获取配置项
        /// </summary>
        private static DBProviderConfigSection ConfiguredProviders
        {
            get
            {
                return DBConfigurationSection.GetSection().ProviderSection;
            }
        }

        #endregion

        #region public static void RegisterFactory(string providername, DBFactory factory) + 1 overload

        /// <summary>
        /// 注册实例
        /// </summary>
        /// <param name="imp"></param>
        public static void RegisterProvider(DBProvider  provider)
        {
            if (null == provider){ 
                throw new Exception(DbError.ProviderFactoryNull);  
            }

            RegisterProvider(provider.ProviderName, provider);
        }

        /// <summary>
        /// 注册实例
        /// </summary>
        /// <param name="providername">实例类型</param>
        /// <param name="imp">提供者</param>
        public static void RegisterProvider(string providername, DBProvider provider)
        {
            if (null == provider)
                throw new ArgumentNullException("factory");
            
            if (string.IsNullOrEmpty(providername))
                throw new ArgumentNullException("providername");

            lock (_lock)
            {
                if (ConfiguredProviders.Contains(providername))
                    ConfiguredProviders.Remove(providername);

                ConfiguredProviders.Add(providername, provider);
                
            }
        }

        #endregion

        #region public static bool IsFactoryRegistered(string providerName)

        /// <summary>
        ///  是否注册
        /// </summary>
        /// <param name="providerName">实例名</param>
        /// <returns>True or false</returns>
        public static bool IsFactoryRegistered(string providerName)
        {
            bool contains;

            lock (_lock)
            {
                contains = ConfiguredProviders.Contains(providerName);
            }
            return contains;
        }

        #endregion

        #region public static DBProvider  GetProvider(string providerName)

        /// <summary>
        /// 根据数据库类型名获取提供者
        /// </summary>
        /// <param name="providerName">类型名</param>
        /// <returns>提供者</returns> 
        public static DBProvider  GetProvider(string providerName)
        {
            DBProvider imp;

            lock (_lock)
            {
                imp = ConfiguredProviders.Get(providerName);
            }

            if(null == imp){ 
                throw new Exception(string.Format(DbError.ProviderNameError, providerName));  
            }

            return imp;
        }

        #endregion

        //
        // 实现实例
        //


        #region 变量

        private string _providername;
        private Dictionary<string, DbProperties> _knownProps = new Dictionary<string, DbProperties>();

        
        #endregion

        //
        //公共属性
        //

        #region public string ProviderName {get;}

        /// <summary>
        /// 实例名
        /// </summary>
        public string ProviderName
        {
            get { return this._providername; }
        }

        #endregion

        //
        // .实现
        //

        #region  protected DBProvider(string providername)

        /// <summary>
        ///构造器
        /// </summary>
        /// <param name="providername"></param>
        protected DBProvider(string providername)
        {
            this._providername = providername;
        }

        #endregion 

        //
        // 方法
        //

        #region  public virtual Database CreateDatabase(string connection, string providerName, DbProviderFactory factory)

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="connection">连接字符串</param>
        /// <param name="providerName">提供者类型</param>
        /// <param name="factory"> .NET 提供的数据工厂类 DbProviderFactory</param>
        /// <returns></returns>
        public virtual Database CreateDatabase(string connection, string providerName, DbProviderFactory factory)
        {
            if (string.IsNullOrEmpty(connection))
                throw new ArgumentNullException("连接字符串");
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("提供类型");
            if (null == factory)
                throw new ArgumentNullException("数据工厂");

            return new Database(connection, providerName, factory);
        }

        #endregion



        #region  public virtual DbProperties CreateDatabaseProperties(Database forDatabase)

        /// <summary>
        /// 获取数据库属性
        /// </summary>
        /// <param name="forDatabase"></param>
        /// <returns></returns>
        public virtual DbProperties CreateDatabaseProperties(Database forDatabase)
        {
            if (null == forDatabase)
                throw new ArgumentNullException("数据库对象");
            if (string.IsNullOrEmpty(forDatabase.ConnectionString))
                throw new ArgumentNullException("数据库连接字符串");

            DbProperties properties;
 
            if (_knownProps.TryGetValue(forDatabase.ProviderName + "|" + forDatabase.ConnectionString, out properties) == false)
            {
                properties = this.GetPropertiesFromDb(forDatabase);
                _knownProps[forDatabase.ProviderName + "|" + forDatabase.ConnectionString] = properties;
            }
            
            return properties;
        }

        #endregion

        //
        // 支持属性
        //

      

        /// <summary>
        /// 键值对字符获取值数据
        /// </summary>
        /// <param name="connection">连接字符串</param>
        /// <param name="sectionSeparator">属性分隔符</param>
        /// <param name="keyvalueSeparator">键值对分割符</param>
        /// <returns>属性值</returns>
        protected virtual System.Collections.Specialized.NameValueCollection SplitConnectionString(string connection, char sectionSeparator, char keyvalueSeparator)
        {
            if (string.IsNullOrEmpty(connection))
                throw new ArgumentNullException("connection");

            string[] all = connection.Split(sectionSeparator);
            System.Collections.Specialized.NameValueCollection nvCol = new System.Collections.Specialized.NameValueCollection(all.Length, StringComparer.OrdinalIgnoreCase);
            foreach (string s in all)
            {
                string[] kv = s.Split(keyvalueSeparator);

                if (kv.Length == 2)
                    nvCol.Add(kv[0].Trim(), kv[1].Trim());

                else if (kv.Length == 1)
                    nvCol.Add(kv[0].Trim(), kv[0].Trim());

                else //如果有重复的，增加到一个中
                {
                    nvCol.Add(kv[0].Trim(), string.Join(keyvalueSeparator.ToString(), kv, 1, kv.Length - 1));
                }
            }
            return nvCol;
        }

 
        /// <summary>
        /// 从数据库连接串中获取属性值
        /// </summary>
        /// <param name="forDatabase">数据库</param>
        /// <param name="sectionSeparator">属性分隔符</param>
        /// <param name="keyvalueSeparator">键值对分割符</param>
        /// <param name="datasourcekey">属性键的值</param>
        /// <returns></returns>
        protected virtual string GetDataSourceNameFromConnection(Database forDatabase, char sectionSeparator, char keyValueSeparator, string datasourcekey)
        {
            if (null == forDatabase)
                throw new ArgumentNullException("数据库为空");
            if (string.IsNullOrEmpty(forDatabase.ConnectionString))
                throw new ArgumentNullException("数据库连接串为空");

            string dbname = "";
            try
            {
                string con = forDatabase.ConnectionString;
                System.Collections.Specialized.NameValueCollection nvCol;
                nvCol = this.SplitConnectionString(con, sectionSeparator, keyValueSeparator);
                dbname = nvCol[datasourcekey];
            }
            catch (Exception)
            {  
            }

            return dbname;
        }

        public IDbDataParameter MakeInParam(string paramName, object paramValue,SqlType sqltype)
        {
            return MakeParam(paramName, DataType.Default, ParameterDirection.Input, sqltype, - 1, paramValue);
        }
        public IDbDataParameter MakeInParam(string paramName, DataType dataType, int size, object paramValue, SqlType sqltype)
        {
            return MakeParam(paramName, dataType, ParameterDirection.Input, sqltype,size, paramValue);
        }
        public IDbDataParameter MakeOutParam(string paramName, DataType dataType,SqlType sqltype)
        {
            return MakeParam(paramName, dataType, ParameterDirection.Output,sqltype, -1, null);
        }
        public IDbDataParameter MakeOutParam(string paramName, DataType dataType, int size, SqlType sqltype)
        {
            return MakeParam(paramName, dataType, ParameterDirection.Output,sqltype, size, null);
        }
        public IDbDataParameter MakeInOutParam(string paramName, object paramValue, SqlType sqltype)
        {
            return MakeParam(paramName, DataType.Default, ParameterDirection.InputOutput,sqltype, -1, paramValue);
        }
        public IDbDataParameter MakeInOutParam(string paramName, DataType dataType, int size, object paramValue, SqlType sqltype)
        {
            return MakeParam(paramName, dataType, ParameterDirection.InputOutput,sqltype, size, paramValue);
        }

        //
        // 虚方法，子类必须实现
        //

        
        /// <summary>
        /// 设置数据库属性，虚方法，集成类必须实现
        /// </summary>
        /// <param name="forDatabase">数据库实例</param>
        /// <returns>数据库属性</returns>
        protected abstract DbProperties GetPropertiesFromDb(Database forDatabase);
        /// <summary>
        /// 获取连接串
        /// </summary>
        /// <param name="DBHost"></param>
        /// <param name="DBName"></param>
        /// <param name="DBUser"></param>
        /// <param name="DBPwd"></param>
        /// <returns></returns>
        public abstract string getConnString(string DBHost,string DBName,string DBUser,string DBPwd);
        public abstract IDbDataParameter CreateDBDataParameter();    
        public abstract string getDataType(string datatype);
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="dataType"></param>
        /// <param name="inout"></param>
        /// <param name="size"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        //public abstract IDbDataParameter MakeParam(string paramName, DataType dataType,ParameterDirection inout, int size, object paramValue);


        public virtual IDbDataParameter MakeParam(string paramName, DataType dataType, ParameterDirection inout,SqlType sqltype, int size, object paramValue)
        {
            IDbDataParameter param = this.CreateDBDataParameter();
            param.ParameterName = paramName;
            if (size > 0) param.Size = size;
            param.Direction = inout;
            if (!(inout == ParameterDirection.Output && paramValue == null))
                param.Value = paramValue;
            return param;
        }
        
    }
}
