

using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
 

namespace ExcelDataBase
{
    /// <summary>
    /// ִ��datareader����
    /// </summary>
    public partial class Database
    {
        ///
        ///�����������
        ///

        #region public delegate object DBCallback(System.Data.Common.DbDataReader reader);
        /// <summary>
        /// ִ��datareader�ķ���
        /// </summary>
        /// <param name="reader">datareader����</param>
        /// <returns>object����</returns>
        public delegate object DBCallback(System.Data.Common.DbDataReader reader);
        #endregion
        
        ///
        ///ִ��DataReader ����Dictionary �Զ�����conn���Զ��ر�conn
        /// 

        #region public Dictionary<string,object> ExecuteReadDict(string sql)
        /// <summary>
        /// ����Dictionary �Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>dict����</returns>
        public Dictionary<string,object> ExecuteReadDict(string sql)
        {
            return this.ExecuteReadDict(sql, CommandType.Text, null);

        }
        #endregion

        #region public Dictionary<string, object> ExecuteReadDict(string sql,DbParam[] param)
        /// <summary>
        /// ����Dictionary �Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="param">����</param>
        /// <returns>dict����</returns>
        public Dictionary<string, object> ExecuteReadDict(string sql,DbParam[] param)
        {
            return this.ExecuteReadDict(sql, CommandType.StoredProcedure, param);

        }
        #endregion

        #region public Dictionary<string,object> ExecuteReadDict(string sql, CommandType type, DbParam[] param)
        /// <summary>
        /// ����Dictionary �Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="sql">sql��洢������</param>
        /// <param name="type">ִ������</param>
        /// <param name="param">����</param>
        /// <returns>dict����</returns>
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
        ///ִ��DataReader ����object ��Ҫ������ã��Զ�����conn���Զ��ر�conn
        ///

        #region public object ExecuteRead(string sqltext, DBCallback callback)

