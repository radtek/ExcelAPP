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
    public partial class Main : Formium
    {

        public Main() : base("http://localhost/Excel/asserts/frame.html")
        {

            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            //Chromium.ShowDevTools();

            LoadHandler.OnLoadStart += LoadHandler_OnLoadStart;
            InitializeComponent();

            this.BorderWidth = 0;

        }


        public void SetURL(string ruleID, string DWBH)
        {
            var host = ConfigurationManager.AppSettings["Host"].ToString();
            this.LoadUrl($"{host}/Excel/asserts/import.html?id=" + ruleID + "&dwbh=" + DWBH);


            LoadHandler.OnLoadStart += LoadHandler_OnLoadStart;
            GlobalObject.Add("APIBridge", new JSMainObject(this));
        }

        private void LoadHandler_OnLoadStart(object sender, Chromium.Event.CfxOnLoadStartEventArgs e)
        {
#if DEBUG


            Chromium.ShowDevTools();
#endif
        }


        public void ShowDevTools()
        {

            Chromium.ShowDevTools();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Application.Exit();
        }
    }
}
