using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelDataBase
{
     public enum DataType:int
    {
        /// <summary> 未识别类型 </summary>
        UnKnown = -2,
        /// <summary> 默认类型 </summary>
        Default = -1,
        /// <summary> 字符类型 </summary>
        Char = 0,
        /// <summary> 可变字符类型 </summary>
        VarChar = 1,
        /// <summary> 整数类型 </summary>
        Int = 2,
        /// <summary> 浮点类型 </summary>
        Decimal = 3,
        /// <summary> 二进制类型 </summary>
        Blob = 4,
        /// <summary> 日期类型 </summary>
        DateTime = 5,
        /// <summary> 游标类型 </summary>
        Cursor = 6,
        /// <summary>
        /// 长文本
        /// </summary>
        Clob = 7 
    }
}
