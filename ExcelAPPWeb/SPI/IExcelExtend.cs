using NPoco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelAPPWeb.SPI
{
    /// <summary>
    /// 上传扩展接口
    /// </summary>
    public interface IExcelExtend
    {
        void AfterRefDelete(List<IDictionary<string, object>> list, Model.EACmpCategory model, IDbConnection db, IDbTransaction transaction);


        void AfterRefUpload(List<IDictionary<string, object>> list, Model.EACmpCategory model, IDbConnection db, IDbTransaction transaction);

        void AfterUpload(List<IDictionary<string, object>> list, Model.EACmpCategory model, IDbConnection db, IDbTransaction transaction);



        void AfterCancel(List<IDictionary<string, object>> list, Model.EACmpCategory model, IDbConnection db, IDbTransaction transaction);

        void CustomBtnClick(List<IDictionary<string, object>> list, Model.EACmpCategory model, IDbConnection db, IDbTransaction transaction);

    }
}
