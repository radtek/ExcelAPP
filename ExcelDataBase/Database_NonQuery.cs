 

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
 
namespace ExcelDataBase
{
    /// <summary>
    /// 处理无返回值函数
    /// </summary>
    public partial class Database
    {


        //
        //处理无返回值,自动创建并关闭conn
        //              

        #region public int ExecuteNonQuery(string text)

        /// <summary>
        /// 执行sql 自动创建并关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQuery(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQuery(sql, CommandType.Text, null);
        }

        #endregion

        #region public int ExecuteNonQuery(string text, DbParam[] param)

        /// <summary>
        /// 执行带参数的sql,自动创建并关闭conn
        /// </summary>
        /// <param name="sql">sql 或 存储过程名</param>
        /// <param name="param">参数</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQuery(string sql, DbParam[] param)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQuery(sql, CommandType.StoredProcedure, param);
        }
        #endregion

        #region public int ExecuteNonQuery(string sql,CommandType type, DbParam[] param)
        /// <summary>
        /// 执行带参数的sql,自动创建并关闭conn
        /// </summary>
        /// <param name="sql">sql 或 存储过程名</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQuery(string sql,CommandType type, DbParam[] param)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            int result;

            using (DbCommand cmd = this.CreateCommand(sql,type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }
                result = this.DoExecuteNonQuery(cmd);
            }

            return result;
        }

        #endregion

        #region public int ExecuteNonQuery(DbCommand cmd)

        /// <summary>
        ///执行带参数的sql,自动创建并关闭conn
        /// </summary>
        /// <param name="cmd">  command 对象</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQuery(DbCommand cmd)
        {
            if (null == cmd)
                throw new ArgumentNullException("cmd");
            if (null == cmd.Connection)
                throw new ArgumentNullException("cmd.Connection");

            int result = this.DoExecuteNonQuery(cmd);

            return result;
        }

        #endregion

        #region protected virtual int DoExecuteNonQuery(DbCommand cmd)

        /// <summary>
        ///  执行sql,自动创建并关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <returns>返回影响行数</returns>
        protected virtual int DoExecuteNonQuery(DbCommand cmd)
        {
            int result = 0;
            bool opened = false;
            
            try
            {
                //sql 或存储过程名处理
                dealCMD(cmd);
                //打开数据库连接
                if (cmd.Connection.State == ConnectionState.Closed)
                {
                    cmd.Connection.Open();
                    opened = true;
                } 
                result = cmd.ExecuteNonQuery();
             
                //cmd.CommandText+cmd.Parameters);


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


        //
        //处理无返回值,支持事务,不关闭conn
        //              

        #region public int ExecuteNonQueryTrans(string text, DbTransaction trans)

        /// <summary>
        /// 执行sql,支持事务,不关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="trans">事务</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQueryTrans(string sql, DbTransaction trans)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQueryTrans(sql, CommandType.Text, null,trans);
        }

        #endregion

        #region public int ExecuteNonQueryTrans(string text, DbParam[] param, DbTransaction trans)

        /// <summary>
        /// 执行带参数的sql,支持事务,不关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <param name="trans">事务</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQueryTrans(string sql, DbParam[] param, DbTransaction trans)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQueryTrans(sql, CommandType.StoredProcedure, param,trans);
        }
        #endregion

        #region public int ExecuteNonQueryTrans(string sql,CommandType type, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// 执行带参数的sql,支持事务,不关闭conn
        /// </summary>
        /// <param name="sql">sql 或 存储过程名</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <param name="trans">事务</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQueryTrans(string sql, CommandType type, DbParam[] param, DbTransaction trans)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            int result;

            using (DbCommand cmd = this.CreateCommand(trans,sql, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
    
                    cmd.Parameters.AddRange(iparam);
                }
                result = this.DoExecuteNonQueryTrans(cmd);
            }

            return result;
        }

        #endregion

        #region public int ExecuteNonQueryTrans(DbCommand cmd)

        /// <summary>
        ///执行sql,支持事务,不关闭conn
        /// </summary>
        /// <param name="cmd">  command 对象</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQueryTrans(DbCommand cmd)
        {
            if (null == cmd)
                throw new ArgumentNullException("cmd");
            if (null == cmd.Connection)
                throw new ArgumentNullException("cmd.Connection");

            int result = this.DoExecuteNonQueryTrans(cmd);

            return result;
        }

        #endregion

        #region protected virtual int DoExecuteNonQueryTrans(DbCommand cmd)

        /// <summary>
        ///  执行sql,支持事务,不关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <returns>返回影响行数</returns>
        protected virtual int DoExecuteNonQueryTrans(DbCommand cmd)
        {

            //sql 或存储过程名处理
            dealCMD(cmd);
            //打开数据库连接
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            } 
            return  cmd.ExecuteNonQuery();

        }

        #endregion


        ///
        ///处理无返回值,由conn创建command,不关闭conn
        ///              

        #region public int ExecuteNonQueryConn(string text, DbConnection conn)

        /// <summary>
        /// 执行sql,由conn创建command,不关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQueryConn(string sql, DbConnection conn)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQueryConn(sql, CommandType.Text, null, conn);
        }

        #endregion

        #region public int ExecuteNonQueryConn(string text, DbParam[] param, DbConnection conn)

        /// <summary>
        /// 执行带参数的sql,由conn创建command,不关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQueryConn(string sql, DbParam[] param, DbConnection conn)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQueryConn(sql, CommandType.StoredProcedure, param, conn);
        }
        #endregion

        #region public int ExecuteNonQueryConn(string sql,CommandType type, DbParam[] param, DbConnection conn)
        /// <summary>
        /// 执行带参数的sql,由conn创建command,不关闭conn
        /// </summary>
        /// <param name="sql">sql 或 存储过程名</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQueryConn(string sql, CommandType type, DbParam[] param, DbConnection conn)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            int result;

            using (DbCommand cmd = this.CreateCommand(conn, sql, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }
                result = this.DoExecuteNonQueryConn(cmd);
            }

            return result;
        }

        #endregion

        #region public int ExecuteNonQueryConn(DbCommand cmd)

        /// <summary>
        ///执行sql,由conn创建command,不关闭conn
        /// </summary>
        /// <param name="cmd">  command 对象</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteNonQueryConn(DbCommand cmd)
        {
            if (null == cmd)
                throw new ArgumentNullException("cmd");
            if (null == cmd.Connection)
                throw new ArgumentNullException("cmd.Connection");

            int result = this.DoExecuteNonQueryConn(cmd);

            return result;
        }

        #endregion

        #region protected virtual int DoExecuteNonQueryConn(DbCommand cmd)

        /// <summary>
        ///  执行sql,由conn创建command,不关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <returns>返回影响行数</returns>
        protected virtual int DoExecuteNonQueryConn(DbCommand cmd)
        {

            //sql 或存储过程名处理
            dealCMD(cmd);
            //打开数据库连接
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            } 
            return cmd.ExecuteNonQuery();

        }

        #endregion
    }
}
