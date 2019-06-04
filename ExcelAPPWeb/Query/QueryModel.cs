using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcelAPPWeb.Query
{
    public class JTPUBQRDEF
    {
        public string JTPUBQRDEF_ID { get; set; }
        public string JTPUBQRDEF_BH { get; set; }
        public string JTPUBQRDEF_MC { get; set; }
        public string JTPUBQRDEF_TITLE { get; set; }

        public string JTPUBQRDEF_SUBTIL { get; set; }
        public string JTPUBQRDEF_TYPE { get; set; }

        public string JTPUBQRDEF_SQL { get; set; }

        public string JTPUBQRDEF_ORA { get; set; }

        public string JTPUBQRDEF_WHERE { get; set; }


        public List<QueryFiter> filter { get; set; }
    }

    public class QueryFiter
    {

        public string FieldName { get; set; }
        public string DisplayName { get; set; }

        public string InputType { get; set; }


        public string IsRequired { get; set; }

        /// <summary>
        /// 帮助定义
        /// </summary>
        public string GetInfoFrom { get; set; }

        /// <summary>
        /// 帮助过滤条件
        /// </summary>
        public string GetInfoWhere { get; set; }

        public string DefaultValue { get; set; }

        public string CMP { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public string IsDisplay { get; set; }
    }
}