using ExcelAPPWeb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcelAPPWeb
{
    /// <summary>
    /// rule 的摘要说明
    /// </summary>
    public class rule : IHttpHandler
    {

        /// <summary>
        /// 规则逻辑
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var service = new EARuleService();
                context.Response.ContentType = "text/plain";
                var GSDWBH = "";
                var op = context.Request.Form.Get("op");
                object res = "";
                switch (op)
                {
                    case "LoadConfig":
                        var dwbh = context.Request.Form.Get("dwbh");
                        var lbid = context.Request.Form.Get("lbid");
                        res = service.GetRuleInfo(dwbh, lbid);
                        break;
                    case "GetUserInfo"://用户信息
                        res = UserService.GetUser();
                        break;
                    case "SetCookie"://用户信息
                        GSDWBH = context.Request.Form.Get("dwbh");
                       // CookieHelper.DelCookie("GSDWBH");
                        CookieHelper.WriteCookie("GSDWBH", GSDWBH,3600);
                      
                        break;
                    case "GetCookie"://用户信息
                        GSDWBH = CookieHelper.GetCookie("GSDWBH");
                        if (string.IsNullOrEmpty(GSDWBH))
                        {
                            GSDWBH = "";
                        }
                        break;
                    default:
                        break;
                }
                if (op!= "GetCookie" && op != "SetCookie")
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { res = true, data = res }));
                else
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { res = true, msg = GSDWBH }));
            }
            catch (Exception ex)
            {
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { res = false, msg = ex.Message+ex.StackTrace }));

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