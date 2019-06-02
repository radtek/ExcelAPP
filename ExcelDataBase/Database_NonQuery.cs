 

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
 
namespace ExcelDataBase
{
    /// <summary>
    /// �����޷���ֵ����
    /// </summary>
    public partial class Database
    {


        //
        //�����޷���ֵ,�Զ��������ر�conn
        //              

        #region public int ExecuteNonQuery(string text)

        /// <summary>
        /// ִ��sql �Զ��������ر�conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>����Ӱ������</returns>
        public int ExecuteNonQuery(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQuery(sql, CommandType.Text, null);
        }

        #endregion

        #region public int ExecuteNonQuery(string text, DbParam[] param)

        /// <summary>
        /// ִ�д�������sql,�Զ��������ر�conn
        /// </summary>
        /// <param name="sql">sql �� �洢������</param>
        /// <param name="param">����</param>
        /// <returns>����Ӱ������</returns>
        public int ExecuteNonQuery(string sql, DbParam[] param)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQuery(sql, CommandType.StoredProcedure, param);
        }
        #endregion

        #region public int ExecuteNonQuery(string sql,CommandType type, DbParam[] param)
        /// <summary>
        /// ִ�д�������sql,�Զ��������ر�conn
        /// </summary>
        /// <param name="sql">sql �� �洢������</param>
        /// <param name="type">ִ������</param>
        /// <param name="param">����</param>
        /// <returns>����Ӱ������</returns>
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
        ///ִ�д�������sql,�Զ��������ر�conn
        /// </summary>
        /// <param name="cmd">  command ����</param>
        /// <returns>����Ӱ������</returns>
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
        ///  ִ��sql,�Զ��������ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <returns>����Ӱ������</returns>
        protected virtual int DoExecuteNonQuery(DbCommand cmd)
        {
            int result = 0;
            bool opened = false;
            
            try
            {
                //sql ��洢����������
                dealCMD(cmd);
                //�����ݿ�����
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
        //�����޷���ֵ,֧������,���ر�conn
        //              

        #region public int ExecuteNonQueryTrans(string text, DbTransaction trans)

        /// <summary>
        /// ִ��sql,֧������,���ر�conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="trans">����</param>
        /// <returns>����Ӱ������</returns>
        public int ExecuteNonQueryTrans(string sql, DbTransaction trans)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQueryTrans(sql, CommandType.Text, null,trans);
        }

        #endregion

        #region public int ExecuteNonQueryTrans(string text, DbParam[] param, DbTransaction trans)

        /// <summary>
        /// ִ�д�������sql,֧������,���ر�conn
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="param">����</param>
        /// <param name="trans">����</param>
        /// <returns>����Ӱ������</returns>
        public int ExecuteNonQueryTrans(string sql, DbParam[] param, DbTransaction trans)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQueryTrans(sql, CommandType.StoredProcedure, param,trans);
        }
        #endregion

        #region public int ExecuteNonQueryTrans(string sql,CommandType type, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// ִ�д�������sql,֧������,���ر�conn
        /// </summary>
        /// <param name="sql">sql �� �洢������</param>
        /// <param name="type">ִ������</param>
        /// <param name="param">����</param>
        /// <param name="trans">����</param>
        /// <returns>����Ӱ������</returns>
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
        ///ִ��sql,֧������,���ر�conn
        /// </summary>
        /// <param name="cmd">  command ����</param>
        /// <returns>����Ӱ������</returns>
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
        ///  ִ��sql,֧������,���ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <returns>����Ӱ������</returns>
        protected virtual int DoExecuteNonQueryTrans(DbCommand cmd)
        {

            //sql ��洢����������
            dealCMD(cmd);
            //�����ݿ�����
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            } 
            return  cmd.ExecuteNonQuery();

        }

        #endregion


        ///
        ///�����޷���ֵ,��conn����command,���ر�conn
        ///              

        #region public int ExecuteNonQueryConn(string text, DbConnection conn)

        /// <summary>
        /// ִ��sql,��conn����command,���ر�conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns>����Ӱ������</returns>
        public int ExecuteNonQueryConn(string sql, DbConnection conn)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQueryConn(sql, CommandType.Text, null, conn);
        }

        #endregion

        #region public int ExecuteNonQueryConn(string text, DbParam[] param, DbConnection conn)

        /// <summary>
        /// ִ�д�������sql,��conn����command,���ر�conn
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="param">����</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns>����Ӱ������</returns>
        public int ExecuteNonQueryConn(string sql, DbParam[] param, DbConnection conn)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            return this.ExecuteNonQueryConn(sql, CommandType.StoredProcedure, param, conn);
        }
        #endregion

        #region public int ExecuteNonQueryConn(string sql,CommandType type, DbParam[] param, DbConnection conn)
        /// <summary>
        /// ִ�д�������sql,��conn����command,���ر�conn
        /// </summary>
        /// <param name="sql">sql �� �洢������</param>
        /// <param name="type">ִ������</param>
        /// <param name="param">����</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns>����Ӱ������</returns>
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
        ///ִ��sql,��conn����command,���ر�conn
        /// </summary>
        /// <param name="cmd">  command ����</param>
        /// <returns>����Ӱ������</returns>
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
        ///  ִ��sql,��conn����command,���ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <returns>����Ӱ������</returns>
        protected virtual int DoExecuteNonQueryConn(DbCommand cmd)
        {

            //sql ��洢����������
            dealCMD(cmd);
            //�����ݿ�����
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            } 
            return cmd.ExecuteNonQuery();

        }

        #endregion
    }
}
