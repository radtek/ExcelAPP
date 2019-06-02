
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ExcelDataBase
{
    /// <summary>
    /// 提供者实例配置项
    /// </summary>
    public class DBProviderConfigElement : ConfigurationElement
    {

        #region 私有变量

        private DBProvider _Provider = null;//存储目前的提供者

        #endregion

        //
        // 公共属性
        //

        #region public string ProviderName {get;}
        /// <summary>
        /// 提供者名称
        /// </summary>
        [ConfigurationProperty("provider", IsRequired = true, IsKey = true)]
        public string ProviderName
        {
            get { return (string)this["provider"]; }
            set { this["provider"] = value; }
        }
        #endregion

        #region public string ImplementationTypeName {get; set;}
        /// <summary>
        /// 实现类型
        /// </summary>
        [ConfigurationProperty("implementation-type", IsRequired = true, IsKey = false)]
        public string ImplementationTypeName
        {
            get { return (string)this["implementation-type"]; }
            set { this["implementation-type"] = value; _Provider = null; }
        }
        #endregion

        #region public DBProviderImplementation Implementation {get;}
        /// <summary>
        /// 获取实例
        /// </summary>
        public DBProvider Provider
        {
            get
            {
                if (null == _Provider)
                    _Provider = CreateImplementation(this.ImplementationTypeName);
                return _Provider;
            }
        }
        #endregion

        //
        // .ctors
        //

        #region public DBProviderConfigElement()

        /// <summary>
        /// 实现构造器
        /// </summary>
        public DBProviderConfigElement() { }

        #endregion

        #region internal DBProviderConfigElement(string providername, string typename, DBProviderImplementation implementation)

        /// <summary>
        /// 内部构造函数
        /// </summary>
        /// <param name="providername"></param>
        /// <param name="typename"></param>
        /// <param name="implementation"></param>
        internal DBProviderConfigElement(string providername, string typename, DBProvider provider)
            : this()
        {
            this.ProviderName = providername;
            this.ImplementationTypeName = typename;
            this._Provider = provider;
        }

        #endregion

        //
        // methods
        //

        #region protected virtual DBProvider CreateImplementation(string typename)

        /// <summary>
        /// 创建一个实例
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        protected virtual DBProvider CreateImplementation(string typename)
        {
            if (string.IsNullOrEmpty(typename))
                throw new ArgumentNullException(DbError.ProviderNameNull);

            Type imptype = Type.GetType(typename, false);
            if (null == imptype)
            {
                throw new Exception(string.Format(DbError.ProviderNameError, typename));
            }

            DBProvider factory;
            try
            {
                object instance = Activator.CreateInstance(imptype);
                factory = (DBProvider)instance;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(DbError.ProviderFactoryError, typename));  
            }

            return factory;
        }

        #endregion
    }


    #region  public class DBProviderConfigElementCollection : ConfigurationElementCollection

    /// <summary>
    /// 定义配置项集合
    /// </summary>
   
    public class DBProviderConfigElementCollection : ConfigurationElementCollection
    {

        /// <summary>
        /// 定义
        /// </summary>
        public DBProviderConfigElementCollection()
            : base(System.StringComparer.InvariantCultureIgnoreCase)
        {
            this.InitDefaultValues();
        }

        /// <summary>
        /// 重载实现实例
        /// </summary>
        /// <returns></returns>
        protected override System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new DBProviderConfigElement();
        }

        /// <summary>
        /// 获取配置项的主键
        /// </summary>
        /// <param name="element"> </param>
        /// <returns></returns>
        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            string name = ((DBProviderConfigElement)element).ProviderName;
            if (string.IsNullOrEmpty(name))
            { 
                throw new Exception(DbError.ProviderNameNull);        
            }

            return name;
        }

        /// <summary>
        /// 默认设置提供者
        /// </summary>
        protected virtual void InitDefaultValues()
        {
            //System.Data.SqlClient
            DBProvider provider = new MSS.DBMSSPovider();
            this.Add(provider);
            //System.Data.OracleClient
            provider = new ORA.DBOraProvider();
            this.Add(provider);

            //imp = new MySqlClient.DBMySqlImplementation();
            //this.Add(imp);

            //imp = new OleDb.DBOleDbImplementation();
            //this.Add(imp);
        }

        //
        // 集合实现
        //

        /// <summary>
        /// add方法
        /// </summary>
        /// <param name="imp"> 提供者实例</param>
        public void Add(DBProvider provider)
        {
            string name = provider.ProviderName;
            string typename = provider.GetType().AssemblyQualifiedName;
            DBProviderConfigElement ele = new DBProviderConfigElement(
                name, typename, provider);
            this.Add(ele);
        }

        /// <summary>
        /// add 方法
        /// </summary>
        /// <param name="element"></param>
        internal void Add(DBProviderConfigElement element)
        {
            this.BaseAdd(element);
        }

        /// <summary>
        /// 根据名称获取
        /// </summary>
        /// <param name="providerName">名称</param>
        /// <returns>返回匹配的实例</returns>
        internal DBProviderConfigElement Get(string providerName)
        {
            return this.BaseGet(providerName) as DBProviderConfigElement;
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="providerName">实例名</param>
        /// <returns></returns>
        internal bool Contains(string providerName)
        {
            return this.Get(providerName) != null;
        }

        /// <summary>
        /// 删除实例
        /// </summary>
        /// <param name="providerName"></param>
        internal void Remove(string providerName)
        {
            this.BaseRemove(providerName);
        }

    }

    #endregion
}
