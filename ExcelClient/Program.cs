using NetDimension.NanUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            Bootstrap.ApplicationDataDirectory = Directory.GetCurrentDirectory() + @"\Cache";
            if (Bootstrap.Load())
            {

                Bootstrap.RegisterAssemblyResources(System.Reflection.Assembly.GetExecutingAssembly());


                frmDevQryShow frm = new frmDevQryShow();
                frm.ProcessID = "1";
                frm.PsDWBH = "0001";
                frm.PsJEJD = "2";
                frm.PsYear = "";
                frm.PsSLJD = "2";
                frm.PsSelect = "select * from lsbzdw ";
                frm.PsSubTitle = "33";
                frm.PsTitle = "123";
                frm.IsPivot = "1";
                Application.Run(frm);

                //Application.Run(new Login());
            }
        }
    }
}
