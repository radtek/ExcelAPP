using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExcelApp.Service
{
    public static class UserService
    {

        private static Model.GSPUser model = null;



        public static Database Db = new Database("DataPlatformDB");

        /// <summary>
        /// 判断是否登录
        /// </summary>
        /// <returns></returns>
        public static bool isLogin()
        {

            return model == null;
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
            model = data;
        }


        /// <summary>
        ///  清空用户信息
        /// </summary>
        public static void EmptyUser()
        {
            model = null;
        }


        /// <summary>
        /// 获取用户Id
        /// </summary>
        /// <returns></returns>
        public static string GetUserId()
        {
            return model.Id;
        }
    }
}
