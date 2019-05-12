using Chromium;
using ExcelApp.JSBridge;
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

namespace ExcelApp
{
    public partial class Login : Formium
    {
        public Login() : base("http://localhost/Excel/asserts/login.html")
        {

            this.StartPosition = FormStartPosition.CenterScreen;
            var host = ConfigurationManager.AppSettings["Host"].ToString();
            this.LoadUrl($"http://{host}/Excel/asserts/login.html");
            //this.LoadUrl("http://www.baidu.com");
            InitializeComponent();

            LoadHandler.OnLoadStart += LoadHandler_OnLoadStart;

            GlobalObject.Add("APIBridge", new JSLoginObject(this));


        }

        public void MessageShow(string mes)
        {
            MessageBox.Show(mes);
        }
        private void LoadHandler_OnLoadStart(object sender, Chromium.Event.CfxOnLoadStartEventArgs e)
        {
            //Chromium.ShowDevTools();
        }
    }
}
