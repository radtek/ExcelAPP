 

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
 
namespace ExcelDataBase
{
    /// <summary>
    /// DbCommand ����
    /// </summary>
    public partial class Database
    {
        ///
        ///�Զ�����conn ���Զ��ر�conn
        ///

        #region public DbCommand CreateCommand()

        /// <summary>
        /// ����һ��dbcommand����connection�����Զ��ͷ�
        /// </summary>
        /// <returns>dbcommand������ʵ��</returns>
        /// <remarks>dbcommand�����ͷ�ʱ����connection�ͷŵķ���</remarks>
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
        /// ����һ��dbcommand ����connection�����Զ��ͷ�
        /// </summary>
        /// <param name="text">Sql</param>
        /// <returns>dbcommand������ʵ��</returns>
        /// <remarks>dbcommand�����ͷ�ʱ����connection�ͷŵķ���</remarks>
        public DbCommand CreateCommand(string text)
        {
            return CreateCommand(text, CommandType.Text);
        }

        #endregion

        #region public DbCommand CreateCommand(string text, CommandType type)

        /// <summary>
        /// ����һ��dbcommand ����connection�����Զ��ͷ�
        /// </summary>
        /// <param name="text"> sql��� ��洢������</param>
        /// <param name="type">sql��������ͣ�sql����洢����</param>
        /// <returns>dbcommand������ʵ��</returns>
        /// <remarks>dbcommand�����ͷ�ʱ����connection�ͷŵķ���</remarks>
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
        ///����������ģ����Զ��ر�conn
        ///

        #region public DbCommand CreateCommand(DbTransaction transaction, string text)

        /// <summary>
        ///  ����һ���������dbcommand �������ֶ��ͷ�connection
        /// </summary>
        /// <param name="transaction">�������</param>
        /// <param name="text">sql���</param>
        /// <returns>dbcommand������ʵ��</returns>      
        /// <remarks>���ֶ��ͷ�connection</remarks>
        public DbCommand CreateCommand(DbTransaction transaction, string text)
        {
            return CreateCommand(transaction, text, CommandType.Text);
        }

        #endregion

        #region public DbCommand CreateCommand(DbTransaction transaction, string text)

        /// <summary>
        /// ����һ���������dbcommand ���� ֧�ִ洢���̻�sql��䣬���ֶ��ͷ�connection
        /// </summary>
        /// <param name="transaction">�������</param>
        /// <param name="text"> sql��� ��洢������</param>
        /// <param name="type">sql��������ͣ�sql����洢����</param>
        /// <returns>dbcommand������ʵ��</returns>
        /// <remarks>���ֶ��ͷ�connection</remarks>
           public DbCommand CreateCommand(DbTransaction transaction, string text, CommandType type)
        {
            if (null == transaction)
                throw new ArgumentNullException("transaction");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            DbCommand cmd = this.Factory.CreateCommand();
            cmd.Connection = transaction.Connection;
            //�ڲ�������
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
        ///ͨ��conn���������Զ��ر�conn
        ///

        #region public DbCommand CreateCommand(DbConnection connection)

        /// <summary>
        /// ͨ��������connection���󴴽�dbcommand�������ֶ��ͷ�connection
        /// </summary>
        /// <param name="connection">ִ�е�connection����</param>
        /// <returns>dbcommand������ʵ��</returns>
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
        /// ͨ��connection���󴴽�dbcommand�����ֶ��ͷ�connection
        /// </summary>
        /// <param name="connection">connection����</param>
        /// <param name="text">sql���</param>
        /// <returns>DbCommand��ʵ��</returns>
        /// <remarks>���ֶ��ͷ�connection</remarks>
        public DbCommand CreateCommand(DbConnection connection, string text)
        {
            return CreateCommand(connection, text, CommandType.Text);
        }

        #endregion

        #region public virtual DbCommand CreateCommand(DbConnection connection, string text, CommandType type)

        /// <summary>
        /// ͨ��connection���󴴽�command����֧��sql ��洢���̣����ֶ��ͷ�connection
        /// </summary>
        /// <param name="connection">connection����</param>
        /// <param name="text">sql����洢������</param>
        /// <param name="type">ִ�����ͣ�sql����洢����</param>
        /// <returns>The initialized DbCommand</returns>
        /// <remarks>���ֶ��ͷ�connection</remarks>
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
        // ˽�еķ�����ʵ�ֲ���
        //

        #region private static void DisposeCommandConnection(object sender, EventArgs e)

        /// <summary>
        /// �ر�dbcommmand�����ݿ�����
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
