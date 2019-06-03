using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
 
namespace ExcelDataBase
{
    public class DataBaseCache
    {
        //public static DataBaseCache DBCache;
        public const string DefDB = "GSY";//gsidp所在数据库       
        public const string GSDB = "GSDB";//gs所在数据库
        public const string nocacheSessionDB = "NoCacheSessionDB";//web.config配置项，不缓存DB。在数据库链接不设置的时候才起作用，即web.config中ConnectionStrings中不能设置GSY的链接信息 。
        public static bool EnabledSession = false;

        private static Dictionary<string, Database> DBCache = new Dictionary<string, Database>();

        private static Dictionary<string, Dictionary<string, Database>> DBGSSessionCache = new Dictionary<string, Dictionary<string, Database>>();


        #region static DataBaseCache()
        /// <summary>
        /// 静态方法生成DBCache
        /// </summary>
        static DataBaseCache()
        {
            #region 数据库结构
            /*Name	Code	Data Type	Length	Precision	Primary	Foreign Key	Mandatory
id	GSYDTSRC_ID	varchar(50)	50		TRUE	FALSE	TRUE
名称	GSYDTSRC_NAME	VARCHAR(60)	60		FALSE	FALSE	FALSE
WEB主机地址	GSYDTSRC_WEBHOST	VARCHAR(100)	100		FALSE	FALSE	FALSE
数据库类型	GSYDTSRC_DBTYPE	VARCHAR(10)	10		FALSE	FALSE	FALSE
数据主机地址	GSYDTSRC_DBHOST	VARCHAR(100)	100		FALSE	FALSE	FALSE
数据库名	GSYDTSRC_DBNAME	VARCHAR(50)	50		FALSE	FALSE	FALSE
用户名	GSYDTSRC_DBUSER	VARCHAR(50)	50		FALSE	FALSE	FALSE
密码	GSYDTSRC_DBPWD	VARCHAR(50)	50		FALSE	FALSE	FALSE
使用否	GSYDTSRC_ISUSED	CHAR(1)	1		FALSE	FALSE	FALSE
备注	GSYDTSRC_NOTE	VARCHAR(200)	200		FALSE	FALSE	FALSE*/
            #endregion

            iniDatabase();

        }
        #endregion


