 

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;
 

namespace ExcelDataBase
{
    /// <summary>
    /// Database�������
    /// </summary>
    public partial class Database
    {

        #region param 

        private DbProviderFactory _factory;
        private string _connstr;
        private string _dbname;
        private string _providername;
        private DBProvider _provider=null;
        private DbProperties _properties;
        

        #endregion

        //
        // ����
        //

        #region public string Name {get; set;}

        /// <summary>
        /// ��������
        /// </summary>        
        public string Name
        {
            get { return (null == _dbname) ? "" : this._dbname; }
            set { _dbname = value; }
        }

        #endregion

        #region public string ConnectionString {get;}

        /// <summary>
        /// ���ݿ������ַ���
        /// </summary>
        public string ConnectionString
        {
            get { return _connstr; }
        }

        #endregion

        #region protected DbProviderFactory Factory {get;}

        /// <summary>
        /// �ṩ�߹���
        /// </summary>
        protected DbProviderFactory Factory
        {
            get { return this._factory; }
        }

        #endregion

        #region public DBProvider Provider
        /// <summary>
        /// �������ṩ��
        /// </summary>
        public DBProvider Provider
        {
            get {
                if (null == _provider)
                    _provider = DBProvider.GetProvider(this.ProviderName);
                return _provider;
            }
        }
        #endregion

        #region public string ProviderName {get;}

        /// <summary>
        /// ���ݿ�ִ�����ṩ����
        /// </summary>
        public string ProviderName
        {
            get { return _providername; }
        }

        #endregion             

        #region public DbProperties GetProperties()

        /// <summary>
        /// �������ݿ�����
        /// </summary>
        /// <returns></returns>
        public DbProperties GetProperties()
        {
            if (_properties == null)
                _properties = this.CreateProperties();
            return _properties;

        }

        #endregion


        #region  public      getProviderName(string strDBType)
        /// <summary>
        /// �������ݿ����ͻ�ȡ�ṩ������
        /// </summary>
        /// <param name="strDBType"></param>
        /// <returns></returns>
        public DataBaseType DBType
        {
            get{
                DataBaseType _dbtype = DataBaseType.MSS;
            switch (_providername)
            {
                case "System.Data.SqlClient":
                    _dbtype = DataBaseType.MSS;
                    break;
                case "System.Data.OracleClient":
                    _dbtype = DataBaseType.ORA;
                    break;
                case "MYSQL":
                    break;
                case "OLEDB":
                    break;
                case "SQLLITE":
                    break;
            }
            return _dbtype;
        }
        }
        #endregion

        //
        // �����ľ�̬����
        //

        #region public static Database Create(string connectionStringName)

        /// <summary>
        /// ͨ��connectionStrings���ô���һ��databaseʵ��
        /// </summary>
        /// <param name="connectionStringName">�����ļ���connectionStrings  Name����</param>
        /// <returns>A new DBDatabase</returns>
        public static Database Create(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
                throw new ArgumentNullException("connectionStringName", string.Format(DbError.ConnstrSetNameNull, connectionStringName));

            System.Configuration.ConnectionStringSettings settings = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName];

            if (settings == null)
                throw new ArgumentNullException("connectionStringName", string.Format(DbError.ConnstrSetValueNull, connectionStringName));

            string con = settings.ConnectionString;
            if (con.IndexOf(";") == -1)
            {
                //con = Encrypt.DecrypRijndael(con);
            }
            string prov = settings.ProviderName ;

            return Create(connectionStringName, con, prov);
        }

        #endregion

        #region public static Database Create(string connectionString, string strDBtype)

        /// <summary>
        ///  ͨ�����Ӵ����ṩ�����ƣ�����databaseʵ��
        /// </summary>
        /// <param name="connectionString">���ݿ����Ӵ�</param>
        /// <param name="providername">���ݿ��ṩ������</param>
        /// <returns>databaseʵ��</returns>
        public static Database Create(string connectionString, string strDBtype)
        {
            return Create(null, connectionString, strDBtype);
        }

        #endregion

        #region public static Database Create(string name, string connectionString, string strDBtype)

        /// <summary>
        /// ͨ��name �����ݿ����Ӵ����ṩ�����ƣ� ����databaseʵ��
        /// </summary>
        /// <param name="name">DBDatabase���ƣ�Ψһ������</param>
        /// <param name="connectionString">���ݿ����Ӵ�</param>
        /// <param name="providername">���ݿ��ṩ������</param>
        /// <returns>databaseʵ��</returns>
        public static Database Create(string name, string connectionString, string strDBtype)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString", String.Format(DbError.ConnstrNull));

            if (string.IsNullOrEmpty(strDBtype))
                throw new ArgumentNullException("providername", String.Format(DbError.ProviderNameNull));
            string providername = getProviderName(strDBtype);
            System.Data.Common.DbProviderFactory fact = System.Data.Common.DbProviderFactories.GetFactory(providername);

            if (null == fact)
                throw new ArgumentNullException("providername", String.Format(DbError.InvalidProviderNameForConn, providername));

