

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
        /// 创建DBDataAdapter对象
        /// </summary>
        /// <returns></returns>
        public DbDataAdapter CreateDataAdapter()
        {
            DbDataAdapter ada = this.Factory.CreateDataAdapter();            
            return ada;
        }


        ///
        ///返回dataset，自动创建conn 执行结束关闭conn
        ///

        #region public DataSet ExecuteDataSet(string text)

        /// <summary>
        /// 获取Dataset ，关闭conn
        /// </summary>
        /// <param name="text">sql语句</param>
        /// <returns>DataSet类型的对象</returns>
        public DataSet ExecuteDataSet(string text)
        {
            return this.ExecuteDataSet(text, CommandType.Text, null);
        }

        #endregion

        #region public DataSet ExecuteDataSet(string sql, DbParam[] param)
        /// <summary>
        /// 获取dataset，关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
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
        /// 获取dataset，关闭conn
        /// </summary>
        /// <param name="text">sql语句或存储过程名</param>
        /// <param name="type">执行类型：sql或存储过程</param>
        /// <param name="param">参数</param>
        /// <returns>DataSet类型</returns>
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
        /// 获取DataSet
        /// </summary>
        /// <param name="cmd">DbCommand对象</param>
        /// <returns>DataSet类型</returns>
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
        ///  获取DataSet
        /// </summary>
        /// <param name="cmd">Dbcommand对象</param>
        /// <returns>DataSet类型</returns>
        protected virtual DataSet DoExecuteDataSet(DbCommand cmd)
        {
            DataSet ds = new DataSet();
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
        ///下面为事务准备,不关闭conn
        ///

        #region    public DataSet ExecuteDataSetTrans(string sql,DbTransaction trans)
        /// <summary>
        /// 执行sql事务，获取DataSet，不关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        public DataSet ExecuteDataSetTrans(string sql, DbTransaction trans)
        {
            return this.ExecuteDataSetTrans(sql, CommandType.Text, null, trans);

        }
        #endregion

        #region  public DataSet ExecuteDataSetTrans(string sql, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// 执行事务存储过程，获取DataSet，不关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <param name="trans">事务</param>
        /// <returns>DataSet对象</returns>
        public DataSet ExecuteDataSetTrans(string sql, DbParam[] param, DbTransaction trans)
        {
            return this.ExecuteDataSetTrans(sql, CommandType.StoredProcedure, param, trans);
        }
        #endregion

        #region public DataSet ExecuteDataSetTrans(string sql, CommandType type, DbParam[] param, DbTransaction trans)
        /// <summary>
        /// 执行sql或存储过程，带事务，获取DataSet，不关闭conn
        /// </summary>
        /// <param name="sql">sql 或存储过程</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <param name="trans">事务</param>
        /// <returns>DataSet对象</returns>
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
        /// 获取DataSet，带事务
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <returns>DataSet对象</returns>
        public DataSet DoExecuteDataSetTrans(DbCommand cmd)
        {
            //sql 或存储过程名处理
            dealCMD(cmd);
            //如未打开数据库连接则打开数据库连接
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
        ///由conn创建command,不关闭conn
        ///

        #region    public DataSet ExecuteDataSetConn(string sql,DbConnection conn)
        /// <summary>
        /// 指定数据库连接对象，获取DataSet，不关闭conn
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns></returns>
        public DataSet ExecuteDataSetConn(string sql, DbConnection conn)
        {
            return this.ExecuteDataSetConn(sql, CommandType.Text, null, conn);

        }
        #endregion

        #region  public DataSet ExecuteDataSetConn(string sql, DbParam[] param, DbConnection conn)
        /// <summary>
        ///  指定数据库连接对象，获取DataSet，不关闭conn
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">参数</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns>DataSet对象</returns>
        public DataSet ExecuteDataSetConn(string sql, DbParam[] param, DbConnection conn)
        {
            return this.ExecuteDataSetConn(sql, CommandType.StoredProcedure, param, conn);
        }
        #endregion

        #region public DataSet ExecuteDataSetConn(string sql, CommandType type, DbParam[] param, DbConnection conn)
        /// <summary>
        /// 执行sql或存储过程，带数据库连接对象，获取DataSet，不关闭conn
        /// </summary>
        /// <param name="sql">sql 或存储过程</param>
        /// <param name="type">执行类型</param>
        /// <param name="param">参数</param>
        /// <param name="conn">数据库连接对象</param>
        /// <returns>DataSet对象</returns>
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
        /// 获取DataSet，带数据库连接对象,不关闭conn
        /// </summary>
        /// <param name="cmd">cmd对象</param>
        /// <returns>DataSet对象</returns>
        public DataSet DoExecuteDataSetConn(DbCommand cmd)
        {
            //sql 或存储过程名处理
            dealCMD(cmd);
            //如未打开数据库连接则打开数据库连接
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
