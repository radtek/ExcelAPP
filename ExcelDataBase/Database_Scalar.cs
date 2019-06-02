

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
 
namespace ExcelDataBase
{
    /// <summary>
    /// �����ȡ��ֵ����
    /// </summary>
    public partial class Database
    {
        ///
        ///ֱ��ִ�У�ִ�н����ر�conn���Զ�����conn���Զ��ر�conn
        ///

        #region public object ExecuteScalar(string text)

        /// <summary>
        /// ��ȡ��ֵ�ĺ������Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="text">sql���</param>
        /// <returns>object���͵Ķ���</returns>
        public object ExecuteScalar(string text)
        {
            return this.ExecuteScalar(text, CommandType.Text,null);
        }

        #endregion

        #region public object ExecuteScalar(string sql, DbParam[] param)
        /// <summary>
        /// ִ�д洢���̷��ص�һֵ���Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="param">����</param>
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
        /// ��ȡ��ֵ�ĺ������Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="text">sql����洢������</param>
        /// <param name="type">ִ�����ͣ�sql��洢����</param>
        /// <param name="param">����</param>
        /// <returns>object����</returns>
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
        /// ��ȡ��ֵ�ĺ������Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="cmd">DbCommand����</param>
        /// <returns>object����</returns>
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
        ///  ִ�л�ȡ��ֵ�Ķ����Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="cmd">Dbcommand����</param>
        /// <returns>object����</returns>
        protected virtual object DoExecuteScalar(DbCommand cmd)
        {
            object result = null;
            bool opened = false; 
            //sql ��洢����������
            dealCMD(cmd);
            //ִ��
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
        ///����Ϊ����׼��,���ر�conn
        ///

        #region    public object ExecScalarTrans(string sql,DbTransaction trans)
        /// <summary>
        /// ִ��sql���񣬻�ȡ��һֵ,���ر�conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="trans">����</param>
        /// <returns></returns>
        public object ExecScalarTrans(string sql,DbTransaction trans)
        {
            return this.ExecScalarTrans(sql, CommandType.Text, null,trans);
            
        }
        #endregion

        #region  public object ExecScalarTrans(string sql, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// ִ������洢���̣���ȡ��һֵ,���ر�conn
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="param">����</param>
        /// <param name="trans">����</param>
        /// <returns>object����</returns>
        public object ExecScalarTrans(string sql, DbParam[] param, DbTransaction trans)
        {
            return this.ExecScalarTrans(sql, CommandType.StoredProcedure, param, trans);
        }
        #endregion
        
        #region public object ExecScalarTrans(string sql, CommandType type, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// ִ��sql��洢���̣������񣬻�ȡ��һֵ,���ر�conn
        /// </summary>
        /// <param name="sql">sql ��洢����</param>
        /// <param name="type">ִ������</param>
        /// <param name="param">����</param>
        /// <param name="trans">����</param>
        /// <returns>object����</returns>
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
        /// ִ�з��ص�ֵ��������,���ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <returns>object����</returns>
        public object DoExecScalarTrans(DbCommand cmd)
        { 
            //sql ��洢����������
            dealCMD(cmd);
            //��δ�����ݿ�����������ݿ�����
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            } 
            return cmd.ExecuteScalar();

        }
        #endregion

        ///
        ///��conn����command,���ر�conn
        ///

        #region    public object ExecScalarConn(string sql,DbConnection conn)
        /// <summary>
        /// ִ��sql����conn����command����ȡ��һֵ,���ر�conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns></returns>
        public object ExecScalarConn(string sql, DbConnection conn)
        {
            return this.ExecScalarConn(sql, CommandType.Text, null, conn);

        }
        #endregion

        #region  public object ExecScalarConn(string sql, DbParam[] param, DbConnection conn)
        /// <summary>
        /// ִ�д洢���̣���conn����command����ȡ��һֵ,���ر�conn
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="param">����</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns>object����</returns>
        public object ExecScalarConn(string sql, DbParam[] param, DbConnection conn)
        {
            return this.ExecScalarConn(sql, CommandType.StoredProcedure, param, conn);
        }
        #endregion

        #region public object ExecScalarConn(string sql, CommandType type, DbParam[] param, DbConnection conn)
        /// <summary>
        /// ִ��sql��洢���̣���conn����command����ȡ��һֵ,���ر�conn
        /// </summary>
        /// <param name="sql">sql ��洢����</param>
        /// <param name="type">ִ������</param>
        /// <param name="param">����</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns>object����</returns>
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
        /// ִ�з��ص�ֵ����conn����command,���ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <returns>object����</returns>
        public object DoExecScalarConn(DbCommand cmd)
        {
            //sql ��洢����������
            dealCMD(cmd);
            //��δ�����ݿ�����������ݿ�����
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            } 
            return cmd.ExecuteScalar();

        }
        #endregion

    }
}
