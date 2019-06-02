using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelDataBase
{
    public enum DataBaseType : int
    {
        /// <summary>
        /// 不存在的数据库类型
        /// </summary>
        NONE=-1,
        /// <summary>
        /// SQL SERVER
        /// </summary>
        MSS = 0,
        /// <summary>
        /// ORACLE
        /// </summary>
        ORA = 1,
        /// <summary>
        /// mysql
        /// </summary>
        MySql = 2,
        /// <summary>
        /// ole
        /// </summary>
        OLEDB = 3,
        /// <summary>
        /// sqlLite
        /// </summary>
        SqlLite =4


    }
}
