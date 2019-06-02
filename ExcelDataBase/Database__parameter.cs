using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
 

namespace ExcelDataBase
{
    /// <summary>
    /// ��������
    /// </summary>
    public partial class Database
    {
        ///
        ///����ʹ��dbprovider�ṩ��MakeParam��������
        ///



        #region private static object[] GetParamValues(DbCommand cmd)

        /// <summary>
        /// ��dbCommand�����л�ȡ����ֵ
        /// </summary>
        /// <param name="cmd">DbCommand����</param>
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
        ///// ת��sql��������
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
        /// ת����������
        /// </summary>
        /// <param name="param">DbParam���͵Ĳ�������</param>
        /// <returns>IDbDataParameter���͵Ĳ�������</returns>
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
