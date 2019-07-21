using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelClient.RestService
{
    public class VersionModel
    {

        public string res { get; set; }
        public string data { get; set; }
    }
    public class VersionService
    {

        public VersionModel GetVersion()
        {
            var host = ConfigurationManager.AppSettings["Host"].ToString();
            var client = new RestClient(host + "/excel/api/help.ashx");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("op", "GetVersion");
            IRestResponse response = client.Execute(request);
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<VersionModel>(response.Content);
            return model;
        }

        public void ChangeConfig(string keyName, string newKeyValue)
        {
            //修改配置文件中键为keyName的项的值
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[keyName].Value = newKeyValue;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void VersionUpdate()
        {
            var remoteVer = GetVersion().data;
            var localVer = ConfigurationManager.AppSettings["VERSION"].ToString();
            if (remoteVer != localVer) {
                // 清楚缓存
                // 写入缓存
                DeleteDir(Directory.GetCurrentDirectory() + @"\Cache");
                ChangeConfig("VERSION", remoteVer);
            }
        }

        public static void DeleteDir(string aimPath)
        {
            try
            {
                //检查目标目录是否以目录分割字符结束如果不是则添加之
                if (aimPath[aimPath.Length - 1] !=
                    Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                //得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                //如果你指向Delete目标文件下面的文件而不包含目录请使用下面的方法
                //string[]fileList=  Directory.GetFiles(aimPath);
                string[] fileList = Directory.GetFileSystemEntries(aimPath);
                //遍历所有的文件和目录 
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个
                    //目录就递归Delete该目录下面的文件 
                    if (Directory.Exists(file))
                    {
                        DeleteDir(aimPath + Path.GetFileName(file));
                    }
                    //否则直接Delete文件 
                    else
                    {
                        File.Delete(aimPath + Path.GetFileName(file));
                    }
                }
                //删除文件夹 
                System.IO.Directory.Delete(aimPath, true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
