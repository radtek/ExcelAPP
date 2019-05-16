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
    public partial class Main : Formium
    {

        public Main() : base("http://localhost/Excel/asserts/frame.html")
        {

            var host = ConfigurationManager.AppSettings["Host"].ToString();
            this.LoadUrl($"{host}/Excel/asserts/frame.html");
            InitializeComponent();

            LoadHandler.OnLoadStart += LoadHandler_OnLoadStart;

            GlobalObject.Add("APIBridge", new JSMainObject(this));

            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            //Chromium.ShowDevTools();
        }


        private void LoadHandler_OnLoadStart(object sender, Chromium.Event.CfxOnLoadStartEventArgs e)
        {
#if DEBUG

            
            //Chromium.ShowDevTools();
#endif
        }


        public void ShowDevTools()
        {

            Chromium.ShowDevTools();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
