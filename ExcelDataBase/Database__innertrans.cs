 

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
 
namespace ExcelDataBase
{
    /// <summary>
    /// Database �ڲ��� ���ݿ�����
    /// </summary>
    public partial class Database
    {
        //
        // ��ʼ����
        //

        #region public DbTransaction BeginTransaction()

        /// <summary>
        /// ����һ������
        /// </summary>
        /// <returns>����һ������</returns>
        /// <remarks>��Dispose���ô�ֵΪtrueʱ�����Բ��õ���Rollback�����ع����ͷŵ�ʱ�����״̬�Զ��ж��ͷŻع������ر�connection����<br/>
        /// ʾ����
        /// DbTransaction trans=db.BeginTransaction();
        /// try{
        ///     trans.Commit();
        /// }catch(Exception ex){
        /// //�������ع�����
        /// }finally{
        ///     trans.Dispose(true);//true ���ع������ر�connection����
        /// }
        /// </remarks>
        public virtual DbTransaction BeginTransaction()
        {
            DbConnection con = this.CreateConnection();
            return new DBInnerTransaction(con);
        }

        #endregion

        //
        // �ڲ��� ���ݿ�����
        //


        #region private class DBOwningTransaction : DbTransaction

        /// <summary>
        /// ���ݿ�������
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
