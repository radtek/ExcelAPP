using NPoco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelApp.Service
{
    public class CustomHelper
    {
        public Database Db = new Database("DataPlatformDB");

        /// <summary>
        /// 获取用户默认单位信息
        /// </summary>
        /// <returns></returns>
        public string GetUserDefaultDW()
        {
            return "";
        }


        //获取单位定义的模板类别
        public static DataTable GetTmpDWInfo(string dwbh)
        {

            return new DataTable();
        }



        /// <summary>
        /// 分页获取帮助数据
        /// </summary>
        /// <param name="HelpId"></param>
        /// <param name="filter"></param>
        /// <param name="Page"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public object GetDataList(string HelpId, string filter, string orderBy, int Page, int Size)
        {



            var model = this.GetHelpInfo(HelpId);
            var table = model.CODE;//表名称
            var fields = string.IsNullOrEmpty(model.SFields) ? (table + ".*") : model.SFields;
            var sql = $"select {fields} from {table} where 1=1 {model.SFilter}  {filter}";
            var result = Db.Page<Dictionary<string, object>>(Page, Size, new Sql(sql));

            return new
            {
                Rows = result.Items,
                Total = result.TotalItems
            };
        }

        /// <summary>
        /// 获取帮助配置信息
        /// </summary>
        /// <param name="HelpId"></param>
        /// <returns></returns>
        public Model.EAHelp GetHelpInfo(string HelpId)
        {
            //Sql sql = new Sql(@"select * from FBDataSource  where  ID=@0", helpid);
            Model.EAHelp model = Db.SingleById<Model.EAHelp>(HelpId);
            return model;
        }
    }
}
