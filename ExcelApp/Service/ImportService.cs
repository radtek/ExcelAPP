using NPoco;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelApp.Service
{

    /// <summary>
    /// 导入服务 导入excel 导入关联表 上传 取消上传等操作
    /// </summary>
    public class ImportService
    {
        public Database Db = new Database("DataPlatformDB");

        //当前缓存变量信息


        public void GetDataTableByFormat(string RuleId, string FilePath)
        {

        }



        #region 导入excel临时数据并上传到中间表
        public List<Dictionary<string, object>> ImportTmpTable(List<Dictionary<string, object>> dt, Model.EACmpCategory model)
        {

            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            var tmpTable = model.TmpTab;

            StringBuilder sb = new StringBuilder();
            List<string> keys = new List<string>();
            StringBuilder values = new StringBuilder();
            sb.AppendFormat("insert into {0} (ID,CreateUser,CreateTime,", tmpTable);
            values.Append("(@0,@1,@2,");
            List<Model.EACmpCateCols> cols = model.Cols.Where(n => n.IsShow == "1").ToList();

            int i = 0;
            foreach (var item in cols)
            {
                if (i == cols.Count - 1)
                {
                    values.Append("@" + (i + 3).ToString());
                    sb.Append(item.FCode);
                }
                else
                {
                    values.Append("@" + (i + 3).ToString() + ",");
                    sb.Append(item.FCode + ",");
                }
                keys.Add(item.FCode);
                i++;
            }
            sb.Append(") values");
            values.Append(")");

            var sql = sb.Append(values).ToString();

            Db.Execute(new Sql("delete from " + tmpTable + " where flag='0' and CreateUser=@0 ", UserService.GetUserId()));
            foreach (Dictionary<string, object> row in dt)
            {
                object[] para = GetParams(keys, row).ToArray();
                Db.Execute(sql, para);
            }

            foreach (var item in cols)
            {
                if (!string.IsNullOrWhiteSpace(item.CalcSQL))
                {
                    Db.Execute(new Sql("update " + tmpTable + " set " + item.FCode + "=(" + item.CalcSQL + ")"));
                }
            }


            res = GetDBData(model);//Db.Fetch<Dictionary<string, object>>(new Sql("select * from " + tmpTable));
            return res;
        }


        public List<Dictionary<string, object>> GetDBData(Model.EACmpCategory model)
        {
            var tmpTable = model.TmpTab;

            return Db.Fetch<Dictionary<string, object>>(new Sql("select * from " + tmpTable + " where 1=1 and CreateUser=@0", UserService.GetUserId()));
        }

        public ArrayList GetParams(List<string> keys, Dictionary<string, object> row)
        {
            var arr = new ArrayList();
            arr.Add(Guid.NewGuid().ToString());
            arr.Add(UserService.GetUserId());
            arr.Add(DateTime.Now.ToString());

            foreach (string key in keys)
            {
                var value = "";
                if (row.ContainsKey(key))
                {
                    value = row[key].ToString();
                }

                arr.Add(value);
            }

            return arr;
        }

        #endregion
        public void UploadData(Model.EACmpCategory model, string ExcelData, Action<string> onProgress, Action<string> onError)
        {

            try
            {

                List<Dictionary<string, object>> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(ExcelData);
                if (list.Count == 0)
                {
                    onError("没有上传选中数据");
                    return;
                }
                StringBuilder sb = new StringBuilder();
                List<string> keys = new List<string>();
                sb.AppendFormat("update {0} set ", model.TmpTab);
                List<Model.EACmpCateCols> cols = model.Cols.Where(n => n.IsShow == "1").ToList();
                var i = 1;
                foreach (var item in cols)
                {
                    if (i == cols.Count)
                    {
                        sb.Append(item.FCode + "=@" + i.ToString() + " ");
                    }
                    else
                    {
                        sb.Append(item.FCode + "=@" + i.ToString() + ",");
                    }
                    keys.Add(item.FCode);
                    i++;
                }
                sb.Append(" where id=@0");
                var sql = sb.ToString();
                int count = 0;
                foreach (Dictionary<string, object> row in list)
                {
                    count++;
                    object[] para = GetParamsUpdate(keys, row).ToArray();
                    Db.Execute(sql, para);
                    float ftemp = (float)count / list.Count;

                    onProgress((ftemp * 100).ToString());
                }
                WriteLog($"上传成功数据{count.ToString()}条", model.ID);
            }
            catch (Exception ex)
            {
                onError(ex.Message);
            }
            // 更新中间表
            // 按照行调用存储过程
            // 更新中间表状态
        }
        public ArrayList GetParamsUpdate(List<string> keys, Dictionary<string, object> row)
        {
            var arr = new ArrayList();
            arr.Add(row["ID"].ToString());

            foreach (string key in keys)
            {
                var value = "";
                if (row.ContainsKey(key))
                {
                    value = row[key].ToString();
                }
                if (key == "FLAG")
                {

                    value = "1";
                }

                arr.Add(value);
            }

            return arr;
        }



        /// <summary>
        /// 取消上传
        /// </summary>
        /// <param name="ExcelData"></param>
        public void CancelUpload(Model.EACmpCategory model, string ExcelData, Action<string> onProgress, Action<string> onError)
        {
            //启动多线程回到存储过程 刷新页面

            try
            {

                List<Dictionary<string, object>> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(ExcelData);
                if (list.Count == 0)
                {
                    onError("没有上传选中数据");
                    return;
                }


                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("update {0} set FLAG='0' where id=@0", model.TmpTab);
                var sql = sb.ToString();
                int count = 0;
                foreach (Dictionary<string, object> row in list)
                {
                    count++;
                    Db.Execute(sql, row["ID"].ToString());
                    float ftemp = (float)count / list.Count;

                    onProgress((ftemp * 100).ToString());
                }

                WriteLog($"取消上传数据{count.ToString()}条", model.ID);
            }
            catch (Exception ex)
            {
                onError(ex.Message);
            }

        }





        public List<Dictionary<string, object>> GetProcData(Model.EACategory model)
        {
            var procName = "";

            List<Dictionary<string, object>> list = Db.Fetch<Dictionary<string, object>>($"exec {procName} @0", model.ID);
            return list;

        }
        public void WriteLog(string op, string rid)
        {
            var model = new Model.EAOpLog
            {
                ID = Guid.NewGuid().ToString(),
                OpTime = DateTime.Now.ToString(),
                OpInfo = op,
                UserCode = UserService.GetUserId(),
                RID = rid
            };
            Db.Save(model);
        }
    }
}
