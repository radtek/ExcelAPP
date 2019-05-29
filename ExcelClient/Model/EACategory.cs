using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelClient.Model
{
    /// <summary>
    /// 导入类别定义
    /// </summary>
    /// 
    [TableName("EACategory")]
    [PrimaryKey("ID", AutoIncrement = false)]
    public class EACategory
    {
        public string ID { get; set; }

        public string RNAME { get; set; }

        /// <summary>
        /// 临时表名
        /// </summary>
        public string TmpTab { get; set; }

        /// <summary>
        /// 导入类型1 存储过程 0 excel
        /// </summary>
        public string ImprtType { get; set; }


        /// <summary>
        /// 上传后执行存储过程
        /// </summary>
        public string ImprtProc { get; set; }

        /// <summary>
        ///  取消后执行存储过程
        /// </summary>
        public string CancelProc { get; set; }




        /// <summary>
        /// 本地数据源取数执行存储过程
        /// </summary>
        public string GetProc { get; set; }


        /// <summary>
        /// DLL 事件扩展 
        /// </summary>
        public string ImprtDLL { get; set; }


        [Ignore]
        public List<EACatCols> Cols { get; set; }


        public string Note { get; set; }
        public string CreateUser { get; set; }
        public string CreateTime { get; set; }
        public string LastModifyUser { get; set; }

        public string LastModifyTime { get; set; }

    }

    /// <summary>
    /// 导入列配置定义
    /// </summary>
    [TableName("EACatCols")]
    [PrimaryKey("ID", AutoIncrement = false)]
    public class EACatCols
    {
        public string ID { get; set; }
        public string RID { get; set; }
        public string FCode { get; set; }

        public string FName { get; set; }
        public string FType { get; set; }
        public string IsShow { get; set; }
        public string FLength { get; set; }
        public string IsReadonly { get; set; }
        public string HelpID { get; set; }

        public string IsMatch { get; set; }
        public string IsRequire { get; set; }
        public string RCols { get; set; }
        public string SCols { get; set; }

    }

}