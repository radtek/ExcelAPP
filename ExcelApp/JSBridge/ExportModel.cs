using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelApp.JSBridge
{
    public class ExportModel
    {

        public string sheetName { get; set; }

        public DataTable dt { get; set; }
    }
}
