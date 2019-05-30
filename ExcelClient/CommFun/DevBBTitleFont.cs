using System;
using System.Drawing;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;

namespace ExcelClient
{
    public class DevBBTitleFontMgr
    {
        //public DevBBTitleFontMgr()
        //{
        //}
        public const string ConstPath = "Config";
        public static DevBBTitleFont getFont(string id)
        {
            DevBBTitleFont titleFont = new DevBBTitleFont();
            try
            {
                string _FontFile = Application.StartupPath + "\\" + ConstPath + "\\Font" + id + ".xml";
                if (System.IO.File.Exists(_FontFile))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(DevBBTitleFont));
                    titleFont = (DevBBTitleFont)ser.Deserialize(new FileStream(_FontFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                }
            }
            catch { }
            return titleFont;
        }
        public static void saveFont(DevBBTitleFont titleFont,string id){
            try
            {
                string _FontFile = Application.StartupPath + "\\" + ConstPath + "\\Font" + id + ".xml";
                XmlSerializer ser = new XmlSerializer(titleFont.GetType());
                ser.Serialize(new FileStream(_FontFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), titleFont);
            }
            catch
            {
            }
        }
    }

   [Serializable()]
   public  class DevBBTitleFont
    {
       public DevBBTitleFont()
       {
           titleFont.Name = "宋体";
           titleFont.Size = 14;
           titleFont.Style = FontStyle.Bold;

           subTitleFont.Name = "宋体";
           subTitleFont.Size = 12;
           subTitleFont.Style = FontStyle.Regular;

           columnFont.Name = "宋体";
           columnFont.Size = 9;
           columnFont.Style = FontStyle.Regular;

           rowFont.Name = "宋体";
           rowFont.Size = 9;
           rowFont.Style = FontStyle.Regular;
       }
       public PrintFont titleFont = new PrintFont();
       public PrintFont subTitleFont = new PrintFont();
       public PrintFont columnFont = new PrintFont();
       public PrintFont rowFont = new PrintFont();
    }
}
