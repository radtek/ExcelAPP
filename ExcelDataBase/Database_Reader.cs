

using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
 

namespace ExcelDataBase
{
    /// <summary>
    /// 执行datareader方法
    /// </summary>
    public partial class Database
    {
        ///
        ///代理调用声明
        ///

        #region public delegate object DBCallback(System.Data.Common.DbDataReader reader);
        /// <summary>
        /// 执行datareader的方法
        /// </summary>
        /// <param name="reader">datareader对象</param>
        /// <returns>object对象</returns>
        public delegate object DBCallback(System.Data.Common.DbDataReader reader);
        #endregion
        
        ///
        ///执行DataReader 返回Dictionary 自动创建conn，自动关闭conn
        /// 

        #region public Dictionary<string,object> ExecuteReadDict(string sql)
        /// <summary>
        /// 返回Dictionary 自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>dict对象</returns>
        public Dictionary<string,object> ExecuteReadDict(string sql)
        {
            return this.ExecuteReadDict(sql, CommandType.Text, null);

        }
        #endregion

        #region public Dictionary<string, object> ExecuteReadDict(string sql,DbParam[] param)
        /// <summary>
        /// 返回Dictionary 自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <returns>dict对象</returns>
        public Dictionary<string, object> ExecuteReadDict(string sql,DbParam[] param)
        {
            return this.ExecuteReadDict(sql, CommandType.StoredProcedure, param);

        }
        #endregion

