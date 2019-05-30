using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraPrinting;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraGrid;
using DevExpress;
using System.Drawing.Printing;

namespace ExcelClient
{
    public class DevPrint
    {
        public DevPrint()
        {
        }
        PrintingSystem _printingSystem = null;
        PrintableComponentLink _printableComponentLink = null;
        private IPrintable _PrtCtrl;
        private string _PrintSettingFile;
        private string _printID = "";


        public IPrintable PrtCtrl
        {
            set { this._PrtCtrl = value; }
            get { return this._PrtCtrl; }
        }
        public string PrintID
        {
            set { this._printID = value; }
            get { return this._printID; }
        }
        
        private void DoPrint(bool isPreview)
        {
            try
            {
                Cursor.Current = Cursors.AppStarting;
                if (PrintID == "") return;
                _PrintSettingFile = Application.StartupPath + "\\" + DevBBTitleFontMgr.ConstPath + "\\" + PrintID + ".xml";
                _printingSystem = new PrintingSystem();
                _printableComponentLink = new PrintableComponentLink(_printingSystem);
                _printableComponentLink.Component = PrtCtrl;
                 
                _printingSystem.Links.Add(_printableComponentLink);
                _printingSystem.PageSettingsChanged -= new EventHandler(ps_PageSettingsChanged);
                LoadPageSetting();
                _printingSystem.PageSettingsChanged += new EventHandler(ps_PageSettingsChanged);
                _printableComponentLink.AfterCreateAreas += new EventHandler(ps_PageSettingsChanged);
                _printingSystem.AfterMarginsChange += new MarginsChangeEventHandler(ps_AfterMarginsChange);
                _printableComponentLink.PaperKind = _printingSystem.PageSettings.PaperKind;
                _printableComponentLink.Margins = _printingSystem.PageSettings.Margins;
                _printableComponentLink.Landscape = _printingSystem.PageSettings.Landscape;
                _printableComponentLink.CreateDocument();
                if (isPreview)
                {
                    _printingSystem.PreviewFormEx.Show();
                }
                else
                {
                    _printingSystem.Print();
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// 进行打印
        /// </summary>
        public void Print()
        {
            DoPrint(false);
        }

        /// <summary>
        /// 打印预览
        /// </summary>
        public void Preview()
        {
            DoPrint(true);
        }


        #region 保存、加载打印设置
        private void ps_AfterMarginsChange(object sender, MarginsChangeEventArgs e)
        {
            SavePageSetting();
        }
        private void _printableComponentLink_CreateReportFooterArea(object sender, CreateAreaEventArgs e)
        {
            SavePageSetting();
        }
        private void ps_PageSettingsChanged(object sender, EventArgs e)
        {
            SavePageSetting();
        }

        /// <summary>
        /// 获取页面设置信息
        /// </summary>
        private void LoadPageSetting()
        {
            try
            {
                if (!System.IO.File.Exists(_PrintSettingFile)) return;
                XmlSerializer ser = new XmlSerializer(typeof(PrintPageSetting));
                PrintPageSetting setting = (PrintPageSetting)ser.Deserialize(new FileStream(_PrintSettingFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                Margins m = new Margins(setting.Left, setting.Right, setting.Top, setting.Bottom);
                _printingSystem.PageSettings.Assign(m, (PaperKind)setting.PaperKind, setting.Landscape);
                _printingSystem.PageSettings.PrinterName = setting.PrinterName;
                System.Drawing.Font headerfont=new System.Drawing.Font(setting.headerFont.Name,setting.headerFont.Size,setting.headerFont.Style);
                string[] arrheader = getStringArray(setting.headerContent);
                PageHeaderArea header = new PageHeaderArea(arrheader, headerfont, setting.headerAlign);

                System.Drawing.Font footerfont = new System.Drawing.Font(setting.footerFont.Name, setting.footerFont.Size,setting.footerFont.Style);
                string[] arrfoot = getStringArray(setting.footerContent);
                PageFooterArea footer = new PageFooterArea(arrfoot, footerfont, setting.footerAlign);
                PageHeaderFooter pagehf = new PageHeaderFooter(header, footer);
                _printableComponentLink.PageHeaderFooter = pagehf;

            }
            catch { }
        }
        private string[] getStringArray(System.Collections.Specialized.StringCollection coll)
        {
            string[] strarr = new string[coll.Count]  ;
            for (int i=0;i<coll.Count ;i++ )
            {
                strarr[i] = coll[i];
            }
            return strarr;
        }

        /// <summary>
        /// 保存当前网格的布局
        /// </summary>
        private void SavePageSetting()
        {
            try
            {
                XtraPageSettings setting = _printingSystem.PageSettings;
                PrintPageSetting pps = new PrintPageSetting();
                pps.Landscape = setting.Landscape;
                pps.Left = setting.Margins.Left;
                pps.Right = setting.Margins.Right;
                pps.Top = setting.Margins.Top;
                pps.Bottom = setting.Margins.Bottom;
                pps.PaperKind = (int)setting.PaperKind;
                pps.PrinterName = setting.PrinterName;
                PageHeaderFooter pageHf = _printableComponentLink.PageHeaderFooter as PageHeaderFooter;
                pps.headerAlign = pageHf.Header.LineAlignment;
                pps.headerContent = pageHf.Header.Content;
                pps.headerFont.Name = pageHf.Header.Font.Name;
                pps.headerFont.Size = pageHf.Header.Font.Size;
                pps.headerFont.Style = pageHf.Header.Font.Style;
                
                pps.footerAlign = pageHf.Footer.LineAlignment;
                pps.footerContent = pageHf.Footer.Content;
                pps.footerFont.Name = pageHf.Footer.Font.Name;
                pps.footerFont.Size = pageHf.Footer.Font.Size;
                pps.footerFont.Style=  pageHf.Footer.Font.Style;
             
                XmlSerializer ser = new XmlSerializer(pps.GetType());
                ser.Serialize(new FileStream(_PrintSettingFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), pps);
            }
            catch (Exception ex) { }
        }
        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class PrintPageSetting
    {
        public PrintPageSetting()
        {
        }
        public bool Landscape;
        public int PaperKind;
        //public string  PaperName;
        public string PrinterName;
        public int Top;
        public int Bottom;
        public int Left;
        public int Right;
        //header set
        public System.Collections.Specialized.StringCollection headerContent;        
        public PrintFont headerFont = new PrintFont();
        public BrickAlignment headerAlign;
        //footer set 
        public System.Collections.Specialized.StringCollection footerContent;     
        public PrintFont footerFont = new PrintFont();
        public BrickAlignment footerAlign;
    }
    [Serializable()]
    public class PrintFont
    {
        public PrintFont()
        {
        }
        public string Name;
        public float Size;
        public System.Drawing.FontStyle Style;
    }
}
