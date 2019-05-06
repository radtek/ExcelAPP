using Chromium.WebBrowser;
using ExcelApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelApp.JSBridge
{
    class JSLoginObject : JSObject
    {
        Login parentForm;
        internal JSLoginObject(Login parentForm)
        {
            this.parentForm = parentForm;

            AddFunction("Login").Execute += Login;
            AddFunction("Warn").Execute += Warn;

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
            var form2 = new Main();
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
