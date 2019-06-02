 

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
 
namespace ExcelDataBase
{
    /// <summary>
    /// DbCommand 对象
    /// </summary>
    public partial class Database
    {
        ///
        ///自动创建conn ，自动关闭conn
        ///

        #region public DbCommand CreateCommand()

        /// <summary>
        /// 创建一个dbcommand对象，connection对象自动释放
        /// </summary>
        /// <returns>dbcommand对象新实例</returns>
        /// <remarks>dbcommand对象释放时调用connection释放的方法</remarks>
        public DbCommand CreateCommand()
        {
            DbConnection con = this.CreateConnection();
            DbCommand cmd = this.CreateCommand(con);
            cmd.Disposed += new EventHandler(DisposeCommandConnection);
            return cmd;
        }

        #endregion 

        #region public DbCommand CreateCommand(string text)

        /// <summary>
        /// 创建一个dbcommand 对象，connection对象自动释放
        /// </summary>
        /// <param name="text">Sql</param>
        /// <returns>dbcommand对象新实例</returns>
        /// <remarks>dbcommand对象释放时调用connection释放的方法</remarks>
        public DbCommand CreateCommand(string text)
        {
            return CreateCommand(text, CommandType.Text);
        }

        #endregion

        #region public DbCommand CreateCommand(string text, CommandType type)

        /// <summary>
        /// 创建一个dbcommand 对象，connection对象自动释放
        /// </summary>
        /// <param name="text"> sql语句 或存储过程名</param>
        /// <param name="type">sql对象的类型：sql语句或存储过程</param>
        /// <returns>dbcommand对象新实例</returns>
        /// <remarks>dbcommand对象释放时调用connection释放的方法</remarks>
        public DbCommand CreateCommand(string text, CommandType type)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            DbConnection con = this.CreateConnection();
            DbCommand cmd = this.CreateCommand(con, text);
            cmd.CommandType = type;
            cmd.Disposed += new EventHandler(DisposeCommandConnection);

            return cmd;
        }

        #endregion

        ///
        ///创建带事务的，不自动关闭conn
        ///

        #region public DbCommand CreateCommand(DbTransaction transaction, string text)

        /// <summary>
        ///  创建一个带事务的dbcommand 对象，需手动释放connection
        /// </summary>
        /// <param name="transaction">事务对象</param>
        /// <param name="text">sql语句</param>
        /// <returns>dbcommand对象新实例</returns>      
        /// <remarks>需手动释放connection</remarks>
        public DbCommand CreateCommand(DbTransaction transaction, string text)
        {
            return CreateCommand(transaction, text, CommandType.Text);
        }

        #endregion

        #region public DbCommand CreateCommand(DbTransaction transaction, string text)

        /// <summary>
        /// 创建一个带事务的dbcommand 对象 支持存储过程或sql语句，需手动释放connection
        /// </summary>
        /// <param name="transaction">事务对象</param>
        /// <param name="text"> sql语句 或存储过程名</param>
        /// <param name="type">sql对象的类型：sql语句或存储过程</param>
        /// <returns>dbcommand对象新实例</returns>
        /// <remarks>需手动释放connection</remarks>
           public DbCommand CreateCommand(DbTransaction transaction, string text, CommandType type)
        {
            if (null == transaction)
                throw new ArgumentNullException("transaction");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            DbCommand cmd = this.Factory.CreateCommand();
            cmd.Connection = transaction.Connection;
            //内部事务检查
            if (transaction is DBInnerTransaction)
                cmd.Transaction = ((DBInnerTransaction)transaction).InnerTransaction;
            else
                cmd.Transaction = transaction;
            cmd.CommandText = text;
            cmd.CommandType = type;
            return cmd;
        }

        #endregion

        ///
        ///通过conn创建，不自动关闭conn
        ///

        #region public DbCommand CreateCommand(DbConnection connection)

        /// <summary>
        /// 通过给定的connection对象创建dbcommand对象，需手动释放connection
        /// </summary>
        /// <param name="connection">执行的connection对象</param>
        /// <returns>dbcommand对象新实例</returns>
        public DbCommand CreateCommand(DbConnection connection)
        {
            if (null == connection)
                throw new ArgumentNullException("connection");

            DbCommand cmd = this.Factory.CreateCommand();
            cmd.Connection = connection;
            return cmd;
        }

        #endregion

        #region public DbCommand CreateCommand(DbConnection connection, string text)

        /// <summary>
        /// 通过connection对象创建dbcommand，需手动释放connection
        /// </summary>
        /// <param name="connection">connection对象</param>
        /// <param name="text">sql语句</param>
        /// <returns>DbCommand新实例</returns>
        /// <remarks>需手动释放connection</remarks>
        public DbCommand CreateCommand(DbConnection connection, string text)
        {
            return CreateCommand(connection, text, CommandType.Text);
        }

        #endregion

        #region public virtual DbCommand CreateCommand(DbConnection connection, string text, CommandType type)

        /// <summary>
        /// 通过connection对象创建command对象，支持sql 或存储过程，需手动释放connection
        /// </summary>
        /// <param name="connection">connection对象</param>
        /// <param name="text">sql语句或存储过程名</param>
        /// <param name="type">执行类型：sql语句或存储过程</param>
        /// <returns>The initialized DbCommand</returns>
        /// <remarks>需手动释放connection</remarks>
        public virtual DbCommand CreateCommand(DbConnection connection, string text, CommandType type)
        {
            DbCommand cmd = this.Factory.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = text;
            cmd.CommandType = type;
            return cmd;
        }

        #endregion

        //
        // 私有的方法的实现部分
        //

        #region private static void DisposeCommandConnection(object sender, EventArgs e)

        /// <summary>
        /// 关闭dbcommmand的数据库连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DisposeCommandConnection(object sender, EventArgs e)
        {
            DbCommand cmd = (DbCommand)sender;
            if (cmd.Connection != null)
            {
                cmd.Connection.Dispose();
            }
        }

        #endregion   
    }
}
