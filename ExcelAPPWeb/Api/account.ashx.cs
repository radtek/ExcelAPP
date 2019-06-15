using ExcelAPPWeb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcelAPPWeb
{
    /// <summary>
    /// account 的摘要说明
    /// </summary>
    public class account : IHttpHandler
    {

        /// <summary>
        /// 登录 注销接口
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                var usercode = context.Request.Form.Get("usercode");
                var pwd = context.Request.Form.Get("pwd");
                var msg = "";
                string userCode = "";
                string userId = "";
                var flag = UserService.CheckUser(usercode, pwd, out msg, out userId, out userCode);
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { res = flag, msg = msg, userCode, userId }));
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