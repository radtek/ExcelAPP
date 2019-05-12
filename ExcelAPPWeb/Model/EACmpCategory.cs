using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelAPPWeb.Model
{
    [TableName("EACmpCategory")]
    [PrimaryKey("ID", AutoIncrement = false)]
    public class EACmpCategory
    {

        /// <summary>
        /// 规则id 业务标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 全局模板ID
        /// </summary>
        public string CID { get; set; }

        /// <summary>
        ///名称
        /// </summary>
        public string RNAME { get; set; }


        /// <summary>
        /// 单位编号
        /// </summary>
        public string DWBH { get; set; }



        public string BMBH { get; set; }


        /// <summary>
        /// 临时表名称
        /// </summary>
        public string TmpTab { get; set; }


        //1 excel 0存储过程
        public string ImprtType { get; set; }

        //上传后执行存储过程
        public string ImprtProc { get; set; }

        //取消上传后执行存储过程
        public string CancelProc { get; set; }


        public string Note { get; set; }

        public string StartLine { get; set; }


        /// <summary>
        /// 关联表上传存储过程
        /// </summary>
        public string RefProc { get; set; }

        /// <summary>
        /// 关联表删除存储过程
        /// </summary>

        public string RefRemoveProc { get; set; }

        /// <summary>
        /// 是否启用关联
        /// </summary>
        public string IsREF { get; set; }


        /// <summary>
        ///  关联表起始行
        /// </summary>
        public string RefStart { get; set; }


        /// <summary>
        /// 是否显示自定义按钮
        /// </summary>
        public string IsCustom { get; set; }

        /// <summary>
        /// 自定义按钮调用执行存储过程
        /// </summary>
        public string CustomProc { get; set; }


        /// <summary>
        /// 自定义按钮名称
        /// </summary>
        public string CustomName { get; set; }

        /// <summary>
        /// 关联帮助说明
        /// </summary>
        public string REFHELP { get; set; }
        /// <summary>
        /// 上传帮助
        /// </summary>
        public string UPURL { get; set; }

        public string CreateUser { get; set; }
        public string CreateTime { get; set; }
        public string LastModifyUser { get; set; }

        public string LastModifyTime { get; set; }
        [Ignore]
        public List<EACmpCateCols> Cols { get; set; }
        [Ignore]
        public EACategory Tmp { get; set; }


    }

    [TableName("EACmpCateCols")]
    [PrimaryKey("ID", AutoIncrement = false)]
    public class EACmpCateCols
    {
        public string ID { get; set; }

        /// <summary>
        /// 单位规则ID
        /// </summary>
        public string RID { get; set; }

        /// <summary>
        /// 字段编号
        /// </summary>
        public string FCode { get; set; }


        /// <summary>
        /// 字段显示名
        /// </summary>
        public string FName { get; set; }

        /// <summary>
        /// 对应excel列名称
        /// </summary>
        public string MatchName { get; set; }


        public string MatchRule { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DeafultValue { get; set; }


        /// <summary>
        /// 计算SQL (A=B) WHERE 1=1  我后边会自动增加本地导入条件
        /// </summary>
        public string CalcSQL { get; set; }



        /// <summary>
        /// 是否显示
        /// </summary>
        public string IsShow { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string FType { get; set; }
        public string IsReadOnly { get; set; }


        public string IsRequire { get; set; }

        public string IsMatch { get; set; }
      
        public string HelpID { get; set; }


        public string HelpFitler { get; set; }

        public string SCols { get; set; }
        public string RCols { get; set; }

        public string SortOrder { get; set; }

        public string Width { get; set; }

        public int FPrec { get; set; }

        public int FLength { get; set; }
        public string BindCol { get; set; }


    }

}