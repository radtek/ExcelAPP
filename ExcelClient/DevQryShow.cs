using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
 


namespace ExcelClient
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class DevQryShow
    {

        public DevQryShow()
        {
        }


        public   const string CultureInfo = "zh-cn";
        public void Show()
        {
            frmDevQryShow frm = new frmDevQryShow();
            frm.PsTitle = PsTitle;
            frm.PsID = PsID;
            frm.PsYear = PsYear;
            frm.PsJEJD = PsJEJD;
            frm.PsSLJD = PsSLJD;
            frm.PsTitleTable = PsTitleTable;
            frm.PsSelect = PsSelect;
            frm.PsSubTitle = PsSubTitle; 
            frm.PsDWBH = PsDWBH;
            frm.PsUsr = this.PsUsr;
            frm.IsPivot = this.IsPivot;
            frm.ProcessID = this.ProcessID;
            frm.OrderKey = this.OrderKey;
            frm.LinkQryID = this.LinkQryID;
            frm.IsSql = this.IsSql;
            frm.ParamArr = this.ParamArr;
            frm.ValueArr = this.ValueArr;
            frm.QryParam = this.QryParam;
            frm.QryValue = this.QryValue;
            frm.IsRepeatDown = this.IsRepeatDown;
            frm.IsUseLocal = this.IsUseLocal;
            frm.Show();             
        }
        //public void showWithParam(string strProcessID, string strDWBH, string strUsr, string strTitle, string strID, string strYear, string strJEJD, string strSLJD, string strTitleTable,
        //    string strSelect,string strSubTitle,string strIsPivot)
        //{        
        //    frmDevQryShow frm = new frmDevQryShow();
        //    frm.PsTitle = strTitle;
        //    frm.PsID = strID;
        //    frm.PsYear = strYear;
        //    frm.PsJEJD = strJEJD;
        //    frm.PsSLJD = strSLJD;
        //    frm.PsTitleTable = strTitleTable;
        //    frm.PsSelect = strSelect;
        //    frm.PsSubTitle = strSubTitle; 
        //    frm.PsDWBH = strDWBH;
        //    frm.PsUsr = strUsr; 
        //    frm.IsPivot = strIsPivot;
        //    frm.ProcessID = strProcessID;
        //    frm.Show();
        //}
         
        public void showWithParam(string strProcessID, string strDWBH, string strUsr, string strTitle, string strID, string strYear, string strJEJD, string strSLJD, string strTitleTable,
            string strSelect, string strSubTitle, string strIsPivot, string strOrderKey )
        {
            frmDevQryShow frm = new frmDevQryShow();
            frm.PsTitle = strTitle;
            frm.PsID = strID;
            frm.PsYear = strYear;
            frm.PsJEJD = strJEJD;
            frm.PsSLJD = strSLJD;
            frm.PsTitleTable = strTitleTable;
            frm.PsSelect = strSelect;
            frm.PsSubTitle = strSubTitle;
            frm.PsDWBH = strDWBH;
            frm.PsUsr = strUsr;
            frm.IsPivot = strIsPivot;
            frm.ProcessID = strProcessID;
            frm.OrderKey = strOrderKey;
            frm.QryParam = this.QryParam;
            frm.QryValue = this.QryValue;

            frm.IsRepeatDown = this.IsRepeatDown;
            frm.IsUseLocal = this.IsUseLocal;
             
            frm.Show();
        }
       
        public void showProc(string strProcessID, string strDWBH, string strUsr, string strTitle, string strID, string strYear, string strJEJD, string strSLJD, string strTitleTable,
            string strSelect, string strSubTitle, string strIsPivot, string vsParamArr, string vsValueArr  )
        {
            frmDevQryShow frm = new frmDevQryShow();
            frm.PsTitle = strTitle;
            frm.PsID = strID;
            frm.PsYear = strYear;
            frm.PsJEJD = strJEJD;
            frm.PsSLJD = strSLJD;
            frm.PsTitleTable = strTitleTable;
            frm.PsSelect = strSelect;
            frm.PsSubTitle = strSubTitle;
            frm.PsDWBH = strDWBH;
            frm.PsUsr = strUsr;
            frm.IsPivot = strIsPivot;
            frm.ProcessID = strProcessID;
            frm.IsSql = "0";
            frm.ParamArr = vsParamArr.Split('^');
            frm.ValueArr = vsValueArr.Split('^');
            frm.QryParam = this.QryParam;
            frm.QryValue = this.QryValue;

            frm.IsRepeatDown = this.IsRepeatDown;
            frm.IsUseLocal = this.IsUseLocal;

            frm.Show();
        }
        public void showWithParamLinkQryID(string strProcessID, string strDWBH, string strUsr, string strTitle, string strID, string strYear, string strJEJD, string strSLJD, string strTitleTable,
            string strSelect, string strSubTitle, string strIsPivot, string strOrderKey,string strLinkQryID)
        {
            frmDevQryShow frm = new frmDevQryShow();
            frm.PsTitle = strTitle;
            frm.PsID = strID;
            frm.PsYear = strYear;
            frm.PsJEJD = strJEJD;
            frm.PsSLJD = strSLJD;
            frm.PsTitleTable = strTitleTable;
            frm.PsSelect = strSelect;
            frm.PsSubTitle = strSubTitle;
            frm.PsDWBH = strDWBH;
            frm.PsUsr = strUsr;
            frm.IsPivot = strIsPivot;
            frm.ProcessID = strProcessID;
            frm.OrderKey = strOrderKey;
            frm.LinkQryID = strLinkQryID;
            frm.IsRepeatDown = this.IsRepeatDown;
            frm.IsUseLocal = this.IsUseLocal;


            frm.QryParam = QryParam;
            frm.QryValue = QryValue;
            frm.Show();
        }
        public void showWithParamUseLocal(string strProcessID, string strDWBH, string strUsr, string strTitle, string strID, string strYear, string strJEJD, string strSLJD, string strTitleTable,
          string strSelect, string strSubTitle, string strIsPivot, string strOrderKey, string strLinkQryID,string strUseLocal)
        {
            frmDevQryShow frm = new frmDevQryShow();
            frm.PsTitle = strTitle;
            frm.PsID = strID;
            frm.PsYear = strYear;
            frm.PsJEJD = strJEJD;
            frm.PsSLJD = strSLJD;
            frm.PsTitleTable = strTitleTable;
            frm.PsSelect = strSelect;
            frm.PsSubTitle = strSubTitle;
            frm.PsDWBH = strDWBH;
            frm.PsUsr = strUsr;
            frm.IsPivot = strIsPivot;
            frm.ProcessID = strProcessID;
            frm.OrderKey = strOrderKey;
            frm.LinkQryID = strLinkQryID;
            frm.IsRepeatDown = this.IsRepeatDown;
            frm.IsUseLocal = strUseLocal;

            frm.QryParam = this.QryParam;
            frm.QryValue = this.QryValue;
            frm.Show();
        }
        public void include()
        {
            frmDevQryShow frm = new frmDevQryShow();
            frm.PsTitle = PsTitle;
            frm.PsID = PsID;
            frm.PsYear = PsYear;
            frm.PsJEJD = PsJEJD;
            frm.PsSLJD = PsSLJD;
            frm.PsTitleTable = PsTitleTable;
            frm.PsSelect = PsSelect;
            frm.PsSubTitle = PsSubTitle;
            frm.PsDWBH = PsDWBH;
            frm.IsPivot = this.IsPivot;
            frm.PsUsr = this.PsUsr;
            frm.ProcessID = this.ProcessID;
            frm.OrderKey = this.OrderKey;
            frm.LinkQryID = this.LinkQryID;
            frm.IsRepeatDown = this.IsRepeatDown;
            frm.IsUseLocal = this.IsUseLocal;
            frm.IsSql = this.IsSql;
            frm.ParamArr = this.ParamArr;
            frm.ValueArr = this.ValueArr;
            frm.QryParam = this.QryParam;
            frm.QryValue = this.QryValue;
            frm.OpenStyle = "show";
            //UIContent dUI = new UIContent(frm, "查询--" + PsTitle, null);
           
            //Genersoft.Platform.AppFrameworkGui.Gui.WorkbenchSingleton.Workbench.ShowView(dUI);            
        }
         
    
        //public void includeWithParam(string strProcessID,string strDWBH,string strUsr, string strTitle, string strID, string strYear, string strJEJD, string strSLJD, string strTitleTable,
        //    string strSelect, string strSubTitle,string strIsPivot)
        //{
        //    frmDevQryShow frm = new frmDevQryShow();
        //    frm.PsTitle = strTitle;
        //    frm.PsID = strID;
        //    frm.PsYear = strYear;
        //    frm.PsJEJD = strJEJD;
        //    frm.PsSLJD = strSLJD;
        //    frm.PsTitleTable = strTitleTable;
        //    frm.PsSelect = strSelect;
        //    frm.PsSubTitle = strSubTitle; 
        //    frm.PsDWBH = strDWBH;
        //    frm.PsUsr = strUsr; 
        //    frm.IsPivot = strIsPivot;

        //    frm.ProcessID = strProcessID;
        //    UIContent dUI = new UIContent(frm, "查询--" + strTitle, null);             
        //    Genersoft.Platform.AppFrameworkGui.Gui.WorkbenchSingleton.Workbench.ShowView(dUI);
        // }
        public void includeWithParam(string strProcessID, string strDWBH, string strUsr, string strTitle, string strID, string strYear, string strJEJD, string strSLJD, string strTitleTable,
              string strSelect, string strSubTitle, string strIsPivot,string strOrderKey)
        {
            frmDevQryShow frm = new frmDevQryShow();
            frm.PsTitle = strTitle;
            frm.PsID = strID;
            frm.PsYear = strYear;
            frm.PsJEJD = strJEJD;
            frm.PsSLJD = strSLJD;
            frm.PsTitleTable = strTitleTable;
            frm.PsSelect = strSelect;
            frm.PsSubTitle = strSubTitle;
            frm.PsDWBH = strDWBH;
            frm.PsUsr = strUsr;
            frm.IsPivot = strIsPivot;
            frm.OrderKey = strOrderKey;
            frm.QryParam = this.QryParam;
            frm.QryValue = this.QryValue;
            frm.ProcessID = strProcessID;
            frm.IsRepeatDown = this.IsRepeatDown;
            frm.IsUseLocal = this.IsUseLocal;
            frm.OpenStyle = "include";
            //UIContent dUI = new UIContent(frm, "查询--" + strTitle, null);
            //Genersoft.Platform.AppFrameworkGui.Gui.WorkbenchSingleton.Workbench.ShowView(dUI);
        }
        public void includeProc(string strProcessID, string strDWBH, string strUsr, string strTitle, string strID,
            string strYear, string strJEJD, string strSLJD, string strTitleTable,
      string strSelect, string strSubTitle, string strIsPivot, string vsParamArr, string vsValueArr)
        {
            frmDevQryShow frm = new frmDevQryShow();
            frm.PsTitle = strTitle;
            frm.PsID = strID;
            frm.PsYear = strYear;
            frm.PsJEJD = strJEJD;
            frm.PsSLJD = strSLJD;
            frm.PsTitleTable = strTitleTable;
            frm.PsSelect = strSelect;
            frm.PsSubTitle = strSubTitle;
            frm.PsDWBH = strDWBH;
            frm.PsUsr = strUsr;
            frm.IsPivot = strIsPivot;
            frm.IsSql = "0"; 
            frm.ParamArr = vsParamArr.Split('^');
            frm.ValueArr = vsValueArr.Split('^');
            frm.QryParam = this.QryParam;
            frm.QryValue = this.QryValue;

            frm.ProcessID = strProcessID;
            frm.IsRepeatDown = this.IsRepeatDown;
            frm.IsUseLocal = this.IsUseLocal;
            frm.OpenStyle = "include";
            //UIContent dUI = new UIContent(frm, "查询--" + strTitle, null);
            //Genersoft.Platform.AppFrameworkGui.Gui.WorkbenchSingleton.Workbench.ShowView(dUI);
        }
        public void includeWithParamLinkQryID(string strProcessID, string strDWBH, string strUsr, string strTitle, string strID, string strYear, string strJEJD, string strSLJD, string strTitleTable,
             string strSelect, string strSubTitle, string strIsPivot, string strOrderKey,string strLinkQryID)
        {
            frmDevQryShow frm = new frmDevQryShow();
            frm.PsTitle = strTitle;
            frm.PsID = strID;
            frm.PsYear = strYear;
            frm.PsJEJD = strJEJD;
            frm.PsSLJD = strSLJD;
            frm.PsTitleTable = strTitleTable;
            frm.PsSelect = strSelect;
            frm.PsSubTitle = strSubTitle;
            frm.PsDWBH = strDWBH;
            frm.PsUsr = strUsr;
            frm.IsPivot = strIsPivot;
            frm.OrderKey = strOrderKey;
            frm.ProcessID = strProcessID;
            frm.LinkQryID = strLinkQryID;

            frm.QryParam = this.QryParam;
            frm.QryValue = this.QryValue;
            frm.IsRepeatDown = this.IsRepeatDown;
            frm.IsUseLocal = this.IsUseLocal;
            frm.OpenStyle = "include";
            //UIContent dUI = new UIContent(frm, "查询--" + strTitle, null);
            //Genersoft.Platform.AppFrameworkGui.Gui.WorkbenchSingleton.Workbench.ShowView(dUI);
        }

        public void includeWithParamUseLocal(string strProcessID, string strDWBH, string strUsr, string strTitle, string strID, string strYear, string strJEJD, string strSLJD, string strTitleTable,
            string strSelect, string strSubTitle, string strIsPivot, string strOrderKey, string strLinkQryID,string strUseLocal)
        {
            frmDevQryShow frm = new frmDevQryShow();
            frm.PsTitle = strTitle;
            frm.PsID = strID;
            frm.PsYear = strYear;
            frm.PsJEJD = strJEJD;
            frm.PsSLJD = strSLJD;
            frm.PsTitleTable = strTitleTable;
            frm.PsSelect = strSelect;
            frm.PsSubTitle = strSubTitle;
            frm.PsDWBH = strDWBH;
            frm.PsUsr = strUsr;
            frm.IsPivot = strIsPivot;
            frm.OrderKey = strOrderKey;
            frm.ProcessID = strProcessID;
            frm.LinkQryID = strLinkQryID;
            frm.IsRepeatDown = this.IsRepeatDown;
            frm.IsUseLocal = strUseLocal;
            frm.OpenStyle = "include";

            frm.QryParam = this.QryParam;
            frm.QryValue = this.QryValue;
            //UIContent dUI = new UIContent(frm, "查询--" + strTitle, null);
            //Genersoft.Platform.AppFrameworkGui.Gui.WorkbenchSingleton.Workbench.ShowView(dUI);
        }
        /// <summary>
        /// 调用adp开发窗体
        /// </summary>
        /// <param name="formDefID"></param>
        /// <param name="dataID"></param>
        /// <param name="initialActionID"></param>
        /// <param name="argumentString"></param>
        public void invokeAdpFrm(string formDefID, string dataID, string initialActionID, string argumentString )
        {
            //XForm form = new XForm(formDefID, dataID, initialActionID, argumentString, ClientContext.Current.FramworkState);
            //form.StartPosition = FormStartPosition.CenterParent;
            //DialogResult result = form.ShowDialog();
        }


        private string _psTitle = "";
        private string _psID = "";
        private string _psYear = "";
        private string _psJEJD = "2";
        private string _psSLJD = "2";
        private string _psTitleTable = "";
        private string _psSelect = "";
        private string _psSubTitle = ""; 
        private string _psDWBH = "";
         

        private string _isPivot = "0";
        public string IsPivot
        {
            set { this._isPivot = value; }
            get
            {
                if (string.IsNullOrEmpty(_isPivot))
                    _isPivot = "0";
                return this._isPivot;
            }
        }
        public string PsDWBH
        {
            set { this._psDWBH = value; }
            get { return this._psDWBH; }
        }
        private string _psUsr = "";
        public string PsUsr
        {
            set { this._psUsr = value; }
            get { return this._psUsr; }
        }
        public string PsTitle
        {
            set { this._psTitle = value; }
            get { return this._psTitle; }
        }
        
        public string PsID
        {
            set { this._psID = value; }
            get { return this._psID; }
        }
        
        public string PsYear
        {
            set { this._psYear = value; }
            get { return this._psYear; }
        } 
        public string PsJEJD
        {
            set { this._psJEJD = value; }
            get { return this._psJEJD; }
        }
        
        public string PsSLJD
        {
            set { this._psSLJD = value; }
            get { return this._psSLJD; }
        } 
        public string PsTitleTable
        {
            set { this._psTitleTable = value; }
            get { return this._psTitleTable; }
        }
        
        public string PsSelect
        {
            set { this._psSelect = value; }
            get { return this._psSelect; }
        }
        
        public string PsSubTitle
        {
            set { this._psSubTitle = value; }
            get { return this._psSubTitle; }
        }

        private string _processID = "";
        public string ProcessID
        {
            set { this._processID = value; }
            get { return this._processID; }
        }
        private string _OrderKey = "";
        public string OrderKey
        {
            set { this._OrderKey = value; }
            get { return this._OrderKey; }
        }
        private string _LinkQryID = "";
        public string LinkQryID
        {
            set { this._LinkQryID = value; }
            get { return this._LinkQryID; }
        }
        private string _IsRepeatDown = "0";
        public string IsRepeatDown
        {
                 set { this._IsRepeatDown = value; }
            get { return this._IsRepeatDown; }
        }

        private string _IsUseLocal = "2";//默认2 无操作 0 缓存到本地文件 1 从本地文件中读取
        public string IsUseLocal
        {
            set { this._IsUseLocal = value; }
            get { return this._IsUseLocal; }
        }

        private string _IsSql = "1";//1是0否 默认sql
        public string IsSql
        {
            set { this._IsSql = value; }
            get { return this._IsSql; }
        }
        private string _ParamStr;
        public string ParamStr
        {
            set { this._ParamStr = value;
            this._ParamArr = this._ParamStr.Split('^');
            }
            get { return this._ParamStr; }
        }

        private string[] _ParamArr ;
        public string[] ParamArr
        {
            set { this._ParamArr = value; }
            get { return this._ParamArr; }
        }

        private string _ValueStr;
        public string ValueStr
        {
            set
            {
                this._ValueStr = value;
                this._ValueArr = this._ValueStr.Split('^');
            }
            get { return this._ValueStr; }
        }
        private string[] _ValueArr ;
        public string[] ValueArr
        {
            set { this._ValueArr = value; }
            get { return this._ValueArr; }
        }


        private string _QryParam;
        /// <summary>
        /// 多个参数用&拆分
        /// </summary>
        public string QryParam
        {
            set { this._QryParam = value; }
            get { return this._QryParam; }
        }
        private string _QryValue;
        /// <summary>
        /// 多个参数用&拆分，值有多个如时间等用`（esc下键）拆分
        /// </summary>
        public string QryValue
        {
            set { this._QryValue = value; }
            get { return this._QryValue; }
        }
    }
}