        private static void iniDatabase()
        {
            try
            {
                //System.Configuration.ConfigurationAllowDefinition inssettings = System.Configuration.ConfigurationManager.ConnectionStrings[ChangeIns];
                //if (inssettings == null || inssettings. = "false") return;

                System.Configuration.ConnectionStringSettings settings = System.Configuration.ConfigurationManager.ConnectionStrings[DefDB];
                if (settings == null) return;
                Database db = Database.Create(DefDB);
                DBCache.Add(DefDB, db);
                CacheDatabase(db);

            }
            catch (Exception ex)
            {
                //Log.Fatal(ex);
            }
        }
        public static void cacheDB(string name, Database db)
        {
            RemoveDatabase(name);
            DBCache.Add(name, db);
        }
        public static Database getCacheDB(string dbsrc)
        {
            Database db = null;

            System.Configuration.ConnectionStringSettings settings = System.Configuration.ConfigurationManager.ConnectionStrings[DefDB];

            if (settings == null && EnabledSession)
            {

                db = GetGSDb(dbsrc);//获取session数据库连接字符               
                string str = System.Configuration.ConfigurationManager.AppSettings.Get(nocacheSessionDB);
                if (!string.IsNullOrEmpty(str) && str == "1")
                    db = GetGSSessionCacheDb(db, dbsrc);//缓存数据库链接                 
            }
            else
            {

                if (0 == DBCache.Count) iniDatabase();
                if (dbsrc != GSDB)
                {
                    if (0 != DBCache.Count && DBCache.ContainsKey(dbsrc))
                    {
                        db = (DataBaseCache.DBCache[dbsrc] as Database);
                    }
                    else if (EnabledSession)
                    {
                        db = GetGSDb(dbsrc);
                    }
                }
                else
                {
                    db = GetGSDb(dbsrc);
                }
            }
            return db;
        }
        public static Database GetGSSessionCacheDb(Database dbgs, string dbsrc)
        {

            if (!DBGSSessionCache.ContainsKey(dbgs.ConnectionString))
            {
                DBGSSessionCache.Add(dbgs.ConnectionString, new Dictionary<string, Database>());
                DBGSSessionCache[dbgs.ConnectionString].Add(DefDB, dbgs);
                DBGSSessionCache[dbgs.ConnectionString].Add(GSDB, dbgs);
                DataSet ds = dbgs.ExecuteDataSet("select * from GSYDTSRC ");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DBGSSessionCache[dbgs.ConnectionString].Add(row["GSYDTSRC_ID"].ToString(), Database.Create(row["GSYDTSRC_ID"].ToString(),
                            row["GSYDTSRC_DBTYPE"].ToString(), row["GSYDTSRC_DBHOST"].ToString(),
                            row["GSYDTSRC_DBNAME"].ToString(), row["GSYDTSRC_DBUSER"].ToString(), row["GSYDTSRC_DBPWD"].ToString()));
                    }
                }
                else//如果没有在数据源中定义，则返回gs数据库
                {
                    return dbgs;
                }
            }
            else//如果没有在数据源中定义，则返回gs数据库
            {
                return dbgs;
            }
            //db.ConnectionString           
            return DBGSSessionCache[dbgs.ConnectionString][dbsrc];
        }
        public static Dictionary<string, Database> getDBCache()
        {
            return DBCache;
        }
        private static Database GetGSDb(string dbname)
        {
            Database db = null;

            try
            {
                //if (Session.GSIDPSession.Session["GSDBCONNSTR"] != null
                //&& !string.IsNullOrEmpty(Session.GSIDPSession.Session["GSDBCONNSTR"].ToString()))
                //{
                //    db = Database.Create(Session.GSIDPSession.Session["GSDBCONNSTR"].ToString(), Session.GSIDPSession.Session["GSDBTYPE"].ToString());
                //    //db = Database.Create(GSDB, provider, source, catalog, userid, password);
                //    db.Name = dbname;
                //}
            }
            catch (Exception ex)
            {
                //Log.Error(ex);
                //db = Database.Create(
                //    GSIDP.GSYS.Session.GSIDPSession.getDefaultConnection(), GSIDP.GSYS.Session.GSIDPSession.getDefaultGSDBTYPE());
                ////db = Database.Create(GSDB, provider, source, catalog, userid, password);
                //db.Name = dbname;
            }

            return db;
        }
        public static void CacheDatabase(Database db)
        {
            DataSet ds = db.ExecuteDataSet("select * from GSYDTSRC ");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    AddDatabase(row["GSYDTSRC_ID"].ToString(),
                        row["GSYDTSRC_DBTYPE"].ToString(), row["GSYDTSRC_DBHOST"].ToString(),
                        row["GSYDTSRC_DBNAME"].ToString(), row["GSYDTSRC_DBUSER"].ToString(), row["GSYDTSRC_DBPWD"].ToString());
                }
            }
        }

        public static void AddDatabase(string name, string dbtype, string dbhost, string dbname, string dbuser, string dbpwd)
        {
            RemoveDatabase(name);
            Database vsdb = Database.Create(name, dbtype, dbhost, dbname, dbuser, dbpwd);
            DBCache.Add(name, vsdb);
        }


        public static void RemoveDatabase(string name)
        {
            if (0 != DBCache.Count && DBCache.ContainsKey(name))
            {
                DBCache.Remove(name);
            }
        }
        public static bool TestDatabase(string name, string dbtype, string dbhost, string dbname, string dbuser, string dbpwd)
        {
            Database vsdb = null;
            bool bl = true;
            try
            {
                vsdb = Database.Create(name, dbtype, dbhost, dbname, dbuser, dbpwd);
                vsdb.GetProperties();
            }
            catch (Exception ex)
            {
                bl = false;
                throw ex;
            }
            finally
            {
                if (vsdb != null)
                    vsdb = null;
            }
            return bl;
        }


        public static string DealEncodePWD(string pwd)
        {
            if (pwd.Length < 21)
            {
                //pwd = Encrypt.EncrypRijndael(pwd);
            }
            return pwd;
        }
        public static string DealDecodePWD(string pwd)
        {
            if (pwd.Length > 21)
            {
                //pwd = Encrypt.DecrypRijndael(pwd);
            }
            return pwd;
        }
    }
}
