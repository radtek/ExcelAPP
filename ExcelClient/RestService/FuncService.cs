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

    public class FuncModel
    {

        public string res { get; set; }
        public DataTable data { get; set; }
    }
    public class FuncService
    {

        public FuncModel GetMenuList()
        {
            var host = ConfigurationManager.AppSettings["Host"].ToString();
            var client = new RestClient(host + "/excel/api/help.ashx");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("op", "GetFunc");
            IRestResponse response = client.Execute(request);
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<FuncModel>(response.Content);
            return model;
        }
    }
}
