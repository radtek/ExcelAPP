using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using ExcelAPPWeb.Model;
using ExcelAPPWeb.SPI;
using ExcelAPPWeb.Service;
using System.Configuration;
namespace ExcelAPPWeb
{
    public partial class JumpDrpLogin : System.Web.UI.Page  
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //ACTDJU0003L

            var s = Base64Encode(Encoding.UTF8, "ACTDJU0003L");
            string p_Reurl = Request.QueryString["p_Reurl"].ToString().Trim();
            string Drp_Token = ConfigurationManager.AppSettings["Drp_Token"];//加入token
            string Host = ConfigurationManager.AppSettings["Host"];//加入Host
            string dbid = ConfigurationManager.AppSettings["Dbid"].ToString();
            Drp_Token = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Drp_Token, "MD5");
            byte[] outputb = Convert.FromBase64String(p_Reurl);//base64 解码
            p_Reurl = Encoding.UTF8.GetString(outputb);
            StringBuilder url = new StringBuilder(); 
           if ( string.IsNullOrEmpty(UserService.GetUserCode()))
            throw new Exception("没有获取到用户信息");
            string p_User = UserService.GetUserCode();
            //可能存在变量接受问题 参数如何解决？
             p_Reurl = p_Reurl.Replace("^", "&");
           // string p_Reurl = "Genersoft_Core/NetBill/NetBillMain.aspx?BILLID=ACTDJU0002&CHECK=0";
            url.AppendFormat("{0}/DRP/Genersoft_API/DrpLogin/GenerSoftErpLogin.aspx?Drp_Token={1}&dbid={4}&userId={2}&pass=&logdate=&logintype=1&logad=&FuncNo={3}"
                                         , Host
                                         , Drp_Token
                                         , p_User
                                         , p_Reurl
                                         , dbid
                                         );

            new ImportService().WriteLogFile("登陆" + url.ToString());
            Response.Redirect(url.ToString());
        }

        public static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }


    }
}