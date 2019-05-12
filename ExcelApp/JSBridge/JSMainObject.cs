using Chromium.WebBrowser;
using ExcelApp.Service;
using NPoco;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Dapper;
using JSON = Newtonsoft.Json.JsonConvert;

namespace ExcelApp.JSBridge
{
    class JSMainObject : JSObject
    {
        public Database Db = new Database("DataPlatformDB");

        public string filepath = "";
        CustomHelper service = new CustomHelper();
        EARuleService rservice = new EARuleService();
        ImportService iservice = new ImportService();
        public Model.EACmpCategory CurrentRule = null;
        public Dictionary<string, Model.EACmpCategory> CurrentRuleList = new Dictionary<string, Model.EACmpCategory>();
        Main parentForm;
        internal JSMainObject(Main parentForm)
        {
            this.parentForm = parentForm;

            AddFunction("GetHelpInfo").Execute += GetHelpInfo;
            AddFunction("GetHelpData").Execute += GetHelpData;
            AddFunction("LoadConfig").Execute += LoadConfig;//根据规则ID加载规则 并且返回数据
            AddFunction("BeginUpload").Execute += BeginUpload;//开始上传
            AddFunction("CancelUpload").Execute += CancelUpload;//取消上传
            AddFunction("chooseFile").Execute += chooseFile;//选择文件
            AddFunction("chooseRelFile").Execute += chooseRelFile;



            AddFunction("LoadExcelDataNew").Execute += LoadExcelDataNew;//选择文件
            AddFunction("LoadExcelData").Execute += LoadExcelData;//选择文件
            AddFunction("LoadLocalData").Execute += LoadLocalData;//选择文件


            AddFunction("refreshData").Execute += refreshData;//选择文件
            AddFunction("setModel").Execute += setModel;//选择文件
            AddFunction("GetSettings").Execute += GetSettings;
            AddFunction("SetSettings").Execute += SetSettings;
            AddFunction("Test").Execute += TestConnection;
            AddFunction("ShowDev").Execute += ShowDevTools;

        }




        private void ShowDevTools(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            parentForm.ShowDevTools();
        }

        private void setModel(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {

            if (e.Arguments.Length > 0)
            {
                var id = e.Arguments[0].ToString();
                var model = e.Arguments[1].ToString();
                if (!CurrentRuleList.ContainsKey(id))
                    CurrentRuleList.Add(id, Newtonsoft.Json.JsonConvert.DeserializeObject<Model.EACmpCategory>(model));
            }
        }

        /// <summary>
        /// 获取帮助数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetHelpData(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {

            try
            {

                if (e.Arguments.Length > 0)
                {
                    var helpID = e.Arguments[0].ToString();
                    var filter = e.Arguments[1].ToString();
                    var order = e.Arguments[2].ToString();
                    var page = int.Parse(e.Arguments[3].ToString());
                    var pageSize = int.Parse(e.Arguments[4].ToString());
                    e.SetReturnValue(JSON.SerializeObject(service.GetDataList(helpID, filter, order, page, pageSize)));
                }
            }
            catch (Exception ex)
            {
                parentForm.ExecuteJavascript("ImportController.msg('" + ex.Message + "');");
            }
        }



