using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web.Services.Protocols;

namespace ExcelClient
{
    public class DevQryPubFun
    {
        public DevQryPubFun()
        {
        }

        public static string GSYDBSrc = "GSY";
        private static object _gspClientService;


        public static void WrapService(SoapHttpClientProtocol webService)
        {
            var host = ConfigurationManager.AppSettings["Host"].ToString();

            webService.Url = host + "/excel/WsGetData.asmx";
  
            // string url = Genersoft.Platform.AppFramework.ClientService.ClientContext.Current.GetUrl("/GSIDP/GSYS/GSIDPWEB/WSGetData.asmx");
            // webService.Url =url.Substring(0,url.ToLower().IndexOf("/cwbase/"))+"/cwbase/GSIDP/GSYS/GSIDPWEB/WSGetData.asmx";
            ////  System.Windows.Forms.MessageBox.Show(webService.Url);
            // /*废弃不用
            //  * if (!IsTest())
            // {
            //     if (_gspClientService == null)
            //     {
            //         throw new Exception("无法加载Genersoft.Platform.AppFramework.ClientService.dll中的Genersoft.Platform.AppFramework.ClientService.ClientContext类");
            //     }
            //     Type[] types = new Type[] { typeof(SoapHttpClientProtocol) };
            //     _gspClientService.GetType().GetMethod("WrapService", types).Invoke(_gspClientService, new object[] { webService });
            // }*/

            // Genersoft.Platform.AppFramework.ClientService.ClientContext.Current.WrapService(webService);

        }
    }
}
