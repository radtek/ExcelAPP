using Chromium.WebBrowser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool ChangeConfig(string AppKey, string AppValue)
        {
            bool result = true;
            try
            {
                XmlDocument xDoc = new XmlDocument();
                //获取App.config文件绝对路径
                String basePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                basePath = basePath.Substring(0, basePath.Length - 10);
                String path = basePath + "App.config";
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
            parentForm.Hide();
            var form2 = new FormConsole();
            form2.Show(parentForm);
            return;





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
