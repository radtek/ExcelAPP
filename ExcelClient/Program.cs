using DevExpress.Skins;
using DevExpress.UserSkins;
using ExcelClient.RestService;
using NetDimension.NanUI;
using System;
using System.Collections.Generic;
using System.Drawing;
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


            //SkinManager.EnableFormSkins();
            SkinManager.EnableMdiFormSkins();
            SkinManager.EnableFormSkinsIfNotVista();
            BonusSkins.Register();
            OfficeSkins.Register();
            //增加版本号检查 版本不一致 强制更新
            VersionService vr = new VersionService();
            vr.VersionUpdate();


            //DictionOrFilePathOperator.UserNo = "root";
            // ServiceManager.ResourceService.ImagesPath = AppDomain.CurrentDomain.BaseDirectory + "Images\\";

            //string systemProperty = ServiceManager.SystemService.GetSystemProperty("SystemTitle", "FirstLogin");


            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            Bootstrap.ApplicationDataDirectory = Directory.GetCurrentDirectory() + @"\Cache";

            if (Bootstrap.Load())
            {

                Bootstrap.RegisterAssemblyResources(System.Reflection.Assembly.GetExecutingAssembly());
                try
                {

                   
                    Application.Run(new Login());
                }
                catch (Exception ew)
                {

                    MessageBox.Show(ew.Message);
                }
            }
        }



    }
}
