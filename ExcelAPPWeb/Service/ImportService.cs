using Dapper;
using ExcelAPPWeb.DB;
using ExcelAPPWeb.SPI;
using NPoco;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ExcelAPPWeb.Service
{

    /// <summary>
    /// 导入服务 导入excel 导入关联表 上传 取消上传等操作
    /// </summary>
    public class ImportService
    {
        public Database Db = DataBaseManager.GetDB();//获取当前database


        public string token = DataBaseManager.GetToken();
        #region 获取临时表所有数据
        /// <summary>
        /// 获取临时表所有数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetDBData(Model.EACmpCategory model)
        {
            var tmpTable = model.TmpTab;

            return Db.Fetch<Dictionary<string, object>>(new Sql("select * from " + tmpTable + " where 1=1 and CreateUser=@0", UserService.GetUserId()));
        }





        public object GetDataREFList(Model.EACmpCategory model, string Flag, string orderBy, int Page, int Size, string StartDate, string EndDate)
        {
            var filter = "";

            //filter += " and Flag='" + Flag + "'";

            var result = Db.Page<Dictionary<string, object>>(Page, Size, new Sql("select EARefTable.* from  EARefTable where 1=1 " + filter + "and UserId=@0 " + orderBy,
                UserService.GetUserId()));
            return new
            {
                Rows = result.Items,
                Total = result.TotalItems
            };
        }
        public object GetDataList(Model.EACmpCategory model, string Flag, string orderBy, int Page, int Size, string StartDate, string EndDate)
        {
            var filter = "";
            var RQ1 = "";
            var RQ2 = "";
            filter += " and Flag='" + Flag + "'";
            if (!string.IsNullOrEmpty(StartDate))
            {
                filter += " and CreateTime>='" + StartDate + " 00:00:00'";
                RQ1 = StartDate + " 00:00:00";
            }

            if (!string.IsNullOrEmpty(EndDate))
            {
                filter += " and CreateTime<='" + EndDate + " 23:59:59'";
                RQ2 = EndDate + " 23:59:59";
            }

            if (Flag == "1")
            {

                try
                {

                    using (var conn = DataBaseManager.GetDbConnection())
                    {
                        DealProcNewRq(RQ1, RQ2, model.Tmp.SwitchProc, model, new List<Dictionary<string, object>>(), conn, null);
                    }
                }
                catch (Exception ex)
                {
                    WriteLogFile("错误获取已上传数据" + ex.ToString());
                }



            }
            var result = Db.Page<Dictionary<string, object>>(Page, Size, new Sql("select " + model.TmpTab + ".* from " + model.TmpTab + " where 1=1 " + filter + "and CreateUser=@0 " + orderBy,
                UserService.GetUserId()));
            return new
            {
                Rows = result.Items,
                Total = result.TotalItems
            };
        }
        #endregion


        #region 导入excel临时数据并上传到中间表
        public List<Dictionary<string, object>> ImportTmpTable(List<Dictionary<string, object>> dt, Model.EACmpCategory model, out string mes)
        {

            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            var tmpTable = model.TmpTab;
            StringBuilder sb = new StringBuilder();
            List<string> keys = new List<string>();
            List<string> colTypes = new List<string>();
            StringBuilder values = new StringBuilder();
            StringBuilder sbmes = new StringBuilder();
            sb.AppendFormat("insert into {0} (ID,CreateUser,CreateTime,GSDWBH,COLORZT,", tmpTable);
            values.AppendFormat("({0}0,{0}1,{0}2,{0}3,{0}4,", token);//add luchg 增加单位
            List<Model.EACmpCateCols> cols = model.Cols.Where(n => n.IsShow == "1").ToList();
            mes = "";
            int i = 0;
            int s = 0;
            foreach (var item in cols)
            {
                if (i == cols.Count - 1)
                {
                    values.Append(token + (i + 5).ToString());
                    sb.Append(item.FCode);
                }
                else
                {
                    values.Append(token + (i + 5).ToString() + ",");
                    sb.Append(item.FCode + ",");
                }
                colTypes.Add(item.FType);
                keys.Add(item.FCode);
                i++;
            }
            sb.Append(") values");
            values.Append(")");

            var sql = sb.Append(values).ToString();
            TableService.CreateTable(model);
            //var mes = "";
            using (var conn = DataBaseManager.GetDbConnection())
            {
                //conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    conn.Execute("delete from " + tmpTable + " where flag='0' and CreateUser=" + token + "CreateUser ", new { CreateUser = UserService.GetUserId() }, transaction);
                    foreach (Dictionary<string, object> row in dt)
                    {
                        var p = new DynamicParameters();
                        DynamicParameters para = GetParams(model, keys, row, colTypes, out mes);
                        s++;
                        if (mes != "")
                        {
                            sbmes.Append($"第{s.ToString()}行{mes}");
                            mes = "";
                        }
                        conn.Execute(sql, para, transaction);
                    }
                    mes = sbmes.ToString();
                    foreach (var item in cols)
                    {
                        if (!string.IsNullOrWhiteSpace(item.CalcSQL))
                        {
                            if (item.CalcSQL.IndexOf("1=1") == -1)
                            {
                                item.CalcSQL += " where 1=1 ";
                            }
                            conn.Execute("update " + tmpTable + " set " + item.FCode + "=" + item.CalcSQL + " and CreateUser=" + token + "CreateUser ", new { CreateUser = UserService.GetUserId() }, transaction);
                        }
                    }
                    DealProcNew(model.Tmp.LoadProc, model, dt, conn, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    WriteLogFile("错误导入excel临时数据并上传到中间表" + ex.ToString());
                    throw ex;
                }
            }
            //Db.Execute(new Sql("delete from " + tmpTable + " where flag='0' and CreateUser=@0 ", UserService.GetUserId()));

            res = GetDBData(model);
            return res;
        }





        public DynamicParameters GetParams(Model.EACmpCategory model, List<string> keys, Dictionary<string, object> row, List<string> colTypes, out string mes)
        {

            DynamicParameters p = new DynamicParameters();
            var sb = new StringBuilder();
            var arr = new ArrayList();
            p.Add("0", Guid.NewGuid().ToString());
            p.Add("1", UserService.GetUserId());
            p.Add("2", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            p.Add("3", model.DWBH.ToString());  //增加 单位编号  
            var i = 5;
            var index = 0;
            foreach (string key in keys)
            {
                object value = "";

                if (row.ContainsKey(key))
                {
                    #region 需要判断一下 数值型 和日期型 是否为 合法字符

                    if (colTypes[index] == "int")
                    {
                        var intRes = 0;
                        if (row[key] != null)
                        {

                            if (!int.TryParse(row[key].ToString(), out intRes))
                            {
                                if (row[key].ToString() != "")
                                {
                                    sb.Append($"第{index.ToString()}列字段{key}数值金额[" + row[key].ToString() + "]有问题，已忽略处理");
                                    p.Add("4", "1");
                                }
                            }
                            value = intRes;
                        }
                        else
                        {
                            value = 0;
                        }
                    }
                    else if (colTypes[index] == "number")
                    {
                        if (row[key] != null)
                        {
                            decimal floatRes = 0;
                            if (!decimal.TryParse(row[key].ToString(), out floatRes))
                            {
                                if (row[key].ToString() != "")
                                {
                                    sb.Append($"第{index.ToString()}列字段{key}字段金额[" + row[key].ToString() + "]有问题，已忽略处理");
                                    p.Add("4", "1");
                                }

                            }
                            value = floatRes;
                        }
                        else
                        {
                            value = 0;
                        }

                    }
                    else if (colTypes[index] == "date")
                    {
                        if (row[key] != null)
                        {
                            DateTime dtDate;
                            if (DateTime.TryParse(row[key].ToString(), out dtDate))
                            {
                                sb.Append($"第{index.ToString()}列字段{key}字段日期" + row[key].ToString() + "有问题，已忽略处理");
                                p.Add("4", "1");  // 
                            }
                            value = dtDate.ToString("yyyy-MM-dd HH:mm:ss");

                        }
                        else
                        {
                            value = "1900-01-01 00:00:00";
                        }

                    }
                    else
                    {
                        if (row[key] != null)
                        {
                            value = row[key].ToString();
                        }
                        else
                        {
                            value = "";
                        }
                        p.Add("4", " ");  // 
                    }
                    #endregion
                }

                p.Add(i.ToString(), value);
                i++;
                index++;
            }
            mes = sb.ToString();

            return p;
        }

        #endregion


        #region 上传数据到中间表

        private string GetUpdateSql(Model.EACmpCategory model, out List<string> keys, out List<string> colTypes)
        {
            StringBuilder sb = new StringBuilder();
            keys = new List<string>();
            colTypes = new List<string>();
            sb.AppendFormat("update {0} set    ", model.TmpTab);
            List<Model.EACmpCateCols> cols = model.Cols.Where(n => n.IsShow == "1").ToList();
            var i = 1;
            sb.Append("COLORZT=" + token + i.ToString() + ",");
            i = 2;

            foreach (var item in cols)
            {

                if (i - 1 == cols.Count)
                {
                    sb.Append(item.FCode + "=" + token + i.ToString() + " ");
                }
                else
                {
                    sb.Append(item.FCode + "=" + token + i.ToString() + ",");
                }
                keys.Add(item.FCode);
                colTypes.Add(item.FType);
                i++;
            }

            sb.Append(" where id=" + token + "0");
            return sb.ToString();
        }
        public void UploadData(Model.EACmpCategory model, string ExcelData, string DelData, Action<string> onProgress, Action<string> onError, Action<string> onScucess, out string mes)
        {
            mes = "";
            try
            {
                List<string> keys = new List<string>();
                List<string> colTypes = new List<string>();
                List<Dictionary<string, object>> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(ExcelData);
                StringBuilder sbmes = new StringBuilder();
                if (list.Count == 0)
                {
                    onError("没有上传选中数据");
                    return;
                }
                var sql = GetUpdateSql(model, out keys, out colTypes);
                int count = 0;
                using (var conn = DataBaseManager.GetDbConnection())
                {
                    //conn.Open();
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {

                        foreach (Dictionary<string, object> row in list)
                        {
                            count++;
                            var p = new DynamicParameters();
                            DynamicParameters para = GetParamsUpdate(keys, row, colTypes, out mes);
                            conn.Execute(sql, para, transaction);
                            if (mes != "")
                            {
                                sbmes.Append($"第{count.ToString()}行{mes}");
                                mes = "";
                            }
                        }

                        mes = sbmes.ToString();
                        List<Dictionary<string, object>> delList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(DelData);
                        foreach (Dictionary<string, object> row in delList)
                        {
                            conn.Execute("delete from " + model.TmpTab + " where ID=" + token + "ID",
                                new { ID = row["ID"].ToString() }, transaction);
                        }

                        DealProcNew(model.ImprtProc, model, list, conn, transaction);
                        //处理程序集
                        DealAssExtend("upload", list, model, conn, transaction);
                        //更新上传后状态
                        conn.Execute("update " + model.TmpTab + " set FLAG='1' where FLAG='2' and CreateUser=" + token + "CreateUser",
                            new { CreateUser = UserService.GetUserId() },
                            transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        WriteLogFile("错误上传数据" + ex.ToString());
                        throw ex;
                    }
                }
                var logInfo = $"上传成功数据{count.ToString()}条";
                WriteLog(logInfo, model.ID);
                onScucess(logInfo);
            }
            catch (Exception ex)
            {
                onError(ex.Message);
            }
        }


        public DynamicParameters GetParamsUpdate(List<string> keys, Dictionary<string, object> row, List<string> colTypes, out string mes)
        {

            var p = new DynamicParameters();
            p.Add("0", row["ID"].ToString());

            var i = 2;
            var sb = new StringBuilder();
            var index = 0;
            foreach (string key in keys)
            {
                object value = "";
                if (row.ContainsKey(key))
                {
                    #region 需要判断一下 数值型 和日期型 是否为 合法字符

                    if (colTypes[index] == "int")
                    {
                        var intRes = 0;
                        if (row[key] != null)
                        {

                            if (!int.TryParse(row[key].ToString(), out intRes))
                            {
                                sb.Append($"第{index.ToString()}列字段{key}数值金额" + row[key].ToString() + "有问题，已忽略处理");
                                p.Add("1", "1");
                            }
                            value = intRes;
                        }
                        else
                        {
                            value = 0;
                        }
                    }
                    else if (colTypes[index] == "number")
                    {
                        if (row[key] != null)
                        {
                            decimal floatRes = 0;
                            if (!decimal.TryParse(row[key].ToString(), out floatRes))
                            {
                                sb.Append($"第{index.ToString()}列字段{key}字段金额" + row[key].ToString() + "有问题，已忽略处理");
                                p.Add("1", "1");
                            }
                            value = floatRes;
                        }
                        else
                        {
                            value = 0;
                        }

                    }
                    else if (colTypes[index] == "date")
                    {
                        if (row[key] != null)
                        {
                            DateTime dtDate;
                            if (DateTime.TryParse(row[key].ToString(), out dtDate))
                            {
                                sb.Append($"第{index.ToString()}列字段{key}字段日期" + row[key].ToString() + "有问题，已忽略处理");
                                p.Add("1", "1");
                            }
                            value = dtDate.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            value = "1900-01-01 00:00:00";
                        }

                    }
                    else
                    {
                        if (row[key] != null)
                        {
                            value = row[key].ToString();
                        }
                        else
                        {
                            value = "";
                        }
                        p.Add("1", " ");
                    }
                    if (key == "FLAG")
                    {
                        value = "2";
                    }
                    #endregion

                    // WriteLogFile("p" + key + i.ToString()+ value+ sb.ToString());
                    p.Add(i.ToString(), value);
                    i++;
                    index++;
                }
            }

            mes = sb.ToString();
            return p;
        }

        #endregion


        #region 取消上传中间表
        /// <summary>
        /// 取消上传
        /// </summary>
        /// <param name="ExcelData"></param>
        public void CancelUpload(Model.EACmpCategory model, string ExcelData, Action<string> onProgress, Action<string> onError, Action<string> onScucess)
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
                sb.AppendFormat("update {0} set FLAG='0' where id=" + token + "ID", model.TmpTab);
                var sql = sb.ToString();
                int count = 0;


                using (var conn = DataBaseManager.GetDbConnection())
                {
                    //conn.Open();
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        foreach (Dictionary<string, object> row in list)
                        {
                            count++;
                            conn.Execute(sql, new { ID = row["ID"].ToString() }, transaction);
                            WriteLogFile("错误取消上传数据" + sql);

                        }




                        DealProcNew(model.CancelProc, model, list, conn, transaction);

                        //处理程序集

                        DealAssExtend("cancel", list, model, conn, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
                WriteLog($"取消上传数据{count.ToString()}条", model.ID);
                onScucess($"取消上传数据{count.ToString()}条");
            }
            catch (Exception ex)
            {
                WriteLogFile("错误取消上传数据" + ex.ToString());
                onError(ex.Message);
            }

        }

        #endregion

        #region 取消上传中间表
        /// <summary>
        /// 取消上传
        /// </summary>
        /// <param name="ExcelData"></param>
        public string DeleteUpload(Model.EACmpCategory model, string ids)
        {
            try
            {


                var arr = ids.Split(',');



                string sql = string.Format("delete from {0} where id=" + token + "ID", model.TmpTab);

                using (var conn = DataBaseManager.GetDbConnection())
                {
                    //conn.Open();
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        foreach (string id in arr)
                        {
                            conn.Execute(sql, new { ID = id }, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "删除成功";

        }

        #endregion


        #region 自定义按钮点击事件
        public void CustomDealData(string ExcelData, Model.EACmpCategory model, Action<string> onProgress, Action<string> onError, Action<string> onScucess)
        {


            try
            {
                List<Dictionary<string, object>> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(ExcelData);

                using (var conn = DataBaseManager.GetDbConnection())
                {
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        DealProcNew(model.CustomProc, model, list, conn, transaction);
                        DealAssExtend("custom", list, model, conn, transaction);

                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                onError(ex.Message);
            }

            // 更新中间表
            // 按照行调用存储过程
            // 更新中间表状态
        }

        #endregion


        #region 删除关联表数据
        public string RemoveDate(Model.EACmpCategory model, string date)
        {


            var arr = date.Split('-');

            using (var conn = DataBaseManager.GetDbConnection())
            {
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    //增加 期间范围内数据清空
                    WriteLogFile("date" + date);
                    //2019 - 05 - 01 - 2019 - 06 - 30
                    var filter = " 1=1 ";
                    var StartDate = "";
                    var EndDate = "";

                    if (date.Length > 5)
                    {
                        filter = "";
                        StartDate = date.Replace(" ", "").Substring(0, 10);
                        EndDate = date.Replace(" ", "").Substring(11, 10);
                        filter += "  CreateTime>='" + StartDate + " 00:00:00'";
                        filter += " and CreateTime<='" + EndDate + " 23:59:59'";

                    }
                    WriteLogFile("SQDELETE  EARefTable   where  YWID='" + model.ID + "'  AND  DWBH='" + model.DWBH + "'  AND  " + filter + "  AND   USERID=" + UserService.GetUserId() + "CreateUser");
                    conn.Execute("DELETE  EARefTable   where  YWID='" + model.ID + "'  AND  DWBH='" + model.DWBH + "'  AND  " + filter + "  AND   USERID=" + token + "CreateUser",
                          new { CreateUser = UserService.GetUserId() },
                          transaction);

                    // DealProcNew(model.RefProc, model, new List<Dictionary<string, object>>(), conn, transaction);
                    // DealAssExtend("refdel", new List<Dictionary<string, object>>(), model, conn, transaction);
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    WriteLogFile("删除关联表数据" + ex.ToString());
                    transaction.Rollback();
                    throw ex;
                }
            }
            return "操作成功！";

        }
        #endregion

        #region 导入数据到关联表
        public Dictionary<string, string> ImportTmpTableREF(DataTable dt, Model.EACmpCategory model)
        {

            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            var tmpTable = model.TmpTab;
            StringBuilder sb = new StringBuilder();
            List<string> keys = new List<string>();
            List<string> cols = new List<string>();
            Dictionary<string, string> hasCols = new Dictionary<string, string>();
            StringBuilder values = new StringBuilder();
            sb.Append("insert into EARefTable (ID,UserId,UserName,CreateTime,DWBH,YWID,FLAG");
            values.AppendFormat("({0}0,{0}1,{0}2,{0}3,{0}4,{0}5,{0}6", token);
            int i = 1;
            foreach (DataColumn col in dt.Columns)
            {
                var field = "XM" + i.ToString().PadLeft(2, '0');
                values.Append("," + token + (i + 6).ToString());
                sb.Append("," + field);

                keys.Add(col.ColumnName);
                cols.Add(field);
                i++;
            }

            sb.Append(") values");
            values.Append(")");

            var sql = sb.Append(values).ToString();



            using (var conn = DataBaseManager.GetDbConnection())
            {
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    int s = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (s == 0)
                        {
                            s++;

                            for (var j = 0; j < cols.Count; j++)
                            {

                                hasCols.Add(cols[j], dt.Rows[0][keys[j]].ToString());
                            }
                            continue;
                        }

                        var para = GetParamsRef(keys, row, model, UserService.GetUser());
                        conn.Execute(sql, para, transaction);
                    }
                    transaction.Commit();


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    WriteLogFile("错误导入数据到关联表" + ex.ToString());
                    throw ex;
                }
            }


            return hasCols;
        }


        public DynamicParameters GetParamsRef(List<string> keys, DataRow row, Model.EACmpCategory model, Model.GSPUser user)
        {
            DynamicParameters p = new DynamicParameters();
            var arr = new List<object>();
            p.Add("0", Guid.NewGuid().ToString());
            p.Add("1", user.Id);
            p.Add("2", user.Name);
            p.Add("3", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            p.Add("4", model.DWBH);
            p.Add("5", model.ID);
            p.Add("6", "0");
            int i = 7;
            foreach (string key in keys)
            {
                p.Add(i.ToString(), row[key].ToString());
                i++;
            }

            return p;
        }
        #endregion


        #region 确认点击上传关联表数据 并调用存储过程
        //上传关联数据自定义
        public void UploadDataRef(Model.EACmpCategory model, string ExcelData, string DelData, Action<string> onProgress, Action<string> onError, Action<string> onScucess)
        {

            try
            {
                List<string> keys = new List<string>();
                List<Dictionary<string, object>> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(ExcelData);
                if (list.Count == 0)
                {
                    onError("没有上传选中数据");
                    return;
                }
                var sql = GetUpdateRefSql(model, out keys);
                int count = 0;

                using (var conn = DataBaseManager.GetDbConnection())
                {
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {

                        foreach (Dictionary<string, object> row in list)
                        {
                            count++;
                            var para = GetParamsRefUpdate(keys, row);
                            conn.Execute(sql, para, transaction);

                            conn.Execute("update EARefTable set Flag='1' where id=" + token + "ID", new { ID = row["ID"].ToString() }, transaction);
                        }

                        DealProcNew(model.RefProc, model, list, conn, transaction);
                        DealAssExtend("ref", list, model, conn, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }

                var logInfo = $"上传关联数据成功{count.ToString()}条";
                WriteLog(logInfo, model.ID);
                onScucess(logInfo);
            }
            catch (Exception ex)
            {
                WriteLogFile("错误上传关联数据" + ex.ToString());
                onError(ex.Message);
            }
        }




        private string GetUpdateRefSql(Model.EACmpCategory model, out List<string> keys)
        {
            StringBuilder sb = new StringBuilder();
            keys = new List<string>();
            sb.AppendFormat("update EARefTable set ");

            for (var i = 1; i <= 50; i++)
            {
                var field = "XM" + i.ToString().PadLeft(2, '0');
                if (i == 50)
                {

                    sb.Append(field + "=" + token + i.ToString() + " ");
                }
                else
                {
                    sb.Append(field + "=" + token + i.ToString() + ",");
                }
                keys.Add(field);
            }

            sb.Append(" where id=" + token + "0");
            return sb.ToString();
        }

        public DynamicParameters GetParamsRefUpdate(List<string> keys, Dictionary<string, object> row)
        {
            DynamicParameters p = new DynamicParameters();

            p.Add("0", row["ID"].ToString());
            int i = 1;
            foreach (string key in keys)
            {
                var value = "";
                if (row.ContainsKey(key))
                {
                    if (row[key] != null)
                    {
                        value = row[key].ToString();
                    }
                    p.Add(i.ToString(), value);
                    i++;
                }

            }

            return p;
        }
        #endregion



        #region DLL 事件扩展调用
        public void DealAssExtend(string type, List<Dictionary<string, object>> list, Model.EACmpCategory model, IDbConnection db, IDbTransaction trans)
        {

            if (string.IsNullOrEmpty(model.Tmp.ImprtDLL) || model.Tmp.ImprtDLL.Length < 2) return;
            var svr = (IExcelExtend)Activator.CreateInstance(Type.GetType(model.Tmp.ImprtDLL, false, true));

            if (svr == null)
                throw new Exception("未能获取到dll信息");
            switch (type)
            {
                case "upload":
                    svr.AfterUpload(list, model, db, trans);
                    break;
                case "cancel":
                    svr.AfterCancel(list, model, db, trans);
                    break;
                case "custom":
                    svr.CustomBtnClick(list, model, db, trans);
                    break;
                case "ref":
                    svr.AfterRefUpload(list, model, db, trans);
                    break;
                case "refdel":
                    svr.AfterRefDelete(list, model, db, trans);
                    break;
                default:
                    break;
            }
        }
        #endregion



        #region 处理存储过程调用

        public void DealProcNew(string procName, Model.EACmpCategory model, List<Dictionary<string, object>> list, IDbConnection conn, IDbTransaction trans)
        {
            if (string.IsNullOrEmpty(procName) || procName.Length < 2) return;
            var refid = "";
            //foreach (var item in list)
            //{
            //    refid += item["ID"].ToString() + ",";
            //}

            var user = UserService.GetUser();

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("UserID", user.Id);
            dict.Add("UserName", user.Name);
            dict.Add("YWID", model.ID);
            dict.Add("DATAID", refid);
            dict.Add("DWBH", model.DWBH);
            dict.Add("TMPTABLE", model.TmpTab);//增加 luchg 20190510 
            ProcHelper.ExecProc(dict, procName, conn, trans);
            //Db.Execute($"exec {procName} @0,@1,@2 ", model.ID, refid, UserService.GetUserId());
        }


        #endregion
        #region 处理存储过程调用日期版本

        public void DealProcNewRq(string Qsrq, string Jsrq, string procName, Model.EACmpCategory model, List<Dictionary<string, object>> list, IDbConnection conn, IDbTransaction trans)
        {
            if (string.IsNullOrEmpty(procName) || procName.Length < 2) return;
            var refid = "";
            //foreach (var item in list)
            //{
            //    refid += item["ID"].ToString() + ",";
            //}

            var user = UserService.GetUser();

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("UserID", user.Id);
            dict.Add("UserName", user.Name);
            dict.Add("YWID", model.ID);
            dict.Add("DATAID", refid);
            dict.Add("DWBH", model.DWBH);
            dict.Add("TMPTABLE", model.TmpTab);//增加 luchg 20190510
            dict.Add("QSRQ", Qsrq);//增加 luchg 20190510 
            dict.Add("JSRQ", Jsrq);//增加 luchg 20190510 
            ProcHelper.ExecProc(dict, procName, conn, trans);
            //Db.Execute($"exec {procName} @0,@1,@2 ", model.ID, refid, UserService.GetUserId());
        }


        #endregion


        #region 记录操作日志
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
        #endregion

        #region 记录操作日志文件版本
        public void WriteLogFile(string txt)
        {
            string path = @"C:\InterFace\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                DateTime now = DateTime.Now;
                StreamWriter writer = new StreamWriter(path + "GS_InterFace_Log" + now.ToString("yyyyMMdd") + ".txt", true, Encoding.Default);
                writer.WriteLine(string.Concat(new object[] { now, "      :      ", txt, "\r\n" }));
                writer.Close();
            }
            catch (Exception)
            {
            }
        }

        #endregion




    }
}
