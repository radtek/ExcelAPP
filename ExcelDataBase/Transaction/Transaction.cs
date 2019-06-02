using System;
using System.Collections.Generic;
using System.Transactions;
using System.Text;

namespace ExcelDataBase
{
    public class Transaction
    {
        /*
          int receiveCount = 0;
            TransactionOptions transactionOption = new TransactionOptions();

            //设置事务隔离级别
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;

            // 设置事务超时时间为60秒
            transactionOption.Timeout = new TimeSpan(0, 0, 60);

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption)) 
         */
        // private TransactionScope _transScope;
        //private bool _isDisposed;

        ///// <summary>
        ///// 构造函数。
        ///// </summary>
        //private GSPTransaction()
        //{
        //    _isDisposed = false;
        //}

        ///// <summary>
        ///// 获取事务管理对象。
        ///// </summary>
        ///// <returns>事务管理对象。</returns>
        //public static IGSPTransaction GetTransaction()
        //{
        //    return new GSPTransaction();
        //}

        ///// <summary>
        ///// 启动事务。
        ///// 该范围需要一个事务。如果已经存在环境事务，则使用该环境事务。否则，在进入范围之前创建新的事务。 
        ///// </summary>
        //public void BeginTransaction()
        //{
        //    this.BeginTransaction(TransactionScopeOption.Required, IsolationLevel.ReadCommitted);
        //}

        ///// <summary>
        ///// 启动一个新事务。
        ///// </summary>
        ///// <remarks>
        ///// 若环境中不存在事务，则其为事务的根，若环境存在事务，不于环境中的事务融合。
        ///// 默认级别为ReadCommited。
        ///// </remarks>
        //public void BeginRequireNewTransaction()
        //{
        //    this.BeginTransaction(TransactionScopeOption.RequiresNew, IsolationLevel.ReadCommitted);
        //}

        ///// <summary>
        ///// 启动一个不支持事务的环境。
        ///// </summary>
        ///// <remarks>在该环境内，永远不参与事务。</remarks>
        //public void BeginSuppression()
        //{
        //    this.BeginTransaction(TransactionScopeOption.Suppress, IsolationLevel.Unspecified);
        //}

        ///// <summary>
        ///// 启动事务。
        ///// </summary>
        ///// <param name="iso">事务隔离级别。</param>
        ///// <param name="scopeOption">事务块属性</param>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //private void BeginTransaction(TransactionScopeOption scopeOption, IsolationLevel iso)
        //{
        //    if (_transScope == null)
        //    {
        //        TransactionOptions options = new TransactionOptions();
        //        options.IsolationLevel = iso;
        //        if (iso != IsolationLevel.Unspecified)
        //        {
        //            _transScope = new TransactionScope(scopeOption, options);
        //        }
        //        else
        //        {
        //            _transScope = new TransactionScope(scopeOption);
        //        }
        //        this._isDisposed = false;
        //    }
        //    else
        //        throw new Exception("已启动事务环境！");
        //}

        ///// <summary>
        ///// 提交事务。
        ///// </summary>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public void SetComplete()
        //{
        //    if (this._isDisposed == false)
        //    {
        //        if (_transScope != null)
        //        {
        //            _transScope.Complete();
        //            _transScope.Dispose();
        //            this._transScope = null;
        //        }
        //        this._isDisposed = true;
        //    }
        //}

        ///// <summary>
        ///// 回滚事务。
        ///// </summary>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public void SetAbort()
        //{
        //    if (_transScope != null && this._isDisposed == false)
        //    {
        //        _transScope.Dispose();
        //        _transScope = null;
        //        this._isDisposed = true;
        //    }
        //}

        ///// <summary>
        ///// 结束事务范围。
        ///// </summary>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public void Dispose()
        //{
        //    if (_isDisposed == false)
        //    {
        //        if (_transScope != null)
        //        {
        //            _transScope.Dispose();
        //            _transScope = null;
        //        }
        //        this._isDisposed = true;
        //    }
        //}

        ///// <summary>
        ///// 启动事务，采用系列化读隔离级别
        ///// </summary>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public void BeginComPlusTransaction()
        //{
        //    if (_transScope == null)
        //    {
        //        TransactionOptions trans = new TransactionOptions();
        //        trans.IsolationLevel = System.Transactions.IsolationLevel.Serializable;
        //        _transScope =
        //                new TransactionScope(
        //                TransactionScopeOption.Required, trans, EnterpriseServicesInteropOption.Full);
        //    }
        //    else
        //        throw new Exception("已启动事务环境！");

        //}

        ///// <summary>
        ///// 启动事务，较多控制选项
        ///// </summary>
        ///// <param name="scopeOption">事务块属性</param>
        ///// <param name="iso">隔离级别</param>
        ///// <param name="interopOption">互操作选项</param>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public void BeginTransaction(TransactionScopeOption scopeOption, IsolationLevel iso, EnterpriseServicesInteropOption interopOption)
        //{
        //    if (_transScope == null)
        //    {
        //        TransactionOptions trans = new TransactionOptions();
        //        trans.IsolationLevel = System.Transactions.IsolationLevel.Serializable;
        //        _transScope =
        //                new TransactionScope(
        //                scopeOption, trans, interopOption);
        //    }
        //    else
        //        throw new Exception("已启动事务环境！");
        //}

        //#region IGSPTransaction 成员

        ///// <summary>
        ///// 启动事务，设定事务的超时时间。
        ///// 该范围需要一个事务。如果已经存在环境事务，则使用该环境事务。否则，在进入范围之前创建新的事务。 
        ///// </summary>
        ///// <param name="tranTimeOut">事务的超时时间(int类型)</param>
        //public void BeginTransaction(int tranTimeOut)
        //{
        //    TransactionOptions options = new TransactionOptions();
        //    options.IsolationLevel = IsolationLevel.ReadCommitted;
        //    if (tranTimeOut <= 0)
        //    {
        //        tranTimeOut = 360;
        //    }
        //    options.Timeout = new TimeSpan(0, 0, tranTimeOut);

        //    this.BeginTransaction(TransactionScopeOption.Required, options);
        //}

        ///// <summary>
        ///// 启动事务。
        ///// </summary>
        ///// <param name="scopeOption">事务隔离级别。</param>
        ///// <param name="options">事务块属性</param>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //private void BeginTransaction(TransactionScopeOption scopeOption, TransactionOptions options)
        //{
        //    if (_transScope == null)
        //    {
        //        if (options.IsolationLevel != IsolationLevel.Unspecified)
        //            _transScope = new TransactionScope(scopeOption, options);
        //        else
        //            _transScope = new TransactionScope(scopeOption);
        //        this._isDisposed = false;
        //    }
        //    else
        //        throw new Exception("已启动事务环境！");
        //}

        //#endregion
    }
}
