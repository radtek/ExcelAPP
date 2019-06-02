

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
 
namespace ExcelDataBase
{
    /// <summary>
    /// 处理获取单值函数
    /// </summary>
    public partial class Database
    {
        ///
        ///直接执行，执行结束关闭conn，自动创建conn，自动关闭conn
        ///

        #region public object ExecuteScalar(string text)

        /// <summary>
        /// 获取单值的函数，自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="text">sql语句</param>
        /// <returns>object类型的对象</returns>
        public object ExecuteScalar(string text)
        {
            return this.ExecuteScalar(text, CommandType.Text,null);
        }

        #endregion

        #region public object ExecuteScalar(string sql, DbParam[] param)
        /// <summary>
        /// 执行存储过程返回单一值，自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, DbParam[] param)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            return this.ExecuteScalar(sql, CommandType.StoredProcedure, param); ;
        }
        #endregion

        #region public object ExecuteScalar(string text, CommandType type,DbParam[] param)

        /// <summary>
        /// 获取单值的函数，自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="text">sql语句或存储过程名</param>
        /// <param name="type">执行类型：sql或存储过程</param>
        /// <param name="param">参数</param>
        /// <returns>object类型</returns>
        public object ExecuteScalar(string text, CommandType type,DbParam[] param)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            object result;

            using (DbCommand cmd = this.CreateCommand(text, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }
                result = this.DoExecuteScalar(cmd);
            }
            if (result == DBNull.Value)
            {
                result = null;
            }
            return result;
        }

        #endregion

        #region public object ExecuteScalar(DbCommand cmd)

        /// <summary>
        /// 获取单值的函数，自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="cmd">DbCommand对象</param>
        /// <returns>object类型</returns>
        public object ExecuteScalar(DbCommand cmd)
        {
            if (null == cmd)
                throw new ArgumentNullException("cmd");

            if (null == cmd.Connection)
                throw new ArgumentNullException("cmd.Connection");

            object result = DoExecuteScalar(cmd);

            return result;
        }

        #endregion

        #region protected virtual object DoExecuteScalar(DbCommand cmd)

        /// <summary>
        ///  执行获取单值的对象，自动创建conn，自动关闭conn
        /// </summary>
        /// <param name="cmd">Dbcommand对象</param>
        /// <returns>object类型</returns>
        protected virtual object DoExecuteScalar(DbCommand cmd)
        {
            object result = null;
            bool opened = false; 
            //sql 或存储过程名处理
            dealCMD(cmd);
            //执行
            try
            {
                if (cmd.Connection.State == ConnectionState.Closed)
                {
                    cmd.Connection.Open();
                    opened = true;
                }

                
                result = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                this.HandleExecutionError(ex);
            }
            finally
            {                 
                if (opened)
                    cmd.Connection.Close();
            }
            return result;
        }

        #endregion

        ///
        ///下面为事务准备,不关闭conn
        ///

        #region    public object ExecScalarTrans(string sql,DbTransaction trans)
        /// <summary>
        /// 执行sql事务，获取单一值,不关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        public object ExecScalarTrans(string sql,DbTransaction trans)
        {
            return this.ExecScalarTrans(sql, CommandType.Text, null,trans);
            
        }
        #endregion

        #region  public object ExecScalarTrans(string sql, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// 执行事务存储过程，获取单一值,不关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <param name="trans">事务</param>
        /// <returns>object对象</returns>
        public object ExecScalarTrans(string sql, DbParam[] param, DbTransaction trans)
        {
            return this.ExecScalarTrans(sql, CommandType.StoredProcedure, param, trans);
        }
        #endregion
        
        #region public object ExecScalarTrans(string sql, CommandType type, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// 执行sql或存储过程，带事务，获取单一值,不关闭conn
        /// </summary>
        /// <param name="sql">sql 或存储过程</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <param name="trans">事务</param>
        /// <returns>object对象</returns>
        public object ExecScalarTrans(string sql, CommandType type, DbParam[] param, DbTransaction trans)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("text");
            if (null == trans)
                throw new ArgumentNullException("trans");

            object returns;
            using (DbCommand cmd = this.CreateCommand(trans, sql, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }
                returns = this.DoExecScalarTrans(cmd);
            }
            return returns;
        }
        #endregion

        #region public object DoExecScalarTrans(DbCommand cmd)
        /// <summary>
        /// 执行返回单值，带事务,不关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <returns>object对象</returns>
        public object DoExecScalarTrans(DbCommand cmd)
        { 
            //sql 或存储过程名处理
            dealCMD(cmd);
            //如未打开数据库连接则打开数据库连接
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            } 
            return cmd.ExecuteScalar();

        }
        #endregion

        ///
        ///由conn创建command,不关闭conn
        ///

        #region    public object ExecScalarConn(string sql,DbConnection conn)
        /// <summary>
        /// 执行sql，由conn创建command，获取单一值,不关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns></returns>
        public object ExecScalarConn(string sql, DbConnection conn)
        {
            return this.ExecScalarConn(sql, CommandType.Text, null, conn);

        }
        #endregion

        #region  public object ExecScalarConn(string sql, DbParam[] param, DbConnection conn)
        /// <summary>
        /// 执行存储过程，由conn创建command，获取单一值,不关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns>object对象</returns>
        public object ExecScalarConn(string sql, DbParam[] param, DbConnection conn)
        {
            return this.ExecScalarConn(sql, CommandType.StoredProcedure, param, conn);
        }
        #endregion

        #region public object ExecScalarConn(string sql, CommandType type, DbParam[] param, DbConnection conn)
        /// <summary>
        /// 执行sql或存储过程，由conn创建command，获取单一值,不关闭conn
        /// </summary>
        /// <param name="sql">sql 或存储过程</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns>object对象</returns>
        public object ExecScalarConn(string sql, CommandType type, DbParam[] param, DbConnection conn)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("text");
            if (null == conn)
                throw new ArgumentNullException("conn");

            object returns;
            using (DbCommand cmd = this.CreateCommand(conn, sql, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }
                returns = this.DoExecScalarConn(cmd);
            }
            return returns;
        }
        #endregion

        #region public object DoExecScalarConn(DbCommand cmd)
        /// <summary>
        /// 执行返回单值，由conn创建command,不关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <returns>object对象</returns>
        public object DoExecScalarConn(DbCommand cmd)
        {
            //sql 或存储过程名处理
            dealCMD(cmd);
            //如未打开数据库连接则打开数据库连接
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            } 
            return cmd.ExecuteScalar();

        }
        #endregion

    }
}
