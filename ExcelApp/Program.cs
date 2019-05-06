using NetDimension.NanUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelApp
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

                Application.Run(new Login());
            }
        }
    }
}
