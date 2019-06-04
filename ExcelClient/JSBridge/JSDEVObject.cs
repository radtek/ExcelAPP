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

    public class QueryModel
    {
        public string pid { get; set; } = "0001";
        public string dwbh { get; set; }
        public string jejd { get; set; } = "2";
        public string id { get; set; }
        public string sql { get; set; }

        public string title { get; set; }

        public string subtitle { get; set; }

        public string ispviot { get; set; } = "1";

        public string parr { get; set; }
        public string varr { get; set; }

    }
    class JSDEVObject : JSObject
    {
        FormCef parentForm;
        internal JSDEVObject(FormCef parentForm)
        {
            this.parentForm = parentForm;

            AddFunction("showDev").Execute += showDev;
            AddFunction("showProc").Execute += showProc;

        }
        private void showDev(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments.Length > 0)
            {
                var json = e.Arguments[0].ToString();

                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryModel>(json);
                frmDevQryShow frm = new frmDevQryShow();
                frm.ProcessID = model.pid;
                frm.PsDWBH = model.dwbh;
                frm.PsJEJD = model.jejd;

                frm.PsID = model.id;
                frm.PsYear = "";// DateTime.Now.Year.ToString();
                frm.PsSLJD = model.jejd;
                frm.PsSelect = model.sql;
                frm.PsSubTitle = model.subtitle;
                frm.PsTitle = model.title;
                frm.IsPivot = model.ispviot;
                this.parentForm.parentFF.openDev(model.id, model.title, frm);
            }
        }
        private void showProc(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {

            if (e.Arguments.Length > 0)
            {
                var json = e.Arguments[0].ToString();

                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryModel>(json);
                frmDevQryShow frm = new frmDevQryShow();
                frm.ProcessID = model.pid;
                frm.PsDWBH = model.jejd;
                frm.PsJEJD = model.jejd;

                frm.PsID = model.id;
                frm.PsYear = DateTime.Now.Year.ToString();
                frm.PsSLJD = model.jejd;
                frm.PsSelect = model.sql;
                frm.PsSubTitle = model.subtitle;
                frm.PsTitle = model.title;
                frm.IsPivot = model.ispviot;
                frm.IsSql = "0";
                frm.PsTitleTable = "";


                frm.ParamArr = model.parr.Split('^');
                frm.ValueArr = model.varr.Split('^');
                this.parentForm.parentFF.openDev(model.id, model.title, frm);

                //frm.QryParam = this.QryParam;
                //frm.QryValue = this.QryValue;

                //frm.IsRepeatDown = this.IsRepeatDown;
                //frm.IsUseLocal = this.IsUseLocal;

                //frm.Show();
            }
        }


    }
}