        /// <summary>
        /// ִ��datareader������sql��� �� delegate���ص���,�Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="sqltext">sql���</param>
        /// <param name="callback"> delegate ����</param>
        /// <returns>object���ͣ���������</returns>
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
        ///ִ�д洢���̻�sql��datareader,�Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="text">proc</param>
        /// <param name="param">����</param>
        /// <param name="callback">������</param>
        /// <returns>object��������</returns>
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
        ///ִ�д洢���̻�sql��datareader,�Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="text">sql ��proc</param>
        /// <param name="type">sql ��洢����</param>
        /// <param name="param">����</param>
        /// <param name="callback">������</param>
        /// <returns>object��������</returns>
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
        /// ͨ��cmdִ��datareader,�Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <param name="callback">������</param>
        /// <returns>object����</returns>
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
        /// ����ִ����,�Զ�����conn���Զ��ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <param name="callback">������</param>
        /// <returns>object����</returns>
        /// <remarks> </remarks>
        protected virtual object DoExecuteRead(DbCommand cmd, DBCallback callback)
        {
            object returns = null;
            DbDataReader reader = null;
            bool opened = false; 
            try
            {
                //����sql���
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
        ///ִ��DataReader ����Dictionary ֧������,���ر�conn
        /// 

        #region public Dictionary<string,object> ExecuteReadDictTrans(string sql, DbTransaction trans)
        /// <summary>
        /// ����Dictionary ֧������,���ر�conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="trans">����</param>
        /// <returns>dict����</returns>
        public Dictionary<string, object> ExecuteReadDictTrans(string sql, DbTransaction trans)
        {
            return this.ExecuteReadDictTrans(sql, CommandType.Text, null,trans);

        }
        #endregion

        #region public Dictionary<string, object> ExecuteReadDictTrans(string sql,DbParam[] param, DbTransaction trans)
        /// <summary>
        /// ����Dictionary֧������,���ر�conn
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="param">����</param>
        /// <param name="trans">����</param>
        /// <returns>dict����</returns>
        public Dictionary<string, object> ExecuteReadDictTrans(string sql, DbParam[] param, DbTransaction trans)
        {
            return this.ExecuteReadDictTrans(sql, CommandType.StoredProcedure, param,trans);

        }
        #endregion

        #region public Dictionary<string,object> ExecuteReadDictTrans(string sql, CommandType type, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// ����Dictionary֧������,���ر�conn
        /// </summary>
        /// <param name="sql">sql ��洢������</param>
        /// <param name="type">ִ������</param>
        /// <param name="param">����</param>
        /// <param name="trans">����</param>
        /// <returns>dict����</returns>
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
        ///ִ��DataReader ����object ֧��trans ��������
        ///

        #region public object ExecuteReadTrans(string sqltext, DbTransaction trans,DBCallback callback)

        /// <summary>
        /// ִ��datareader������sql��� �� delegate���ص���,���ر�conn
        /// </summary>
        /// <param name="sqltext">sql���</param>
        /// <param name="trans">����</param>
        /// <param name="callback"> delegate ����</param>
        /// <returns>object���ͣ���������</returns>
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
        ///ִ�д洢���̻�sql��datareader,���ر�conn
        /// </summary>
        /// <param name="text">�洢����</param>
        /// <param name="param">����</param>
        /// <param name="trans">����</param>
        /// <param name="callback">������</param>
        /// <returns>object��������</returns>
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
        ///ִ�д洢���̻�sql��datareader,���ر�conn
        /// </summary>
        /// <param name="text">sql ��proc</param>        
        /// <param name="type">sql ��洢����</param>
        /// <param name="param">����</param>
        /// <param name="trans">����</param>
        /// <param name="callback">������</param>
        /// <returns>object��������</returns>
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
        /// ͨ��cmdִ��datareader,���ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <param name="callback">������</param>
        /// <returns>object����</returns>
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
        /// ����ִ����,���ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <param name="callback">������</param>
        /// <returns>object����</returns>
        /// <remarks> </remarks>
        protected virtual object DoExecuteReadTrans(DbCommand cmd, DBCallback callback)
        {
            object returns = null;
            DbDataReader reader = null;
            //����sql��洢������
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
        ///ִ��DataReader ����Dictionary ��conn����command,���ر�conn
        /// 

        #region public Dictionary<string,object> ExecuteReadDictConn(string sql, DbConnection conn)
        /// <summary>
        /// ����Dictionary ��conn����command,���ر�conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns>dict����</returns>
        public Dictionary<string, object> ExecuteReadDictConn(string sql, DbConnection conn)
        {
            return this.ExecuteReadDictConn(sql, CommandType.Text, null, conn);

        }
        #endregion

        #region public Dictionary<string, object> ExecuteReadDictConn(string sql,DbParam[] param, DbConnection conn)
        /// <summary>
        /// ����Dictionary��conn����command,���ر�conn
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="param">����</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns>dict����</returns>
        public Dictionary<string, object> ExecuteReadDictConn(string sql, DbParam[] param, DbConnection conn)
        {
            return this.ExecuteReadDictConn(sql, CommandType.StoredProcedure, param, conn);

        }
        #endregion

        #region public Dictionary<string,object> ExecuteReadDictConn(string sql, CommandType type, DbParam[] param, DbConnection conn)
        /// <summary>
        /// ����Dictionary,��conn����command,���ر�conn
        /// </summary>
        /// <param name="sql">sql ��洢������</param>
        /// <param name="type">ִ������</param>
        /// <param name="param">����</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <returns>dict����</returns>
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
        ///ִ��DataReader ����object ֧��conn ��������
        ///

        #region public object ExecuteReadConn(string sqltext, DbConnection conn,DBCallback callback)

        /// <summary>
        /// ִ��datareader������sql��� �� delegate���ص���,���ر�conn
        /// </summary>
        /// <param name="sqltext">sql���</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <param name="callback"> delegate ����</param>
        /// <returns>object���ͣ���������</returns>
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
        ///ִ�д洢���̻�sql��datareader,���ر�conn
        /// </summary>
        /// <param name="text">�洢����</param>
        /// <param name="param">����</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <param name="callback">������</param>
        /// <returns>object��������</returns>
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
        ///ִ�д洢���̻�sql��datareader,���ر�conn
        /// </summary>
        /// <param name="text">sql ��proc</param>        
        /// <param name="type">sql ��洢����</param>
        /// <param name="param">����</param>
        /// <param name="conn">���ݿ����Ӷ���</param>
        /// <param name="callback">������</param>
        /// <returns>object��������</returns>
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
        /// ͨ��cmdִ��datareader,���ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <param name="callback">������</param>
        /// <returns>object����</returns>
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
        /// ����ִ����,���ر�conn
        /// </summary>
        /// <param name="cmd">cmd����</param>
        /// <param name="callback">������</param>
        /// <returns>object����</returns>
        /// <remarks> </remarks>
        protected virtual object DoExecuteReadConn(DbCommand cmd, DBCallback callback)
        {
            object returns = null;
            DbDataReader reader = null;
            //����sql��洢������
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
