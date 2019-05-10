using ExcelAPPWeb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcelAPPWeb
{
    /// <summary>
    /// help 的摘要说明
    /// </summary>
    public class help : IHttpHandler
    {


        /// <summary>
        /// 帮助逻辑
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {


            try
            {
                var service = new CustomHelper();
                context.Response.ContentType = "text/plain";

                var op = context.Request.Form.Get("op");
                object res = "";
                switch (op)
                {
                    case "GetHelpInfo":
                        var id = context.Request.Form.Get("id");
                        res = service.GetHelpInfo(id);
                        break;
                    case "GetHelpData":
                    case "GetHelpDataSearch":
                        var helpID = context.Request.Form.Get("id");
                        var filter = context.Request.Form.Get("filter");
                        var order = context.Request.Form.Get("order");
                        var row = context.Request.Form.Get("row");
                        var dwbh = context.Request.Form.Get("dwbh");
                        var page = int.Parse(context.Request.Form.Get("page"));
                        var pageSize = int.Parse(context.Request.Form.Get("pageSize"));
                        res = service.GetDataList(helpID, filter, order, page, pageSize);
                        break;
                    case "GetFunc":

                        res = service.GetFuncList();
                        break;
                    default:
                        break;
                }
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { res = true, data = res }));
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