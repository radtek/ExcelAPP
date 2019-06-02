 

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
 
namespace ExcelDataBase
{
    /// <summary>
    /// Database 内部类 数据库事务
    /// </summary>
    public partial class Database
    {
        //
        // 开始事务
        //

        #region public DbTransaction BeginTransaction()

        /// <summary>
        /// 创建一个事务
        /// </summary>
        /// <returns>创建一个事务</returns>
        /// <remarks>在Dispose调用传值为true时，可以不用调用Rollback方法回滚，释放的时候根据状态自动判断释放回滚，并关闭connection对象。<br/>
        /// 示例：
        /// DbTransaction trans=db.BeginTransaction();
        /// try{
        ///     trans.Commit();
        /// }catch(Exception ex){
        /// //不用做回滚处理
        /// }finally{
        ///     trans.Dispose(true);//true 做回滚，并关闭connection对象
        /// }
        /// </remarks>
        public virtual DbTransaction BeginTransaction()
        {
            DbConnection con = this.CreateConnection();
            return new DBInnerTransaction(con);
        }

        #endregion

        //
        // 内部类 数据库事务
        //


        #region private class DBOwningTransaction : DbTransaction

        /// <summary>
        /// 数据库事务类
        /// </summary>
        private class DBInnerTransaction : DbTransaction
        {
            private DbConnection _con;
            private bool _opened;
            private bool _active;
            private DbTransaction _innerTrans;

            public DbTransaction InnerTransaction
            {
                get { return this._innerTrans; }
            }

            public DBInnerTransaction(DbConnection conn)
            {
                this._con = conn;
                if (_con == null)
                    throw new ArgumentException("Cannot create an transaction without a valid connection");
                if (_con.State == System.Data.ConnectionState.Closed)
                {
                    _con.Open();
                    _opened = true;
                }
                else
                    _opened = false;

                this._innerTrans = _con.BeginTransaction();
                this._active = true;
            }

            public override void Commit()
            {
                this._innerTrans.Commit();
                this._active = false;
            }

            protected override DbConnection DbConnection
            {
                get { return this._con; }
            }

            public override System.Data.IsolationLevel IsolationLevel
            {
                get { return this._innerTrans.IsolationLevel; }
            }

            public override void Rollback()
            {
                this._innerTrans.Rollback();
                this._active = false;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_innerTrans != null)
                    {                        
                        if (_active)
                            _innerTrans.Rollback();

                        _innerTrans.Dispose();
                    }
                    if (_opened)
                        _con.Close();
                }

                base.Dispose(disposing);
            }
        }

        #endregion


    }
}
