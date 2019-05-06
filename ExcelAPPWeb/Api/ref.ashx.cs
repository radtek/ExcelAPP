using ExcelAPPWeb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcelAPPWeb.Api
{
    /// <summary>
    /// _ref 的摘要说明
    /// </summary>
    public class _ref : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";

                EARuleService svr = new EARuleService();
                ImportService dsvr = new ImportService();
                var pageSize = context.Request.Form.Get("pagesize");
                var pageIndex = context.Request.Form.Get("page");
                var type = context.Request.Form.Get("flag");
                var startDate = context.Request.Form.Get("start");
                var endDate = context.Request.Form.Get("end");

                var order = "";
                if (!string.IsNullOrEmpty(context.Request.Form.Get("sortname")))
                {
                    order = " order by " + context.Request.Form.Get("sortname") + " " + context.Request.Form.Get("sortorder");
                }
                var ruleID = context.Request.Form.Get("id");

                var model = svr.GetRuleInfo(ruleID);

                var data = dsvr.GetDataREFList(model, type, order, int.Parse(pageIndex), int.Parse(pageSize), startDate, endDate);

                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { res = false, msg = ex.Message }));

            }
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