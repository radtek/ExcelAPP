

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common; 
 
namespace ExcelDataBase
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Database
    {
    

        /// <summary>
        /// ����DBDataAdapter����
        /// </summary>
        /// <returns></returns>
        public DbDataAdapter CreateDataAdapter()
        {
            DbDataAdapter ada = this.Factory.CreateDataAdapter();            
            return ada;
        }


        ///
        ///����dataset���Զ�����conn ִ�н����ر�conn
        ///

        #region public DataSet ExecuteDataSet(string text)

        /// <summary>
        /// ��ȡDataset ���ر�conn
        /// </summary>
        /// <param name="text">sql���</param>
        /// <returns>DataSet���͵Ķ���</returns>
        public DataSet ExecuteDataSet(string text)
        {
            return this.ExecuteDataSet(text, CommandType.Text, null);
        }

        #endregion

        #region public DataSet ExecuteDataSet(string sql, DbParam[] param)
        /// <summary>
        /// ��ȡdataset���ر�conn
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="param">����</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string sql, DbParam[] param)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            return this.ExecuteDataSet(sql, CommandType.StoredProcedure, param); ;
        }
        #endregion

        #region public DataSet ExecuteDataSet(string text, CommandType type,DbParam[] param)

        /// <summary>
        /// ��ȡdataset���ر�conn
        /// </summary>
        /// <param name="text">sql����洢������</param>
        /// <param name="type">ִ�����ͣ�sql��洢����</param>
        /// <param name="param">����</param>
        /// <returns>DataSet����</returns>
        public DataSet ExecuteDataSet(string text, CommandType type, DbParam[] param)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            DataSet ds=new DataSet();
            using (DbCommand cmd = this.CreateCommand(text, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }
                ds = this.ExecuteDataSet(cmd);
            }
            return ds;
        }

        #endregion

        #region public DataSet ExecuteDataSet(DbCommand cmd)

        /// <summary>
        /// ��ȡDataSet
        /// </summary>
        /// <param name="cmd">DbCommand����</param>
        /// <returns>DataSet����</returns>
        public DataSet ExecuteDataSet(DbCommand cmd)
        {
            if (null == cmd)
                throw new ArgumentNullException("cmd");

            if (null == cmd.Connection)
                throw new ArgumentNullException("cmd.Connection");

            DataSet ds = DoExecuteDataSet(cmd);

            return ds;
        }

        #endregion

        #region protected virtual DataSet DoExecuteDataSet(DbCommand cmd)

        /// <summary>
        ///  ��ȡDataSet
        /// </summary>
        /// <param name="cmd">Dbcommand����</param>
        /// <returns>DataSet����</returns>
        protected virtual DataSet DoExecuteDataSet(DbCommand cmd)
        {
            DataSet ds = new DataSet();
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
                using (DbDataAdapter ada = this.CreateDataAdapter())
                {
                    //if (cmd.GetType().Name == "OracleCommand" && cmd.CommandText.IndexOf(";")!=-1)
                    //{
                    //    cmd.CommandText =    cmd.CommandText ;
                    //}
                    ada.SelectCommand = cmd;
                    ada.Fill(ds);
                }
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
            return ds;
        }

        #endregion

        ///
        ///����Ϊ����׼��,���ر�conn
        ///

        #region    public DataSet ExecuteDataSetTrans(string sql,DbTransaction trans)
        /// <summary>
        /// ִ��sql���񣬻�ȡDataSet�����ر�conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="trans">����</param>
        /// <returns></returns>
        public DataSet ExecuteDataSetTrans(string sql, DbTransaction trans)
        {
            return this.ExecuteDataSetTrans(sql, CommandType.Text, null, trans);

        }
        #endregion

        #region  public DataSet ExecuteDataSetTrans(string sql, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// ִ������洢���̣���ȡDataSet�����ر�conn
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="param">����</param>
        /// <param name="trans">����</param>
        /// <returns>DataSet����</returns>
        public DataSet ExecuteDataSetTrans(string sql, DbParam[] param, DbTransaction trans)
        {
            return this.ExecuteDataSetTrans(sql, CommandType.StoredProcedure, param, trans);
        }
        #endregion

        #region public DataSet ExecuteDataSetTrans(string sql, CommandType type, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// ִ��sql��洢���̣������񣬻�ȡDataSet�����ر�conn
        /// </summary>
        /// <param name="sql">sql ��洢����</param>
        /// <param name="type">ִ������</param>
        /// <param name="param">����</param>
        /// <param name="trans">����</param>
        /// <returns>DataSet����</returns>
        public DataSet ExecuteDataSetTrans(string sql, CommandType type, DbParam[] param, DbTransaction trans)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("text");
            if (null == trans)
                throw new ArgumentNullException("trans");

            DataSet returns;
            using (DbCommand cmd = this.CreateCommand(trans, sql, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }
                returns = this.DoExecuteDataSetTrans(cmd);
            }
            return returns;
        }
        #endregion

        #region public DataSet DoExecuteDataSetTrans(DbCommand cmd)
        /// <summary>
        /// ��ȡDataSet��������
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <returns>DataSet����</returns>
        public DataSet DoExecuteDataSetTrans(DbCommand cmd)
        {
            //sql ��洢����������
            dealCMD(cmd);
            //��δ�����ݿ�����������ݿ�����
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            }
            DataSet ds = new DataSet(); 
            using (DbDataAdapter ada = this.CreateDataAdapter())
            {
                ada.SelectCommand = cmd;
                ada.Fill(ds);
            }
            return ds;

        }
        #endregion

        ///
        ///��conn����command,���ر�conn
        ///

        #region    public DataSet ExecuteDataSetConn(string sql,DbConnection conn)
        /// <summary>
        /// ָ�����ݿ����Ӷ��󣬻�ȡDataSet�����ر�conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns></returns>
        public DataSet ExecuteDataSetConn(string sql, DbConnection conn)
        {
            return this.ExecuteDataSetConn(sql, CommandType.Text, null, conn);

        }
        #endregion

        #region  public DataSet ExecuteDataSetConn(string sql, DbParam[] param, DbConnection conn)
        /// <summary>
        ///  ָ�����ݿ����Ӷ��󣬻�ȡDataSet�����ر�conn
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="param">����</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns>DataSet����</returns>
        public DataSet ExecuteDataSetConn(string sql, DbParam[] param, DbConnection conn)
        {
            return this.ExecuteDataSetConn(sql, CommandType.StoredProcedure, param, conn);
        }
        #endregion

        #region public DataSet ExecuteDataSetConn(string sql, CommandType type, DbParam[] param, DbConnection conn)
        /// <summary>
        /// ִ��sql��洢���̣������ݿ����Ӷ��󣬻�ȡDataSet�����ر�conn
        /// </summary>
        /// <param name="sql">sql ��洢����</param>
        /// <param name="type">ִ������</param>
        /// <param name="param">����</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns>DataSet����</returns>
        public DataSet ExecuteDataSetConn(string sql, CommandType type, DbParam[] param, DbConnection conn)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("text");
            if (null == conn)
                throw new ArgumentNullException("conn");

            DataSet returns;
            using (DbCommand cmd = this.CreateCommand(conn, sql, type))
            {
                cmd.CommandType = type;
                if (param != null)
                {
                    cmd.CommandType = param[0].sqlType == SqlType.Sql ? CommandType.Text : CommandType.StoredProcedure;
                    IDataParameter[] iparam = this.ConvertParams(param);
                    cmd.Parameters.AddRange(iparam);
                }
                returns = this.DoExecuteDataSetConn(cmd);
            }
            return returns;
        }
        #endregion

        #region public DataSet DoExecuteDataSetConn(DbCommand cmd)
        /// <summary>
        /// ��ȡDataSet�������ݿ����Ӷ���,���ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <returns>DataSet����</returns>
        public DataSet DoExecuteDataSetConn(DbCommand cmd)
        {
            //sql ��洢����������
            dealCMD(cmd);
            //��δ�����ݿ�����������ݿ�����
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            } 
            DataSet ds = new DataSet();
            using (DbDataAdapter ada = this.CreateDataAdapter())
            {
                ada.SelectCommand = cmd;
                ada.Fill(ds);
            }
            return ds;

        }
        #endregion

       
        
    }
}
