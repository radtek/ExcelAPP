﻿using ExcelAPPWeb.Model;
using ExcelAPPWeb.SPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;

namespace ExcelAPPWeb
{
    public class TestExtend : IExcelExtend
    {

        /// <summary>
        /// 删除关联后
        /// </summary>
        /// <param name="list"></param>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="transaction"></param>

        public void AfterRefDelete(List<IDictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 关联上传后
        /// </summary>
        /// <param name="list"></param>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        public void AfterRefUpload(List<IDictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
        {
            throw new Exception("不能关联上传" + model.ID);
        }


        /// <summary>
        /// 取消上传后
        /// </summary>
        /// <param name="list">数据信息 为list 集合 类似于datatable 比dt 轻量</param>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        public void AfterCancel(List<IDictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
        {


            var info = db.Query<dynamic>("select  * from lsbzdw where  rownum=1 ", transaction: transaction).ToList();
            var diclist = info.Select(x => (IDictionary<string, object>)x).ToList();
            throw new Exception("不能取消上传" + model.ID + "单位数据" + Newtonsoft.Json.JsonConvert.SerializeObject(diclist));
        }


        /// <summary>
        /// 确认上传后
        /// </summary>
        /// <param name="list"></param>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        public void AfterUpload(List<IDictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
        {
            //var count = db.ExecuteScalar<int>("select count(*) from lsbzdw ", transaction: transaction);
            //throw new Exception("不能上传" + model.ID + "单位个数" + count.ToString());
            /* var info = db.Query<dynamic>("select  * from lsbzdw where  rownum=1 ", transaction: transaction).ToList();
             var diclist = info.Select(x => (IDictionary<string, object>)x).ToList();
             new ExcelAPPWeb.Service.ImportService().WriteLogFile("获取" + Newtonsoft.Json.JsonConvert.SerializeObject(new { Bills = list }));
             throw new Exception("不能取消上传" + model.ID + "单位数据" + Newtonsoft.Json.JsonConvert.SerializeObject(diclist));
             */
            try
            {
             var YWID = model.ID.Replace(model.DWBH,"");
             new YzDrpInterFace().DrpIMInterFace(YWID, list, model, db, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 自定义按钮点击后
        /// </summary>
        /// <param name="list"></param>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        public void CustomBtnClick(List<IDictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}