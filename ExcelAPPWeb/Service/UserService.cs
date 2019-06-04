using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExcelAPPWeb.Service
{
    
    /// <summary>
    /// 用户服务 登录 等操作
    /// </summary>
    public static class UserService
    {

         

        /// <summary>
        /// 判断是否登录
        /// </summary>
        /// <returns></returns>
        public static bool isLogin()
        {
            return false;

        }


        public static string Encrpt(string pwd)
        {
            SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider();

            byte[] bytes = Encoding.UTF8.GetBytes(pwd);
            byte[] inArray = provider.ComputeHash(bytes);
            provider.Clear();
            return Convert.ToBase64String(inArray);
        }

        /// <summary>
        /// 验证登录是否成功
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        public static bool CheckUser(string userName, string pwd, out string mes)
        {
            pwd = Encrpt(pwd);
            mes = "";

            Database Db = new Database("DataPlatformDB");
            var list = Db.Fetch<Model.GSPUser>(new Sql("select * from gspuser where code=@0 and password=@1", userName, pwd));
            if (list.Count > 0)
            {
                SetLoginInfo(list[0]);
                return true;
            }
            else
            {
                mes = "用户名密码不正确！";
                return false;
            }




        }

        /// <summary>
        /// 设置登录信息
        /// </summary>
        /// <param name="model"></param>
        public static void SetLoginInfo(Model.GSPUser data)
        {
            var jwt = JsonWebToken.Encode(data, "XB#4%", JwtHashAlgorithm.RS256);
            CookieHelper.WriteCookie("EAToken", jwt);
        }


        /// <summary>
        ///  清空用户信息
        /// </summary>
        public static void EmptyUser()
        {

        }
        /// <summary>
        /// 获取当前单位
        /// </summary>
        /// <returns></returns>
        public static string GetGsdwh()
        {
            var GSDWBH = CookieHelper.GetCookie("GSDWBH");
            return GSDWBH;
        }

        /// <summary>
        /// 获取用户Id
        /// </summary>
        /// <returns></returns>
        public static string GetUserId()
        {
            var cookie = CookieHelper.GetCookie("EAToken");

            JsonWebToken.Decode(cookie, "XB#4%", true);


            var parts = cookie.Split('.');
            //if (parts.Length != 3) throw new Exception("invalid Session Info!");
            var payload = parts[1];
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));


            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.GSPUser>(payloadJson);

            //// Session build 
            //user = StateChecker.CheckAuthString(cookie);
            return user.Id;
            //return model.Id;
        }


        /// <summary>
        /// 获取用户code
        /// </summary>
        /// <returns></returns>
        public static string GetUserCode()
        {
            var cookie = CookieHelper.GetCookie("EAToken");

            JsonWebToken.Decode(cookie, "XB#4%", true);


            var parts = cookie.Split('.');
            //if (parts.Length != 3) throw new Exception("invalid Session Info!");
            var payload = parts[1];
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));


            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.GSPUser>(payloadJson);

            //// Session build 
            //user = StateChecker.CheckAuthString(cookie);
            return user.Code;
            //return model.Id;
        }
        public static Model.GSPUser GetUser()
        {
            var cookie = CookieHelper.GetCookie("EAToken");

            JsonWebToken.Decode(cookie, "XB#4%", true);


            var parts = cookie.Split('.');
            //if (parts.Length != 3) throw new Exception("invalid Session Info!");
            var payload = parts[1];
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));


            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.GSPUser>(payloadJson);

             
            return user; 
        }
        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding  
            output = output.Replace('_', '/'); // 63rd char of encoding  
            switch (output.Length % 4) // Pad with trailing '='s  
            {
                case 0: break; // No pad chars in this case  
                case 2: output += "=="; break; // Two pad chars  
                case 3: output += "="; break; // One pad char  
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder  
            return converted;
        }



    }
}
