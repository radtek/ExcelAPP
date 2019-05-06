using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelAPPWeb.Model
{
    [TableName("EAOpLog")]
    [PrimaryKey("ID", AutoIncrement = false)]
    public class EAOpLog
    {
      
        public string ID { get; set; }
        public string RID { get; set; }

        public string UserCode { get; set; }

        public string OpInfo { get; set; }
        public string OpTime { get; set; }


    }
}