        #region public Dictionary<string,object> ExecuteReadDict(string sql, CommandType type, DbParam[] param)
        /// <summary>
        /// 返回Dictionary 自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="sql">sql或存储过程名</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <returns>dict对象</returns>
        public Dictionary<string,object> ExecuteReadDict(string sql, CommandType type, DbParam[] param)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            Dictionary<string, object> returns;
            using (DbCommand cmd = this.CreateCommand(sql, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }

                returns = (Dictionary<string, object>)this.DoExecuteRead(cmd, 
                    delegate(System.Data.Common.DbDataReader reader)
            {
                if (reader.Read())
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dict.Add(reader.GetName(i).ToUpper(),reader.GetValue(i));               
                    }
                    return dict;
                }
                else
                {
                    return null;
                }
            });
            }
            return returns;
        }
        #endregion

        ///
        ///执行DataReader 返回object 需要代理调用，自动创建conn，自动关闭conn
        ///

        #region public object ExecuteRead(string sqltext, DBCallback callback)

        /// <summary>
        /// 执行datareader，参数sql语句 和 delegate返回调用,自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="sqltext">sql语句</param>
        /// <param name="callback"> delegate 方法</param>
        /// <returns>object类型，基本类型</returns>
        public object ExecuteRead(string sqltext, DBCallback callback)
        {
            if (string.IsNullOrEmpty(sqltext))
                throw new ArgumentNullException("sqltext");

            if (null == callback)
                throw new ArgumentNullException("callback");

            return this.ExecuteRead(sqltext, CommandType.Text, null, callback);
        }

        #endregion

        #region public object ExecuteRead(string text,DbParam[] param, DBCallback callback)

        /// <summary>
        ///执行存储过程或sql的datareader,自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="text">proc</param>
        /// <param name="param">参数</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object基本类型</returns>
        public object ExecuteRead(string text,  DbParam[] param, DBCallback callback)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (null == callback)
                throw new ArgumentNullException("callback");

            return this.ExecuteRead(text, CommandType.StoredProcedure, param, callback);
        }

        #endregion

        #region public object ExecuteRead(string text,CommandType type,DbParam[] param, DBCallback callback)

        /// <summary>
        ///执行存储过程或sql的datareader,自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="text">sql 或proc</param>
        /// <param name="type">sql 或存储过程</param>
        /// <param name="param">参数</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object基本类型</returns>
        public object ExecuteRead(string text,CommandType type, DbParam[] param, DBCallback callback)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (null == callback)
                throw new ArgumentNullException("callback");

            object returns;

            using (DbCommand cmd = this.CreateCommand(text, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }
                returns = this.DoExecuteRead(cmd, callback);
            }

            return returns;
        }

        #endregion

        #region public object ExecuteRead(DbCommand cmd, DBCallback callback)

        /// <summary>
        /// 通过cmd执行datareader,自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object对象</returns>
        public object ExecuteRead(DbCommand cmd, DBCallback callback)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            if (cmd.Connection == null)
                throw new ArgumentNullException("cmd.Connection");

            if (callback == null)
                throw new ArgumentNullException("callback");

            object returns = this.DoExecuteRead(cmd, callback);

            return returns;
        }



        #endregion

        #region protected virtual object DoExecuteRead(DbCommand cmd, DBCallback callback)

        /// <summary>
        /// 具体执行类,自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object对象</returns>
        /// <remarks> </remarks>
        protected virtual object DoExecuteRead(DbCommand cmd, DBCallback callback)
        {
            object returns = null;
            DbDataReader reader = null;
            bool opened = false; 
            try
            {
                //处理sql语句
                dealCMD(cmd);
                //
                if (cmd.Connection.State == ConnectionState.Closed)
                {
                    cmd.Connection.Open();
                    opened = true;
                } 
                reader = cmd.ExecuteReader();
                returns = callback(reader);
            }
            catch (Exception ex)
            {
                this.HandleExecutionError(ex);
            }
            finally
            {
                if (null != reader)
                    reader.Dispose();

                if (opened)
                    cmd.Connection.Close();              
            }

            return returns;
        }

        #endregion

        ///
        ///执行DataReader 返回Dictionary 支持事务,不关闭conn
        /// 

        #region public Dictionary<string,object> ExecuteReadDictTrans(string sql, DbTransaction trans)
        /// <summary>
        /// 返回Dictionary 支持事务,不关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="trans">事务</param>
        /// <returns>dict对象</returns>
        public Dictionary<string, object> ExecuteReadDictTrans(string sql, DbTransaction trans)
        {
            return this.ExecuteReadDictTrans(sql, CommandType.Text, null,trans);

        }
        #endregion

        #region public Dictionary<string, object> ExecuteReadDictTrans(string sql,DbParam[] param, DbTransaction trans)
        /// <summary>
        /// 返回Dictionary支持事务,不关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <param name="trans">事务</param>
        /// <returns>dict对象</returns>
        public Dictionary<string, object> ExecuteReadDictTrans(string sql, DbParam[] param, DbTransaction trans)
        {
            return this.ExecuteReadDictTrans(sql, CommandType.StoredProcedure, param,trans);

        }
        #endregion

        #region public Dictionary<string,object> ExecuteReadDictTrans(string sql, CommandType type, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// 返回Dictionary支持事务,不关闭conn
        /// </summary>
        /// <param name="sql">sql 或存储过程名</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <param name="trans">事务</param>
        /// <returns>dict对象</returns>
        public Dictionary<string, object> ExecuteReadDictTrans(string sql, CommandType type, DbParam[] param, DbTransaction trans)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            Dictionary<string, object> returns;
            using (DbCommand cmd = this.CreateCommand(trans,sql, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }

                returns = (Dictionary<string, object>)this.DoExecuteReadTrans(cmd,
                    delegate(System.Data.Common.DbDataReader reader)
                    {
                        if (reader.Read())
                        {
                            Dictionary<string, object> dict = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                dict.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            return dict;
                        }
                        else
                        {
                            return null;
                        }
                    });
            }
            return returns;
        }
        #endregion

        ///
        ///执行DataReader 返回object 支持trans 需代理调用
        ///

        #region public object ExecuteReadTrans(string sqltext, DbTransaction trans,DBCallback callback)

        /// <summary>
        /// 执行datareader，参数sql语句 和 delegate返回调用,不关闭conn
        /// </summary>
        /// <param name="sqltext">sql语句</param>
        /// <param name="trans">事务</param>
        /// <param name="callback"> delegate 方法</param>
        /// <returns>object类型，基本类型</returns>
        public object ExecuteReadTrans(string sqltext, DbTransaction trans, DBCallback callback)
        {
            if (string.IsNullOrEmpty(sqltext))
                throw new ArgumentNullException("sqltext");

            if (null == callback)
                throw new ArgumentNullException("callback");
            return this.ExecuteReadTrans(sqltext, CommandType.Text, null, trans, callback);
        }

        #endregion

        #region public object ExecuteReadTrans(string text,DbParam[] param, DbTransaction trans,DBCallback callback)

        /// <summary>
        ///执行存储过程或sql的datareader,不关闭conn
        /// </summary>
        /// <param name="text">存储过程</param>
        /// <param name="param">参数</param>
        /// <param name="trans">事务</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object基本类型</returns>
        public object ExecuteReadTrans(string text, DbParam[] param, DbTransaction trans, DBCallback callback)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (null == callback)
                throw new ArgumentNullException("callback");

            return this.ExecuteReadTrans(text,CommandType.StoredProcedure,param,trans, callback);
        }

        #endregion

        #region public object ExecuteReadTrans(string text,CommandType type,DbParam[] param, DbTransaction trans,DBCallback callback)

        /// <summary>
        ///执行存储过程或sql的datareader,不关闭conn
        /// </summary>
        /// <param name="text">sql 或proc</param>        
        /// <param name="type">sql 或存储过程</param>
        /// <param name="param">参数</param>
        /// <param name="trans">事务</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object基本类型</returns>
        public object ExecuteReadTrans(string text,CommandType type, DbParam[] param, DbTransaction trans, DBCallback callback)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (null == callback)
                throw new ArgumentNullException("callback");

            object returns;

            using (DbCommand cmd = this.CreateCommand(trans, text,type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }
                returns = this.DoExecuteReadTrans(cmd, callback);
            }

            return returns;
        }

        #endregion

        #region public object ExecuteReadTrans(DbCommand cmd, DBCallback callback)

        /// <summary>
        /// 通过cmd执行datareader,不关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object对象</returns>
        public object ExecuteReadTrans(DbCommand cmd,  DBCallback callback)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            if (cmd.Connection == null)
                throw new ArgumentNullException("cmd.Connection");

            if (callback == null)
                throw new ArgumentNullException("callback");
            object returns = this.DoExecuteReadTrans(cmd, callback);
            return returns;
        }



        #endregion

        #region protected virtual object DoExecuteReadTrans(DbCommand cmd, DBCallback callback)

        /// <summary>
        /// 具体执行类,不关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object对象</returns>
        /// <remarks> </remarks>
        protected virtual object DoExecuteReadTrans(DbCommand cmd, DBCallback callback)
        {
            object returns = null;
            DbDataReader reader = null;
            //处理sql或存储过程名
            dealCMD(cmd);
            //
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();

            }
            reader = cmd.ExecuteReader();
            returns = callback(reader);
            return returns;
        }

        #endregion

        ///
        ///执行DataReader 返回Dictionary 由conn创建command,不关闭conn
        /// 

        #region public Dictionary<string,object> ExecuteReadDictConn(string sql, DbConnection conn)
        /// <summary>
        /// 返回Dictionary 由conn创建command,不关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns>dict对象</returns>
        public Dictionary<string, object> ExecuteReadDictConn(string sql, DbConnection conn)
        {
            return this.ExecuteReadDictConn(sql, CommandType.Text, null, conn);

        }
        #endregion

        #region public Dictionary<string, object> ExecuteReadDictConn(string sql,DbParam[] param, DbConnection conn)
        /// <summary>
        /// 返回Dictionary由conn创建command,不关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns>dict对象</returns>
        public Dictionary<string, object> ExecuteReadDictConn(string sql, DbParam[] param, DbConnection conn)
        {
            return this.ExecuteReadDictConn(sql, CommandType.StoredProcedure, param, conn);

        }
        #endregion

        #region public Dictionary<string,object> ExecuteReadDictConn(string sql, CommandType type, DbParam[] param, DbConnection conn)
        /// <summary>
        /// 返回Dictionary,由conn创建command,不关闭conn
        /// </summary>
        /// <param name="sql">sql 或存储过程名</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns>dict对象</returns>
        public Dictionary<string, object> ExecuteReadDictConn(string sql, CommandType type, DbParam[] param, DbConnection conn)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            Dictionary<string, object> returns;
            using (DbCommand cmd = this.CreateCommand(conn, sql, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }

                returns = (Dictionary<string, object>)this.DoExecuteReadConn(cmd,
                    delegate(System.Data.Common.DbDataReader reader)
                    {
                        if (reader.Read())
                        {
                            Dictionary<string, object> dict = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                dict.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            return dict;
                        }
                        else
                        {
                            return null;
                        }
                    });
            }
            return returns;
        }
        #endregion

        ///
        ///执行DataReader 返回object 支持conn 需代理调用
        ///

        #region public object ExecuteReadConn(string sqltext, DbConnection conn,DBCallback callback)

        /// <summary>
        /// 执行datareader，参数sql语句 和 delegate返回调用,不关闭conn
        /// </summary>
        /// <param name="sqltext">sql语句</param>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="callback"> delegate 方法</param>
        /// <returns>object类型，基本类型</returns>
        public object ExecuteReadConn(string sqltext, DbConnection conn, DBCallback callback)
        {
            if (string.IsNullOrEmpty(sqltext))
                throw new ArgumentNullException("sqltext");

            if (null == callback)
                throw new ArgumentNullException("callback");
            return this.ExecuteReadConn(sqltext, CommandType.Text, null, conn, callback);
        }

        #endregion

        #region public object ExecuteReadConn(string text,DbParam[] param, DbConnection conn,DBCallback callback)

        /// <summary>
        ///执行存储过程或sql的datareader,不关闭conn
        /// </summary>
        /// <param name="text">存储过程</param>
        /// <param name="param">参数</param>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object基本类型</returns>
        public object ExecuteReadConn(string text, DbParam[] param, DbConnection conn, DBCallback callback)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (null == callback)
                throw new ArgumentNullException("callback");

            return this.ExecuteReadConn(text, CommandType.StoredProcedure, param, conn, callback);
        }

        #endregion

        #region public object ExecuteReadConn(string text,CommandType type,DbParam[] param, DbConnection conn,DBCallback callback)

        /// <summary>
        ///执行存储过程或sql的datareader,不关闭conn
        /// </summary>
        /// <param name="text">sql 或proc</param>        
        /// <param name="type">sql 或存储过程</param>
        /// <param name="param">参数</param>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object基本类型</returns>
        public object ExecuteReadConn(string text, CommandType type, DbParam[] param, DbConnection conn, DBCallback callback)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (null == callback)
                throw new ArgumentNullException("callback");

            object returns;

            using (DbCommand cmd = this.CreateCommand(conn, text, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }
                returns = this.DoExecuteReadConn(cmd, callback);
            }

            return returns;
        }

        #endregion

        #region public object ExecuteReadConn(DbCommand cmd, DBCallback callback)

        /// <summary>
        /// 通过cmd执行datareader,不关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object对象</returns>
        public object ExecuteReadConn(DbCommand cmd, DBCallback callback)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            if (cmd.Connection == null)
                throw new ArgumentNullException("cmd.Connection");

            if (callback == null)
                throw new ArgumentNullException("callback");
            object returns = this.DoExecuteReadConn(cmd, callback);
            return returns;
        }



        #endregion

        #region protected virtual object DoExecuteReadConn(DbCommand cmd, DBCallback callback)

        /// <summary>
        /// 具体执行类,不关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <param name="callback">代理方法</param>
        /// <returns>object对象</returns>
        /// <remarks> </remarks>
        protected virtual object DoExecuteReadConn(DbCommand cmd, DBCallback callback)
        {
            object returns = null;
            DbDataReader reader = null;
            //处理sql或存储过程名
            dealCMD(cmd);
            //
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();

            } 
            reader = cmd.ExecuteReader();
            returns = callback(reader);
            return returns;
        }

        #endregion

    }
}
