using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelDataBase
{
    public enum DbParamLayout
	{
        /// <summary>
        /// 通过名字确定，名字是唯一标示
        /// </summary>
        Named,
        /// <summary>
        /// 通过位置顺序确定 
        /// </summary>
        Positional
	}
}
