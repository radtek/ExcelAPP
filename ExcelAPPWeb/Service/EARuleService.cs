using ExcelAPPWeb.DB;
using NPoco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ExcelAPPWeb.Service
{

    /// <summary>
    /// 业务规则sevice
    /// </summary>
    public class EARuleService
    {
        public Database Db = new Database("DataPlatformDB");

        public IDbConnection conn = DataBaseManager.GetDbConnection();//这里用dapper是为了处理oracle游标类型

        /// <summary>
        /// 获取单位类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.EACmpCategory GetRuleInfo(string id)
        {
            Model.EACmpCategory model = Db.SingleById<Model.EACmpCategory>(id);
            model.Cols = Db.Fetch<Model.EACmpCateCols>(new Sql(
                @"select a.*,b.HelpID,b.IsShow,b.FType,b.IsReadOnly,b.SCols,b.RCols, b.BindCol,b.FLength,b.FPrec,b.IsMatch,b.MatchRule,b.IsReadOnly,b.IsRequire,b.HelpFitler from EACmpCateCols  a left join EACatCols  b on a.FCode=b.FCode and b.RID=@1  where a.RID=@0", model.ID, model.CID));
            model.Tmp = GetPublicInfo(model.CID);
            return model;
        }


        public Model.EACmpCategory GetRuleInfo(string dwbh, string cid)
        {
            var sql = new Sql("select * from EACmpCategory where DWBH=@0 and CID=@1", dwbh, cid);
            var list = Db.Fetch<Model.EACmpCategory>(sql); 
            if (list.Count > 0)
            {
                Model.EACmpCategory model = list[0];
                model.Cols = Db.Fetch<Model.EACmpCateCols>(new Sql("select a.*,b.BindCol,b.Width,b.HelpID,b.IsShow,b.FPrec, b.FLength,b.FType,b.IsReadOnly,b.SCols,b.RCols,b.IsMatch,b.MatchRule,b.IsRequire,b.HelpFitler from EACmpCateCols  a left join EACatCols  b on a.FCode=b.FCode and b.RID=@1  where a.RID=@0 order by a.SortOrder asc", model.ID, model.CID));
                model.Tmp = GetPublicInfo(model.CID);
                return model;
            }
            return null;
        }

        public Model.EACategory GetPublicInfo(string id)
        {
            Model.EACategory model = Db.SingleById<Model.EACategory>(id);
            if (model != null & model.ID != null)
                model.Cols = Db.Fetch<Model.EACatCols>(new Sql("select * from EACatCols where RID=@0", model.ID));

            return model;
        }
    }
}
