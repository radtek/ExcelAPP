using ExcelAPPWeb.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ExcelAPPWeb
{
    /// <summary>
    /// excel 的摘要说明
    /// </summary>
    public class excel : IHttpHandler
    {

        public class ExportModel
        {

            public string sheetName { get; set; }

            public DataTable dt { get; set; }
        }
        /// <summary>
        /// 导入逻辑
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var service = new ImportService();
                var rSvr = new EARuleService();
                var newsvr = new ImportService();
                context.Response.ContentType = "text/plain";
                var ruleid = context.Request.Form.Get("ruleid");
                var ruleInfo = rSvr.GetRuleInfo(ruleid);
                var op = context.Request.Form.Get("op");
                var data = context.Request.Form.Get("data");
                object res = "";
                var tips = "";
                switch (op)
                {
                    case "BeginUpload"://开始上传从中间表---正式表
                        var delData = context.Request.Form.Get("rdata");

                        service.UploadData(ruleInfo, data, delData, (string mes) =>
                          {


                          }, (string mes) =>
                          {
                              throw new Exception(mes);
                          }, (string mes) =>
                          {
                              res = mes;


                          }, out tips);
                        break;
                    case "UploadDataRef"://上传关联表
                        var delData1 = context.Request.Form.Get("rdata");

                        service.UploadDataRef(ruleInfo, data, delData1, (string mes) =>
                        {


                        }, (string mes) =>
                        {
                            throw new Exception(mes);
                        }, (string mes) =>
                        {
                            res = mes;


                        });
                        break;

                    case "CancelUpload"://取消上传中间表
                        service.CancelUpload(ruleInfo, data, (string mes) =>
                        {
                            res = mes;

                        }, (string mes) =>
                        {
                            throw new Exception(mes);
                        }, (string mes) =>
                        {
                            res = mes;
                        });
                        break;
                    case "RefreshData"://刷新界面
                        res = newsvr.GetDBData(ruleInfo);
                        break;
                    case "LoadExcelData"://导入本地excelk导入
                        var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExportModel>>(context.Request.Form.Get("tmpdata"));
                        var mapdata = GetDataMap(ruleInfo, list);

                        res = newsvr.ImportTmpTable(mapdata, ruleInfo, out tips);
                        break;
                    case "LoadExcelDataLocal"://导入本地数据源

                        res = newsvr.ImportTmpTable(Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(context.Request.Form.Get("tmpdata")), ruleInfo, out tips);
                        break;
                    case "loadExcelDataNew"://导入关联表
                        {
                            var list1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExportModel>>(context.Request.Form.Get("tmpdata"));
                            res = newsvr.ImportTmpTableREF(list1[0].dt, ruleInfo);
                        }

                        break;

                    case "removeRef"://删除引用
                        {
                            var date = context.Request.Form.Get("date");
                            res = newsvr.RemoveDate(ruleInfo, date);
                        }

                        break;
                    case "deleteRow"://删除上传行数据
                        {
                            var ids = context.Request.Form.Get("ids");
                            res = newsvr.DeleteUpload(ruleInfo, ids);
                        }

                        break;
                    case "Custom"://自定义按钮点击
                        service.CustomDealData(data, ruleInfo, (string mes) =>
                         {
                             res = mes;

                         }, (string mes) =>
                         {
                             throw new Exception(mes);

                         }, (string mes) =>
                         {
                             res = mes;
                         });
                        break;



                    default:
                        break;
                }
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { res = true, data = res, tips = tips }));
            }
            catch (Exception ex)
            {
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { res = false, msg = ex.Message, stack = ex.StackTrace }));

            }
        }


        public List<Dictionary<string, object>> GetDataMap(Model.EACmpCategory ruleInfo, List<ExportModel> list)
        {

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            if (list.Count > 0)
            {

                Dictionary<string, object> dictMap = new Dictionary<string, object>();

                DataRow row = list[0].dt.Rows[0];
                foreach (DataColumn col in list[0].dt.Columns)
                {
                    var label = row[col.ColumnName].ToString();
                    var findInfo = ruleInfo.Cols.Where(p => p.FName == label).ToList();
                    if (findInfo.Count > 0)
                    {

                        var code = findInfo[0].FCode;
                        if (!dictMap.ContainsKey(code))
                            dictMap.Add(code, col.ColumnName);
                        continue;
                    }

                }
                for (var i = 1; i < list[0].dt.Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    var dataRow = list[0].dt.Rows[i];
                    foreach (var item in ruleInfo.Cols)
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
            return result;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}