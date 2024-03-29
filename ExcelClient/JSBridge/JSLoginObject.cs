﻿using Chromium.WebBrowser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
namespace ExcelClient.JSBridge
{
    class JSLoginObject : JSObject
    {
        Login parentForm;
        internal JSLoginObject(Login parentForm)
        {
            this.parentForm = parentForm;

            AddFunction("Login").Execute += Login;
            AddFunction("Warn").Execute += Warn;
            AddFunction("GetLogin").Execute += GetLogin;
            AddFunction("SetLogin").Execute += SetLogin;

        }
        private void GetLogin(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            var user = ConfigurationManager.AppSettings["LoginUser"].ToString();
            var pwd = ConfigurationManager.AppSettings["LoginPassWord"].ToString();
            parentForm.ExecuteJavascript("SetLogin('" + user + "','" + pwd + "');");

        }
        private void SetLogin(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {

            if (e.Arguments.Length > 0)
            {
                var user = e.Arguments[0].ToString();
                var pwd = e.Arguments[1].ToString();
                ChangeConfig("LoginUser", user);
                ChangeConfig("LoginPassWord", pwd);
            }


        }

        public void ChangeConfig(string keyName, string newKeyValue)
        {
            //修改配置文件中键为keyName的项的值
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[keyName].Value = newKeyValue;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public bool ChangeConfig1(string AppKey, string AppValue)
        {
            bool result = true;
            try
            {
                XmlDocument xDoc = new XmlDocument();
                //获取App.config文件绝对路径
                string basePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                basePath = basePath.Substring(0, basePath.Length - 10);
                string path = basePath + "App.config";
                xDoc.Load(path);

                XmlNode xNode;
                XmlElement xElem1;
                XmlElement xElem2;
                //修改完文件内容，还需要修改缓存里面的配置内容，使得刚修改完即可用
                //如果不修改缓存，需要等到关闭程序，在启动，才可使用修改后的配置信息
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                xNode = xDoc.SelectSingleNode("//appSettings");
                xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
                if (xElem1 != null)
                {
                    xElem1.SetAttribute("value", AppValue);
                    cfa.AppSettings.Settings[AppKey].Value = AppValue;
                }
                else
                {
                    xElem2 = xDoc.CreateElement("add");
                    xElem2.SetAttribute("key", AppKey);
                    xElem2.SetAttribute("value", AppValue);
                    xNode.AppendChild(xElem2);
                    cfa.AppSettings.Settings.Add(AppKey, AppValue);
                }
                //改变缓存中的配置文件信息（读取出来才会是最新的配置）
                cfa.Save();
                ConfigurationManager.RefreshSection("appSettings");
                xDoc.Save(path);
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        //    this.RequireUIThread(() =>
        //    {
        //      
        //    });

        /// <summary>
        /// 获取帮助数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
          
            //DeleteDir(Directory.GetCurrentDirectory() + @"\Cache");
            if (e.Arguments.Length > 0)
            {
                var a1 = e.Arguments[0].ToString();
                var cookie = e.Arguments[1].ToString();
                UserInfo.Cookie = cookie;
                SetUserContext(cookie);
            }

            parentForm.DialogResult = System.Windows.Forms.DialogResult.OK;
            parentForm.Hide();

            var form2 = new FormConsole();
            form2.SetParentForm(parentForm);
            form2.Show();
            



        }

        //========================================  
        //实现一个静态方法将指定文件夹下面的所有内容Detele
        //测试的时候要小心操作，删除之后无法恢复。
        //========================================
        public static void DeleteDir(string aimPath)
        {
            try
            {
                //检查目标目录是否以目录分割字符结束如果不是则添加之
                if (aimPath[aimPath.Length - 1] !=
                    Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                //得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                //如果你指向Delete目标文件下面的文件而不包含目录请使用下面的方法
                //string[]fileList=  Directory.GetFiles(aimPath);
                string[] fileList = Directory.GetFileSystemEntries(aimPath);
                //遍历所有的文件和目录 
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个
                    //目录就递归Delete该目录下面的文件 
                    if (Directory.Exists(file))
                    {
                        DeleteDir(aimPath + Path.GetFileName(file));
                    }
                    //否则直接Delete文件 
                    else
                    {
                        File.Delete(aimPath + Path.GetFileName(file));
                    }
                }
                //删除文件夹 
                System.IO.Directory.Delete(aimPath, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void SetUserContext(string cookie)
        {
            var cookieArr = cookie.Split(';');
            foreach (var item in cookieArr)
            {
                var valArr = item.Split('=');
                if (valArr[0] == "EAToken")
                {
                    JsonWebToken.Decode(valArr[1], "XB#4%", true);
                    var parts = cookie.Split('.');
                    //if (parts.Length != 3) throw new Exception("invalid Session Info!");
                    var payload = parts[1];
                    var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
                    var user = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.GSPUser>(payloadJson);
                    UserInfo.UserCode = user.Code;
                    UserInfo.UserName = user.Name;
                    UserInfo.UserId = user.Id;
                }


            }
            // cookie = cookie.Replace("EAToken=", "");


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


        private void Warn(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {



            if (e.Arguments.Length > 0)
            {
                var mes = e.Arguments[0].ToString();
                parentForm.MessageShow(mes);
            }
        }



    }
}
