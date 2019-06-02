using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ExcelDataBase
{
    public class DbModifyFunc
    {
        //根据GSYCOL生成建表字符串
        public static string GenerateCol(DataTable dt, string dbname, out string key)
        {
            Database db = DbFunction.GetDB(dbname);
            var isOraDb = db.DBType == DataBaseType.ORA; //是否oracle数据库

            StringBuilder sql = new StringBuilder();
            key = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                sql.AppendFormat("{0} ", dr["GSYCOL_FIELDID"]);

                //字段类型判断
                var colType = dr["GSYCOL_TYPE"].ToString();
                string vstype = db.Provider.getDataType(colType);
                switch (colType.ToLower())
                {
                    case "char":
                    case "varchar":
                        sql.AppendFormat("{0}({1})", vstype, dr["GSYCOL_LEN"]);
                        break;
                    case "int":
                    case "datetime":
                    case "blob":
                    case "clob":
                        sql.AppendFormat("{0}", vstype);
                        break;                    
                    case "decimal":
                        sql.AppendFormat("{0}({1},{2})", vstype, dr["GSYCOL_LEN"], dr["GSYCOL_PREC"]);
                        break;
                    default:
                        break;
                }

                //默认值
                if(!string.IsNullOrEmpty(Convert.ToString(dr["GSYCOL_DEF"])))
                {
                    sql.AppendFormat(" default {0}", dr["GSYCOL_DEF"]);
                }
                //是否为空
                if(dr["GSYCOL_NULLABLE"].ToString() == "0")
                    sql.Append(" not null,");
                else
                    sql.Append(" null,");

                //是否主键
                if (dr["GSYCOL_ISKEY"].ToString() == "1")
                    key = string.Format("{0},{1}", key, dr["GSYCOL_FIELDID"]);
                

            }
            if (key.Length > 0) key = key.Substring(1);
            return sql.ToString();
        }

        //创建表, 检查是否在数据库里存在这个表, 如果有的话, 判断是否有数据, 有数据则不允许删除
        public static bool CreateTable(string dbname, string tableName, string Columns, string Key, out string msg)
        {
            msg = string.Empty;
            //如果已经存在就返回
            if (CheckTableExist(dbname, tableName))
            {
                msg = "表"+ tableName + "已经存在";
                return false;
            }
            var sql = string.Empty;
            if(Key.Length>0)
                sql = string.Format("create table {0}({1} CONSTRAINT PK_{0} PRIMARY KEY({2}))", tableName, Columns, Key);
            else
                sql = string.Format("create table {0}({1})", tableName, Columns.Remove(Columns.Length-1));
            try
            {
                DbFunction.ExecuteNonQuery(dbname, sql);
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }

        //强制删除并创建表
        public static bool CreateTableForced(string dbname, string tableName, string Columns, string Key, out string msg)
        {

            msg = string.Empty;
            string vsDroptable = string.Empty;

            //如果已经存在就返回
            if (CheckTableExist(dbname, tableName))
            {
                vsDroptable = string.Format("drop table {0}", tableName);
                return false;
            }

            var sql = string.Empty;
            if (Key.Length > 0)
                sql = string.Format("create table {0}({1} CONSTRAINT PK_{0} PRIMARY KEY({2}))", tableName, Columns, Key);
            else
                sql = string.Format("create table {0}({1})", tableName, Columns.Remove(Columns.Length - 1)); 
            
            try
            {
                if (vsDroptable != string.Empty)
                    DbFunction.ExecuteNonQuery(dbname, vsDroptable);
                DbFunction.ExecuteNonQuery(dbname, sql);
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }

        //判断表在数据库中是否存在
        public static bool CheckTableExist(string dbname, string tableName)
        {
            Database db = DbFunction.GetDB(dbname);
            string sql = string.Empty;
            if (db.DBType == DataBaseType.ORA)
            {
                sql = "SELECT COUNT(1) FROM USER_TABLES WHERE TABLE_NAME = UPPER('"+tableName+"')";
            }
            else
            {
                sql = "SELECT COUNT(1) from sysobjects where Upper(name) = Upper('" + tableName + "') and type = 'U'";
            }

            int count = Convert.ToInt32(DbFunction.ExecuteScalar(dbname, sql));
            if (count == 0)
                return false;
            else
                return true;
        }

        //修改表中的字段
        public static bool ModifyColumn(string dbname, string tableName, string colName, string colType, string colLenght, string defaultValue, string nullable, out string msg)
        {
            Database db = DbFunction.GetDB(dbname);
            
            msg = string.Empty;

            //检查是否存在列信息
            if (!CheckColumnExist(dbname, tableName, colName))
            {
                msg = "列" + colName + "不存在";
                return false;
            }
            var isOraDb = db.DBType == DataBaseType.ORA;

            string sql = string.Format("alter table {0} alter column {1} ", tableName, colName);
            if(isOraDb)
                sql = string.Format("alter table {0} modify ( {1} ", tableName, colName);

            sql += getColTypeStr(db, colType, colLenght); 
            //默认值
            if (!string.IsNullOrEmpty(defaultValue))
            {
                sql += string.Format(" default {0}", defaultValue);
            }
            //是否为空
            if (nullable == "0")
                sql +=" not null";
            else
                sql +=" null";
            if(isOraDb)
                sql+=")";
            try
            {
                DbFunction.ExecuteNonQuery(dbname, sql);
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }

        //检查表中是否存在指定的列
        public static bool CheckColumnExist(string dbname, string tableName, string colName)
        {
            Database db = DbFunction.GetDB(dbname);
            string sql = string.Empty;
            if (db.DBType == DataBaseType.ORA)
            {
                sql = "SELECT COUNT(1) as EXISTFLAG FROM USER_TAB_COLS WHERE COLUMN_NAME=UPPER('" + colName + "') AND TABLE_NAME=UPPER('" + tableName + "')";
            }
            else
            {
                sql = "SELECT COUNT(1) from syscolumns c,sysobjects o where o.id=c.id and Upper(o.name) = Upper('" + tableName + "') and o.type = 'U' and Upper(c.name) = Upper('" + colName + "')";
            }

            int count = Convert.ToInt32(DbFunction.ExecuteScalar(dbname, sql));
            if (count == 0)
                return false;
            else
                return true;
        }

        //增加表中的列
        public static bool AddColumn(string dbname, string tableName, string colName, string colType, string colLenght, string defaultValue, string nullable, out string msg)
        {
            Database db = DbFunction.GetDB(dbname);
            string sql = string.Format("alter table {0} add {1} ", tableName, colName);
            msg = string.Empty;

            //检查是否存在列信息
            if (CheckColumnExist(dbname, tableName, colName))
            {
                msg = "列" + colName + "已经存在";
                return false;
            }
           
            sql+=getColTypeStr(db,colType,colLenght);           
            //默认值
            if (!string.IsNullOrEmpty(defaultValue))
            {
                sql += string.Format(" default {0}", defaultValue);
            }
            //是否为空
            if (nullable == "0")
                sql += " not null";
            else
                sql += " null";


            try
            {
                DbFunction.ExecuteNonQuery(dbname, sql);
                return true;
            }
            catch(Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }
        private static string getColTypeStr(Database db,string colType,string colLenght){
            string sql="";
            string vstype = db.Provider.getDataType(colType);
            switch (colType.ToLower())
            {
                case "char":
                case "varchar":
                case "decimal":
                    sql += string.Format("{0}({1})", vstype, colLenght);
                    break;
                case "int":
                case "datetime":  
                case "blob": 
                case "clob":
                    sql += string.Format("{0}", vstype);                   
                    break; 
                default:
                    break;
            }
            return sql;
        }

        //删除列
        public static bool DeleteColumn(string dbname, string tableName, string colName, out string msg)
        {
            Database db = DbFunction.GetDB(dbname);
            string sql = string.Format("alter table {0} drop column {1} ", tableName, colName);
            msg = string.Empty;

            //检查是否存在列信息
            if (!CheckColumnExist(dbname, tableName, colName))
            {
                msg = "列" + colName + "不存在";
                return false;
            }

            try
            {
                DbFunction.ExecuteNonQuery(dbname, sql);
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }        
    }

    public class DataTableOp
    {
        public static DataTable Join(DataTable left, DataTable right,
            DataColumn[] leftCols, DataColumn[] rightCols,
            bool includeLeftJoin, bool includeRightJoin)
        {
            DataTable result = new DataTable("JoinResult");
            //判断left和right的tablename是否一样, 一样的话就修改right的tableName
            if (left.TableName == right.TableName) right.TableName += "_2";
            using (DataSet ds = new DataSet())
            {
                ds.Tables.AddRange(new DataTable[] { left.Copy(), right.Copy() });
                DataColumn[] leftRelationCols = new DataColumn[leftCols.Length];
                for (int i = 0; i < leftCols.Length; i++)
                    leftRelationCols[i] = ds.Tables[0].Columns[leftCols[i].ColumnName];

                DataColumn[] rightRelationCols = new DataColumn[rightCols.Length];
                for (int i = 0; i < rightCols.Length; i++)
                    rightRelationCols[i] = ds.Tables[1].Columns[rightCols[i].ColumnName];

                //create result columns
                for (int i = 0; i < left.Columns.Count; i++)
                    result.Columns.Add(left.Columns[i].ColumnName, left.Columns[i].DataType);
                for (int i = 0; i < right.Columns.Count; i++)
                {
                    string colName = right.Columns[i].ColumnName;
                    while (result.Columns.Contains(colName))
                        colName += "_2";
                    result.Columns.Add(colName, right.Columns[i].DataType);
                }

                //add left join relations
                DataRelation drLeftJoin = new DataRelation("rLeft", leftRelationCols, rightRelationCols, false);
                ds.Relations.Add(drLeftJoin);

                //join
                result.BeginLoadData();
                foreach (DataRow parentRow in ds.Tables[0].Rows)
                {
                    DataRow[] childrenRowList = parentRow.GetChildRows(drLeftJoin);
                    if (childrenRowList != null && childrenRowList.Length > 0)
                    {
                        object[] parentArray = parentRow.ItemArray;
                        foreach (DataRow childRow in childrenRowList)
                        {
                            object[] childArray = childRow.ItemArray;
                            object[] joinArray = new object[parentArray.Length + childArray.Length];
                            Array.Copy(parentArray, 0, joinArray, 0, parentArray.Length);
                            Array.Copy(childArray, 0, joinArray, parentArray.Length, childArray.Length);
                            result.LoadDataRow(joinArray, true);
                        }
                    }
                    else //left join
                    {
                        if (includeLeftJoin)
                        {
                            object[] parentArray = parentRow.ItemArray;
                            object[] joinArray = new object[parentArray.Length];
                            Array.Copy(parentArray, 0, joinArray, 0, parentArray.Length);
                            result.LoadDataRow(joinArray, true);
                        }
                    }
                }

                if (includeRightJoin)
                {
                    //add right join relations
                    DataRelation drRightJoin = new DataRelation("rRight", rightRelationCols, leftRelationCols, false);
                    ds.Relations.Add(drRightJoin);

                    foreach (DataRow parentRow in ds.Tables[1].Rows)
                    {
                        DataRow[] childrenRowList = parentRow.GetChildRows(drRightJoin);
                        if (childrenRowList == null || childrenRowList.Length == 0)
                        {
                            object[] parentArray = parentRow.ItemArray;
                            object[] joinArray = new object[result.Columns.Count];
                            Array.Copy(parentArray, 0, joinArray,
                                joinArray.Length - parentArray.Length, parentArray.Length);
                            result.LoadDataRow(joinArray, true);
                        }
                    }
                }

                result.EndLoadData();
            }

            return result;
        }

        public static void SetDefault(DataTable dt, DataRow dr)
        {

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Columns[i].DataType == typeof(decimal)
                    || dt.Columns[i].DataType == typeof(int)
                    || dt.Columns[i].DataType == typeof(float))
                    if (dr.IsNull(dt.Columns[i]))
                        dr[i] = 0;
            }
        }
    }
}
