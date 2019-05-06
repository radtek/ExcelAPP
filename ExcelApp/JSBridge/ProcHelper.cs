using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using System.Web; 
using Oracle.ManagedDataAccess.Client;

namespace ExcelApp.JSBridge
{
    public class ProcHelper
    {

        public static List<IDictionary<string, object>> GetProcDataOracle(Dictionary<string, object> paramsInfo, string procName, IDbConnection conn)
        {
            var res = new List<IDictionary<string, object>>();

            var p = new OracleDynamicParameters();
            foreach (var item in paramsInfo)
            {
                p.Add(item.Key, item.Value);
            }
            p.Add("Re_CURSOR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var list = conn.Query(procName, param: p, commandType: CommandType.StoredProcedure);
            res = list.Select(x => (IDictionary<string, object>)x).ToList();
            return res;
        }
        public static List<IDictionary<string, object>> GetProcDataSQL(Dictionary<string, object> paramsInfo, string procName, IDbConnection conn)
        {
            var res = new List<IDictionary<string, object>>();

            var p = new DynamicParameters();
            foreach (var item in paramsInfo)
            {
                p.Add(item.Key, item.Value);
            }
            var list = conn.Query(procName, param: p, commandType: CommandType.StoredProcedure);
            res = list.Select(x => (IDictionary<string, object>)x).ToList();
            return res;
        }

        public static int ExecProc(Dictionary<string, object> paramsInfo, string procName, IDbConnection conn, IDbTransaction trans)
        {
            var p = new DynamicParameters();
            foreach (var item in paramsInfo)
            {
                p.Add(item.Key, item.Value);
            }
            var res = conn.Execute(procName, param: p, commandType: CommandType.StoredProcedure, transaction: trans);
            return res;
        }
    }
}
