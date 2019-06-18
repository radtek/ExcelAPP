using Chromium;
using ExcelClient.JSBridge;
using NetDimension.NanUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelClient
{
    public partial class Login : Formium
    {
        public Login() : base("http://localhost/Excel/asserts/login.html")
        {

            this.StartPosition = FormStartPosition.CenterScreen;
            var host = ConfigurationManager.AppSettings["Host"].ToString();
            this.LoadUrl($"{host}/Excel/asserts/login.html");
            //this.LoadUrl("http://www.baidu.com");
            InitializeComponent();

            LoadHandler.OnLoadStart += LoadHandler_OnLoadStart;

            GlobalObject.Add("APIBridge", new JSLoginObject(this));
            //Chromium.ShowDevTools();

        }

        public void MessageShow(string mes)
        {
            MessageBox.Show(mes);
        }
        private void LoadHandler_OnLoadStart(object sender, Chromium.Event.CfxOnLoadStartEventArgs e)
        {
            var host = ConfigurationManager.AppSettings["Host"].ToString();
            var url = $"{host}/Excel/asserts/login.html";
            Chromium.ShowDevTools();

        }

        public void ChangeConfig(string keyName, string newKeyValue)
        {
            //修改配置文件中键为keyName的项的值
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[keyName].Value = newKeyValue;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void setCookie(string key, string value, string dwmc)
        {
            ExecuteJavascript("setCookie('" + key + "','" + value + "')");
            ChangeConfig("LastSetDWBH", value);
            ChangeConfig("LastSetDWMC", dwmc);
        }
    }
}
