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
    [TableName("EAHelp")]
    [PrimaryKey("ID", AutoIncrement = false)]
    public class EAHelp
    {
        public string ID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string SFields { get; set; }
        public string SFilter { get; set; }
        public string HelpType { get; set; }
        public string PathField { get; set; }
        public string LevelField { get; set; }
        public string DetailField { get; set; }
        public string IDField { get; set; }
        public string PIDField { get; set; }
        public string GradeFormat { get; set; }
        public string ShowCols { get; set; }

        public string PageSize { get; set; }

        public string CodeField { get; set; }

        public string NameField { get; set; }
 

    }
}
