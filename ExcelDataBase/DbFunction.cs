using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ExcelDataBase
{
    public class DbFunction
    {

		
        //
        // 公用方法
        //

        #region public static DBType DataBaseType(string dbname)
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <param name="name">database名称</param>
        /// <returns>数据库类型，MSS Ora等</returns>
        public static DataBaseType DBType(string dbname)
        {
            DataBaseType vsre = DataBaseType.NONE;
            vsre = (GetDB(dbname) as Database).DBType;
            return vsre;
        }
        #endregion

        #region public static Database GetDB(string dbname)
        /// <summary>
        /// 获取database对象
        /// </summary>
        /// <param name="dbname">database名称</param>
        /// <returns>数据库类型，MSS Ora等</returns>
        public static Database GetDB(string dbname)
        {
            if (string.IsNullOrEmpty(dbname)) dbname = DataBaseCache.DefDB;
            Database db =DataBaseCache.getCacheDB(dbname);
            return db;
        }
        #endregion

		

       


        //
        //获取单一值
        //

        #region public static object ExecuteScalar(string text)

        /// <summary>
        /// 从DataBaseCache.DefDB获取单值的函数，自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="text">sql语句</param>
        /// <returns>object类型的对象</returns>
        public static T ExecuteScalar<T>(string text)
        {            
            return ExecuteScalar<T>(DataBaseCache.DefDB,text);
        }

        public static T ExecuteScalar<T>(string dbsrc,string text)
        {
            object obj = GetDB(dbsrc).ExecuteScalar(text);
            if ((obj is System.DBNull) ||obj == null) return default(T);
            else return (T)obj;
        }
        public static object ExecuteScalar(string text)
        {
            return GetDB(DataBaseCache.DefDB).ExecuteScalar(text);
        }
        #endregion

        #region public static object ExecuteScalar(string dbname,string text)

        /// <summary>
        /// 获取单值的函数，自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="dbname">数据库名</param>
        /// <param name="text">sql语句</param>
        /// <returns>object类型的对象</returns>
        public static object ExecuteScalar(string dbname, string text)
        {
            return GetDB(dbname).ExecuteScalar(text);
        }

        #endregion

        #region public static object ExecuteScalar(string dbname,string sql, DbParam[] param)
        /// <summary>
        /// 执行存储过程返回单一值，自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="dbname">数据库名</param>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string dbname, string sql, DbParam[] param)
        {
            Database db = GetDB(dbname);
            return db.ExecuteScalar(sql, param);
        }
        #endregion

        #region public static object ExecuteScalar(string dbname,string sql, CommandType type,DbParam[] param)

        /// <summary>
        /// 获取单值的函数，自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="dbname">数据库名</param>
        /// <param name="text">sql语句或存储过程名</param>
        /// <param name="type">执行类型：sql或存储过程</param>
        /// <param name="param">参数</param>
        /// <returns>object类型</returns>
        public static object ExecuteScalar(string dbname, string sql, CommandType type, DbParam[] param)
        {

            Database db = GetDB(dbname);
            return db.ExecuteScalar(sql, type, param);
        }

        #endregion
        
        ///
        ///执行无返回值函数
        ///

        #region public static int ExecuteNonQuery(string text)

        /// <summary>
        /// 从DataBaseCache.DefDB执行sql 自动创建并关闭conn
        /// </summary>      
        /// <param name="sql">sql</param>
        /// <returns>返回影响行数</returns>
        public static int ExecuteNonQuery(string sql)
        {
            return GetDB(DataBaseCache.DefDB).ExecuteNonQuery(sql);
        }

        #endregion

        #region public static int ExecuteNonQuery(string dbname,string text)

        /// <summary>
        /// 执行sql 自动创建并关闭conn
        /// </summary>
        /// <param name="dbname">数据库名</param>
        /// <param name="sql">sql</param>
        /// <returns>返回影响行数</returns>
        public static int ExecuteNonQuery(string dbname, string sql)
        {           
            return GetDB(dbname).ExecuteNonQuery(sql);
        }

        #endregion

        #region public static int ExecuteNonQuery(string dbname,string text, DbParam[] param)

        /// <summary>
        /// 执行带参数的sql,自动创建并关闭conn
        /// </summary>
        /// <param name="dbname">数据库名</param>
        /// <param name="sql">sql 或 存储过程名</param>
        /// <param name="param">参数</param>
        /// <returns>返回影响行数</returns>
        public static int ExecuteNonQuery(string dbname, string sql, DbParam[] param)
        {

            Database db = GetDB(dbname);
            return db.ExecuteNonQuery(sql, param);
        }
        #endregion

        #region public static int ExecuteNonQuery(string dbname,string sql,CommandType type,  DbParam[] param)
        /// <summary>
        /// 执行带参数的sql,自动创建并关闭conn
        /// </summary>
        /// <param name="dbname">数据库名</param>
        /// <param name="sql">sql 或 存储过程名</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <returns>返回影响行数</returns>
        public static int ExecuteNonQuery(string dbname, string sql, CommandType type, DbParam[] param)
        {
            Database db = GetDB(dbname);
            return db.ExecuteNonQuery(sql, type, param);
        }

        #endregion

        ///
        ///执行DataReader 返回Dictionary 自动创建conn，自动关闭conn
        /// 

        #region public static Dictionary<string,object> ExecuteReadDict(string sql)
        /// <summary>
        /// 返回Dictionary 自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>dict对象</returns>
        public static Dictionary<string, object> ExecuteReadDict(string sql)
        {
            Database db = GetDB(DataBaseCache.DefDB);
            return db.ExecuteReadDict(sql);
        }
        #endregion

        #region public static Dictionary<string,object> ExecuteReadDict(string dbname,string sql)
        /// <summary>
        /// 返回Dictionary 自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>dict对象</returns>
        public static Dictionary<string, object> ExecuteReadDict(string dbname, string sql)
        {
            Database db = GetDB(dbname);
            return db.ExecuteReadDict(sql);
        }
        #endregion

        #region static public Dictionary<string, object> ExecuteReadDict(string dbname,string sql,DbParam[] param)
        /// <summary>
        /// 返回Dictionary 自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <returns>dict对象</returns>
        public static Dictionary<string, object> ExecuteReadDict(string dbname, string sql, DbParam[] param)
        {
            

            Database db = GetDB(dbname);
            return db.ExecuteReadDict(sql, param);

        }
        #endregion

        #region static public Dictionary<string,object> ExecuteReadDict(string dbname,string sql, CommandType type, DbParam[] param)
        /// <summary>
        /// 返回Dictionary 自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="sql">sql或存储过程名</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <returns>dict对象</returns>
        public static Dictionary<string, object> ExecuteReadDict(string dbname, string sql, CommandType type, DbParam[] param)
        {
            Database db = GetDB(dbname); 
            return db.ExecuteReadDict(sql,type, param);
        }
        #endregion

        ///
        ///执行DataReader 返回object 需要代理调用，自动创建conn，自动关闭conn
        ///

        #region public static object ExecuteRead(string dbname,string sqltext,Genersoft.GSIDP.GSYS.Data.Database.DBCallback callback)

        /// <summary>
        /// 执行datareader，参数sql语句 和 delegate返回调用,自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="sqltext">sql语句</param>
        /// <param name="callback"> delegate 方法</param>
        /// <returns>object类型，基本类型</returns>
        public static object ExecuteRead(string dbname, string sqltext, Database.DBCallback callback)
        {
            Database db = GetDB(dbname);
            return db.ExecuteRead(sqltext,callback);
        }

        #endregion

        #region public static object ExecuteRead(string dbname,string text,DbParam[] param,DBCallback  callback)

        /// <summary>
        ///执行存储过程或sql的datareader,自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="text">proc</param>
        /// <param name="param">参数</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object基本类型</returns>
        public static object ExecuteRead(string dbname, string text, DbParam[] param, Database.DBCallback callback)
        {

            Database db = GetDB(dbname);
            return db.ExecuteRead(text, CommandType.StoredProcedure, param, callback);
        }

        #endregion

        #region public static object ExecuteRead(string dbname,string text,CommandType type,DbParam[] param,  DBCallback callback)

        /// <summary>
        ///执行存储过程或sql的datareader,自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="text">sql 或proc</param>
        /// <param name="type">sql 或存储过程</param>
        /// <param name="param">参数</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object基本类型</returns>
        public static object ExecuteRead(string dbname, string text, CommandType type, DbParam[] param, Database.DBCallback callback)
        {

            Database db = GetDB(dbname);
            return db.ExecuteRead(text, type, param, callback);
        }

        #endregion

        ///
        ///返回dataset，自动创建conn 执行结束关闭conn
        ///

        #region public static DataSet ExecuteDataSet(string text)

        /// <summary>
        /// 从DataBaseCache.DefDB获取Dataset ，关闭conn
        /// </summary>
        /// <param name="text">sql语句</param>
        /// <returns>DataSet类型的对象</returns>
        public static DataSet ExecuteDataSet(string text)
        {
            Database db = GetDB(DataBaseCache.DefDB);
            return db.ExecuteDataSet(text);
        }

        #endregion
        
        #region public static DataSet ExecuteDataSet(string dbname,string text)

        /// <summary>
        /// 获取Dataset ，关闭conn
        /// </summary>
        /// <param name="text">sql语句</param>
        /// <returns>DataSet类型的对象</returns>
        public static DataSet ExecuteDataSet(string dbname, string text)
        {
            Database db = GetDB(dbname);
            return db.ExecuteDataSet(text);
        }

        #endregion

        #region public static DataSet ExecuteDataSet(string dbname,string sql, DbParam[] param)
        /// <summary>
        /// 获取dataset，关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string dbname, string sql, DbParam[] param)
        {
            Database db = GetDB(dbname);
            return db.ExecuteDataSet(sql, param); 
        }
        #endregion

        #region public static DataSet ExecuteDataSet(string dbname,string text, CommandType type,DbParam[] param)

        /// <summary>
        /// 获取dataset，关闭conn
        /// </summary>
        /// <param name="text">sql语句或存储过程名</param>
        /// <param name="type">执行类型：sql或存储过程</param>
        /// <param name="param">参数</param>
        /// <returns>DataSet类型</returns>
        public static DataSet ExecuteDataSet(string dbname, string text, CommandType type, DbParam[] param)
        {

            Database db = GetDB(dbname);
            return db.ExecuteDataSet(text, type, param); 
        }

        #endregion
    }
}
