using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelAPPWeb.Model
{


    /// <summary>
    /// 帮助定义
    /// </summary>
    [TableName("EAFunc")]
    [PrimaryKey("ID", AutoIncrement = false)]
    public class EAFunc
    {
        public string ID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string IsDetail { get; set; }
        public string PId { get; set; }
        public string REFID { get; set; }

        /// <summary>
        /// 打开类型 1 规则导入 2 url 3 winform
        /// </summary>
        public string REFType { get; set; }


        /// <summary>
        /// url信息
        /// </summary>
        public string URLInfo { get; set; }


        /// <summary>
        /// form窗体信息
        /// </summary>
        public string FormInfo { get; set; }
    }
}