        private void GetHelpInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments.Length > 0)
            {
                var HelpID = e.Arguments[0].ToString();
                e.SetReturnValue(JSON.SerializeObject(service.GetHelpInfo(HelpID)));
            }

        }

        public delegate void MyDelegate(string s);
        public void AppendLog(string percent)
        {
            parentForm.ExecuteJavascript("ImportController.progress('" + percent + "');");
        }

        public void Message(string data)
        {
            parentForm.ExecuteJavascript("ImportController.msg('" + data + "');");
        }


        public void BeginUpload(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments.Length > 0)
            {
                var data = e.Arguments[0].ToString();
                Task.Factory.StartNew(() => ProcessGrid(data));
            }
        }

        public void ProcessGrid(string data)
        {
            MyDelegate inv = new MyDelegate(AppendLog);
            MyDelegate msg = new MyDelegate(Message);
            iservice.UploadData(CurrentRule, data,
                (string mes) =>
                {
                    parentForm.BeginInvoke(inv, mes);

                }, (string mes) =>
                {
                    parentForm.BeginInvoke(msg, mes);
                });
        }
        public void CancelUpload(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments.Length > 0)
            {
                var data = e.Arguments[0].ToString();
                Task.Factory.StartNew(() => ProcessGridCancel(data));
            }
        }



        public void ProcessGridCancel(string data)
        {
            MyDelegate inv = new MyDelegate(AppendLog);
            MyDelegate msg = new MyDelegate(Message);


            iservice.CancelUpload(CurrentRule, data,
                (string mes) =>
                {
                    parentForm.BeginInvoke(inv, mes);

                }, (string mes) =>
                {
                    parentForm.BeginInvoke(msg, mes);
                });


        }

        #region 加载规则配置
        private void LoadConfig(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments.Length > 0)
            {
                var dwbh = e.Arguments[0].ToString();
                var lbid = e.Arguments[1].ToString();

                CurrentRule = rservice.GetRuleInfo(dwbh, lbid);
                e.SetReturnValue(JSON.SerializeObject(CurrentRule));
            }

        }

        #endregion


        #region 刷新数据
        private void refreshData(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            Task.Factory.StartNew(ProcessRefresh);

        }
        private void ProcessRefresh()
        {
            ImportService newsvr = new ImportService();
            var result = newsvr.GetDBData(CurrentRule);

            ExecScript("ImportController.setData('" + JSON.SerializeObject(result) + "')");


        }
        #endregion

        #region 加载Excel数据
        private void LoadExcelData(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {

            try
            {
                if (e.Arguments.Length > 0)
                {
                    var id = e.Arguments[0].ToString();

                    List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();


                    var is2007 = false;
                    if (FileHash[id].ToLower().IndexOf("xlsx") != -1)
                        is2007 = true;
                    var stream = File.OpenRead(FileHash[id]);
                    NPOIData svr = new NPOIData();
                    if (string.IsNullOrEmpty(CurrentRuleList[id].StartLine)) CurrentRuleList[id].StartLine = "1";
                    var list = svr.getSheetInfo(stream, is2007, CurrentRuleList[id].StartLine, "");
                    e.SetReturnValue(JSON.SerializeObject(list));

                }
            }
            catch (Exception ex)
            {
                ExecScript("alert('" + ex.Message + "')");
            }


            //Task.Factory.StartNew(ProcessExcelData);
        }
        private void LoadLocalData(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {

            try
            {
                if (e.Arguments.Length > 0)
                {
                    var id = e.Arguments[0].ToString();
                    var rule = CurrentRuleList[id];
                    var proc = rule.Tmp.GetProc;
                    var dbContext = DataBaseManager.BuildContext();
                    var sql = "";
                    if (!string.IsNullOrEmpty(proc))
                    {
                        Dictionary<string, object> dict = new Dictionary<string, object>();
                        dict.Add("UserID", "");
                        //dict.Add("UserName", "");
                        dict.Add("YWID", rule.ID);
                        //dict.Add("DWBH", rule.DWBH);
                        var res = new List<IDictionary<string, object>>();
                        if (dbContext.DBType == "MSS")
                        {
                            res = ProcHelper.GetProcDataSQL(dict, proc, dbContext.Conn);
                        }
                        else
                        {
                            res = ProcHelper.GetProcDataOracle(dict, proc, dbContext.Conn);
                        }

                        e.SetReturnValue(JSON.SerializeObject(res));
                    }
                    else if (!string.IsNullOrEmpty(sql))
                    {
                        var conn = dbContext.Conn;
                        var res = conn.Query(sql);
                        e.SetReturnValue(JSON.SerializeObject(res));
                    }
                    else
                    {
                        throw new Exception("未定义本地取数数据源！");
                    }


                }
            }
            catch (Exception ex)
            {
                ExecScript("Msg.alert('" + ex.Message + "')");
            }


            //Task.Factory.StartNew(ProcessExcelData);
        }
        private void LoadExcelDataNew(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {

            try
            {
                if (e.Arguments.Length > 0)
                {
                    var id = e.Arguments[0].ToString();

                    List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();


                    var is2007 = false;
                    if (FileHashRel[id].ToLower().IndexOf("xlsx") != -1)
                        is2007 = true;
                    var stream = File.OpenRead(FileHashRel[id]);
                    NPOIData svr = new NPOIData();
                    if (string.IsNullOrEmpty(CurrentRuleList[id].RefStart)) CurrentRuleList[id].RefStart = "1";
                    var list = svr.getSheetInfo(stream, is2007, CurrentRuleList[id].RefStart, "");
                    e.SetReturnValue(JSON.SerializeObject(list));

                }
            }
            catch (Exception ex)
            {
                ExecScript("alert('" + ex.Message + "')");
            }


            //Task.Factory.StartNew(ProcessExcelData);
        }



        public void ProcessExcelData()
        {
            try
            {
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();


                var is2007 = false;
                if (filepath.ToLower().IndexOf("xlsx") != -1)
                    is2007 = true;
                var stream = File.OpenRead(filepath);
                NPOIData svr = new NPOIData();
                if (string.IsNullOrEmpty(CurrentRule.StartLine)) CurrentRule.StartLine = "1";
                var list = svr.getSheetInfo(stream, is2007, CurrentRule.StartLine, "");

                if (list.Count > 0)
                {

                    Dictionary<string, object> dictMap = new Dictionary<string, object>();

                    DataRow row = list[0].dt.Rows[0];
                    foreach (DataColumn col in list[0].dt.Columns)
                    {
                        var label = row[col.ColumnName].ToString();
                        var res = CurrentRule.Cols.Where(p => p.FName == label).ToList();
                        if (res.Count > 0)
                        {
                            dictMap.Add(res[0].FCode, col.ColumnName);
                            continue;
                        }

                    }
                    for (var i = 1; i < list[0].dt.Rows.Count; i++)
                    {
                        Dictionary<string, object> obj = new Dictionary<string, object>();
                        var dataRow = list[0].dt.Rows[i];
                        foreach (var item in CurrentRule.Cols)
                        {
                            var key = item.FCode;
                            if (dictMap.ContainsKey(key))

                                obj.Add(key, dataRow[dictMap[key].ToString()]);
                            else
                                obj.Add(key, item.DeafultValue == null ? "" : item.DeafultValue);

                        }
                        result.Add(obj);
                    }
                }


                result = iservice.ImportTmpTable(result, CurrentRule);

                ExecScript("ImportController.setData('" + JSON.SerializeObject(result) + "')");

            }
            catch (Exception ex)
            {
                ExecScript("ImportController.showError('" + ex.Message + "')");
            }

        }

        #endregion
        public static Dictionary<string, string> FileHash = new Dictionary<string, string>();
        public static Dictionary<string, string> FileHashRel = new Dictionary<string, string>();


        #region 选择文件
        public void chooseFile(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments.Length > 0)
            {
                var id = e.Arguments[0].ToString();
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Multiselect = true;
                fileDialog.Title = "请选择文件";
                fileDialog.Filter = "所有文件(*xlsx*)|*.xlsx*"; //设置要选择的文件的类型
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string file = fileDialog.FileName;//返回文件的完整路径        
                    if (!FileHash.ContainsKey(id))                           //MessageBox.Show(file);
                        FileHash.Add(id, file);
                    else
                        FileHash[id] = file;
                    //filepath = file;

                    ExecScript("setFilePath('" + id + "','" + Path.GetFileName(file) + "')");
                }
            }
        }
        #endregion
        public void chooseRelFile(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments.Length > 0)
            {
                var id = e.Arguments[0].ToString();
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Multiselect = true;
                fileDialog.Title = "请选择文件";
                fileDialog.Filter = "所有文件(*xlsx*)|*.xlsx*"; //设置要选择的文件的类型
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string file = fileDialog.FileName;//返回文件的完整路径        
                    if (!FileHashRel.ContainsKey(id))                           //MessageBox.Show(file);
                        FileHashRel.Add(id, file);
                    else
                        FileHashRel[id] = file;
                    //filepath = file;

                    ExecScript("setFilePathNew('" + id + "','" + Path.GetFileName(file) + "')");
                }
            }
        }

        public void ExecScript(string str)
        {
            parentForm.ExecuteJavascript(str);
        }


        private void GetSettings(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {

            var model = new Model.DBSettings
            {
                DataScource = ConfigurationManager.AppSettings["DataScource"].ToString(),
                DbType = ConfigurationManager.AppSettings["DbType"].ToString(),
                Catalog = ConfigurationManager.AppSettings["Catalog"].ToString(),
                UserId = ConfigurationManager.AppSettings["UserId"].ToString(),
                Password = ConfigurationManager.AppSettings["Password"].ToString(),
            };
            e.SetReturnValue(JSON.SerializeObject(model));
        }

        private void TestConnection(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {

            e.SetReturnValue(true);
        }






        private void SetSettings(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {

            if (e.Arguments.Length > 0)
            {
                var connectionStr = e.Arguments[0].ToString();
                var model = JSON.DeserializeObject<Model.DBSettings>(connectionStr);
                ChangeConfig("DataScource", model.DataScource);
                ChangeConfig("DbType", model.DbType);
                ChangeConfig("Catalog", model.Catalog);
                ChangeConfig("UserId", model.UserId);
                ChangeConfig("Password", model.Password);
            }
        }

        public bool ChangeConfig(string AppKey, string AppValue)
        {
            bool result = true;
            try
            {
                XmlDocument xDoc = new XmlDocument();
                //获取App.config文件绝对路径
                String basePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                basePath = basePath.Substring(0, basePath.Length - 10);
                String path = basePath + "App.config";
                xDoc.Load(path);

                XmlNode xNode;
                XmlElement xElem1;
                XmlElement xElem2;
                //修改完文件内容，还需要修改缓存里面的配置内容，使得刚修改完即可用
                //如果不修改缓存，需要等到关闭程序，在启动，才可使用修改后的配置信息
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                xNode = xDoc.SelectSingleNode("//appSettings");
                xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
                if (xElem1 != null)
                {
                    xElem1.SetAttribute("value", AppValue);
                    cfa.AppSettings.Settings[AppKey].Value = AppValue;
                }
                else
                {
                    xElem2 = xDoc.CreateElement("add");
                    xElem2.SetAttribute("key", AppKey);
                    xElem2.SetAttribute("value", AppValue);
                    xNode.AppendChild(xElem2);
                    cfa.AppSettings.Settings.Add(AppKey, AppValue);
                }
                //改变缓存中的配置文件信息（读取出来才会是最新的配置）
                cfa.Save();
                ConfigurationManager.RefreshSection("appSettings");
                xDoc.Save(path);
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

    }
}
