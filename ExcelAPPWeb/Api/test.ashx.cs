
using ExcelAPPWeb.Dappper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace ExcelAPPWeb.Api
{
    /// <summary>
    /// test 的摘要说明
    /// </summary>
    public class test : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //Database Db = new Database("DataPlatformDB");

            //var Re_CURSOR = new OracleParameter("@Re_CURSOR", OracleDbType.RefCursor);
            //Re_CURSOR.Direction = ParameterDirection.Output;
            //Re_CURSOR.Size = 0;



            //var COUNT_CURSOR = new OracleParameter("@COUNT_CURSOR", OracleDbType.RefCursor);
            //Re_CURSOR.Direction = ParameterDirection.Output;
            //Re_CURSOR.Size = 0;

            //Sql result = Sql.Builder.Append("exec FASTDWEB_PAGELIST @sqlQuery, @page, @pageSize, @orderStr", new
            //{
            //    sqlQuery = "select * from lsbzdw",
            //    page = 1,
            //    pageSize = 10,
            //    orderStr = ""
            //});


            //Sql result1 = Sql.Builder.Append("call GSIDPPUB.PAGEPROC(@sqlQuery, @pageSize, @pageIndex, @total,@Re_CURSOR,@COUNT_CURSOR)", new
            //{
            //    sqlQuery = "select * from lsbzdw",
            //    pageSize = 1,
            //    pageIndex = 1,
            //    total = 0,
            //    Re_CURSOR,
            //    COUNT_CURSOR,
            //});

            Dictionary<string, List<IDictionary<string, object>>> dataset = new Dictionary<string, List<IDictionary<string, object>>>();

            using (var cnn = new OracleConnection(ConfigurationManager.ConnectionStrings["DataPlatformDB"].ConnectionString))
            {
                var p = new OracleDynamicParameters();
                p.Add("sqlQuery", "select * from lsbzdw");
                p.Add("pageSize", 10);
                p.Add("pageIndex", 1);
                p.Add("total", 0);
                p.Add("Re_CURSOR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                p.Add("COUNT_CURSOR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var list = cnn.QueryMultiple("GSIDPPUB.PAGEPROC", param: p, commandType: CommandType.StoredProcedure);

                var i = 1;
                while (!list.IsConsumed)
                {
                    //读取当前结果集
                    var result = list.Read().ToList();
                    if (result != null)
                    {
                        var info = result.Select(x => (IDictionary<string, object>)x).ToList();
                        var key = i.ToString();
                        dataset.Add(key, info);
                        i++;

                    }

                }
                // var list = cnn.Query(" GSIDPPUB.PAGEPROC", param: p, commandType: CommandType.StoredProcedure).ToList<>();
                //var res = list.Select(x => (IDictionary<string, object>)x).ToList();
            }

            //////var list = Db.Fetch<Dictionary<string, object>>(new Sql("select * from lsbzdw "));

            ////var list = Db.Fetch<Dictionary<string, object>>(result1);

            ////;
            ////context.Response.Write(list.Count);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}