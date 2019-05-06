using ExcelAPPWeb.Model;
using ExcelAPPWeb.SPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ExcelAPPWeb
{
    public class TestExtend : IExcelExtend
    {
        public void AfterCancel(List<Dictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
        {
            throw new Exception("不能取消" + model.ID);
        }

        public void AfterRefDelete(List<Dictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public void AfterRefUpload(List<Dictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public void AfterUpload(List<Dictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public void BeforeCancel(List<Dictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public void CustomBtnClick(List<Dictionary<string, object>> list, EACmpCategory model, IDbConnection db, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}