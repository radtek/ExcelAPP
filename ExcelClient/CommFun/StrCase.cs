using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelClient
{
   public class StrCase
    {
    
       public static string strConvert(string str)
        {
            str = str.Replace("\"", "“");
            str = str.Replace("'", "‘");
            str = str.Replace(")", "）");
            str = str.Replace("(", "（");
            str = str.Replace("--", "－－");
            str = str.Replace("|", "｜");
            str = str.Replace("`", "·");
            str = str.Replace("^", "……");
            str = str.Replace("~", "～");
            return str;
        }
   }
}