            DBProvider datafact = DBProvider.GetProvider(providername);

            if (null == datafact)
                throw new ArgumentNullException("providername", String.Format(DbError.InvalidProviderNameForDBFactory, providername));

            Database db = datafact.CreateDatabase(connectionString, providername, fact);

            //��������
            db.Name = name;

            //����ʵ��
            return db;
        }

        #endregion

        #region public static Database Create(string name, string connectionString, string DBType ,string dbhost,string dbname,string dbuser,string dbpwd)

        /// <summary>
        /// ͨ��name �����ݿ����Ӵ����ṩ�����ƣ� ����databaseʵ��
        /// </summary>
        /// <param name="name">DBDatabase���ƣ�Ψһ������</param>
        /// <param name="connectionString">���ݿ����Ӵ�</param>
        /// <param name="providername">���ݿ��ṩ������</param>
        /// <returns>databaseʵ��</returns>
        public static Database Create(string name, string strDBType, string dbhost, string dbname, string dbuser, string dbpwd)
        {
            if (string.IsNullOrEmpty(strDBType))
                throw new ArgumentNullException("DBType", String.Format(DbError.ConnstrNull));
            string providername = getProviderName(strDBType);
            if (string.IsNullOrEmpty(strDBType))
                throw new ArgumentNullException("providername", String.Format(DbError.ProviderNameNull));

            System.Data.Common.DbProviderFactory fact = System.Data.Common.DbProviderFactories.GetFactory(providername);

            if (null == fact)
                throw new ArgumentNullException("providername", String.Format(DbError.InvalidProviderNameForConn, providername));

            DBProvider datafact = DBProvider.GetProvider(providername);
            dbpwd = DataBaseCache.DealDecodePWD(dbpwd);
            string connstring = datafact.getConnString(dbhost, dbname, dbuser, dbpwd);
            if (null == datafact)
                throw new ArgumentNullException("providername", String.Format(DbError.InvalidProviderNameForDBFactory, providername));

            Database db = datafact.CreateDatabase(connstring, providername, fact);

            //��������
            db.Name = name;

            //����ʵ��
            return db;
        }

        #endregion
       
        //
        // ����������ʵ�� conn
        //

        #region public virtual DbConnection CreateConnection()

        /// <summary>
        /// �������ݿ�����
        /// </summary>
        /// <returns>A new DbConnection</returns>
        public virtual DbConnection CreateConnection()
        {
            DbConnection con = this.Factory.CreateConnection();
            con.ConnectionString = this.ConnectionString;
            return con;
        }
        #endregion       

        //
        // �ܱ���������ʵ�ֲ���
        //

        #region protected internal Database(string connection, string providerName, DbProviderFactory provider)

        /// <summary>
        /// �ڲ� ���ݿ������ʵ��
        /// </summary>
        /// <param name="connection">���Ӵ�</param>
        /// <param name="providerName">���ݷ��ʶ����ṩ��</param>
        /// <param name="factory">���ݹ���</param>
        protected internal Database(string connstr, string providerName, DbProviderFactory factory)
        {
            if (null == factory)
                throw new ArgumentNullException(DbError.ProviderFactoryNull);
            if (string.IsNullOrEmpty(connstr))
                throw new ArgumentNullException(DbError.ConnstrNull);
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException(DbError.ProviderNameNull);

            this._connstr = connstr;
            this._factory = factory;
            this._providername = providerName;
        }

        #endregion

        #region protected virtual void HandleExecutionError(Exception ex)
        /// <summary>
        /// �������
        /// </summary>
        /// <param name="ex">������</param>
        /// <remarks>�Ƿ����´������ݿ���ʳ���</remarks>       
        protected virtual void HandleExecutionError(Exception ex)
        {
            if (DBConfigurationSection.GetSection().OverrideExceptions)
            {
                throw new DataException(DbError.DBErrorCaption, ex);
            }
            else
            {
                throw ex;
            }
        }
        #endregion
        
        #region protected virtual DBDatabaseProperties CreateProperties()
        /// <summary>
        /// ��ȡ���ݿ���������
        /// </summary>
        /// <returns></returns>
        protected virtual DbProperties CreateProperties()
        {
            DBProvider factory = DBProvider.GetProvider(this.ProviderName);
            DbProperties properties = factory.CreateDatabaseProperties(this);
            return properties;
        }

        #endregion

        #region  protected static string getProviderName(string strDBType)
        /// <summary>
        /// �������ݿ����ͻ�ȡ�ṩ������
        /// </summary>
        /// <param name="strDBType"></param>
        /// <returns></returns>
        protected static string getProviderName(string strDBType)
        {
            string vsre = "";
            switch (strDBType.ToUpper())
            {
                case "MSS":
                    vsre = "System.Data.SqlClient";
                    break;
                case "ORA":
                    vsre = "System.Data.OracleClient";
                    break;
                case "MYSQL":
                    break;
                case "OLEDB":
                    break;
                case "SQLLITE":
                    break;
            }
            return vsre;
        }
        #endregion

    }
}
