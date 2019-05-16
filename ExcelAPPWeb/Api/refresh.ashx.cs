using ExcelAPPWeb.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcelAPPWeb.Api
{
    /// <summary>
    /// refresh 的摘要说明
    /// </summary>
    public class refresh : IHttpHandler
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

                var data = dsvr.GetDataList(model, type, order, int.Parse(pageIndex), int.Parse(pageSize), startDate, endDate);
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(data, Formatting.Indented, timeFormat));
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