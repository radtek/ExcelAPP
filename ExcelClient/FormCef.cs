using ExcelClient.JSBridge;
using NetDimension.NanUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelClient
{
    public partial class FormCef : Formium
    {

        public FormConsole parentFF;
        public FormCef()
        {
            InitializeComponent();
            LoadHandler.OnLoadStart += LoadHandler_OnLoadStart;
            this.BorderWidth = 0;
        }



        public void SetURL(string url)
        {
            var host = ConfigurationManager.AppSettings["Host"].ToString();

            if (url.IndexOf("http") == -1)
                this.LoadUrl($"{host}/Excel/" + url);
            else
                this.LoadUrl(url);
            GlobalObject.Add("APIBridge", new JSDEVObject(this));

        }

        private void LoadHandler_OnLoadStart(object sender, Chromium.Event.CfxOnLoadStartEventArgs e)
        {
#if DEBUG


            Chromium.ShowDevTools();
#endif
        }
    }
}
