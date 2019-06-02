using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
 

namespace ExcelDataBase
{
    /// <summary>
    /// 参数处理
    /// </summary>
    public partial class Database
    {
        ///
        ///参数使用dbprovider提供的MakeParam函数处理
        ///



        #region private static object[] GetParamValues(DbCommand cmd)

        /// <summary>
        /// 从dbCommand对象中获取参数值
        /// </summary>
        /// <param name="cmd">DbCommand对象</param>
        /// <returns></returns>
        private static object[] GetParamValues(DbCommand cmd)
        {
            if (cmd.Parameters != null && cmd.Parameters.Count > 0)
            {
                int count = cmd.Parameters.Count;
                object[] all = new object[count];
                for (int i = 0; i < count; i++)
                {
                    DbParameter p = cmd.Parameters[i];
                    all[i] = p.Value;
                }
                return all;
            }
            else
                return new object[] { };
        }

        #endregion


        ///// <summary>
        ///// 转换sql参数数组
        ///// </summary>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public  IDbDataParameter[] ConvertParam(DbParam[] param)
        //{
        //    IDbDataParameter[] iparam = new IDbDataParameter[param.Length];
        //    for (int i = 0; i < param.Length; i++)
        //    {
        //        DbParam p = param[i];
        //        iparam[i] = this.Provider.MakeParam(p.paramName, p.dataType, p.paramDirct, p.sqlType, p.Size, p.paramValue);
        //    }
        //    return iparam;
        //}

        #region public  IDbDataParameter[] ConvertParams( DbParam[] param)
        /// <summary>
        /// 转换参数类型
        /// </summary>
        /// <param name="param">DbParam类型的参数数组</param>
        /// <returns>IDbDataParameter类型的参数数组</returns>
        public IDbDataParameter[] ConvertParams( DbParam[] param)
        {
            if (null == param) return null;
            ArrayList  arr = new ArrayList();
            foreach (DbParam p in param)
            {
                IDbDataParameter vsparam = this.Provider.MakeParam(p.paramName, p.dataType, p.paramDirct, p.sqlType, p.Size, p.paramValue);
                if (vsparam != null)
                {
                    arr.Add(vsparam);
                }
            }
            IDbDataParameter[] rearr = new IDbDataParameter[arr.Count];
            for (int i = 0; i < arr.Count; i++)
                rearr[i] = arr[i] as IDbDataParameter;
            return rearr;
        }
        #endregion


    }
}
