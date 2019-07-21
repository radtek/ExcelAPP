using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelClient.RestService
{

    public class RestModel
    {

        public string res { get; set; }
        public RestModelData data { get; set; }
    }
    public class RestModelData
    {

        public DataTable Rows { get; set; }
    }

    public static class DWService
    {

        public static RestModel GetDWData(string filter)
        {

            var host = ConfigurationManager.AppSettings["Host"].ToString();
            var client = new RestClient(host + "/excel/api/help.ashx");
            var request = new RestRequest(Method.POST);
            //request.AddHeader("Postman-Token", "ef2f2b5e-1172-4cee-8edf-ba1a35fc1971");
            //request.AddHeader("Cookie", UserInfo.Cookie);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var parm = UserInfo.Cookie.Split(';');

            request.AddParameter("EAToken", getEAToken(), ParameterType.Cookie);
            request.AddHeader("Set-Cookie", UserInfo.Cookie);
            request.AddParameter("op", "GetHelpData");
            request.AddParameter("id", "LSBZDW");
            request.AddParameter("filter", filter);
            request.AddParameter("order", "");
            request.AddParameter("row", "");
            request.AddParameter("page", 1);
            request.AddParameter("pageSize", 500);

            IRestResponse response = client.Execute(request);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<RestModel>(response.Content);
        }

        public static string getEAToken()
        {
            var token = "";
            var parm = UserInfo.Cookie.Split(';');
            foreach (var p in parm)
            {
                var parr = p.Split('=');
                if (parr[0].Trim() == "EAToken")
                {
                    token = parr[1];
                }
            }
            return token;
        }
    }
}
