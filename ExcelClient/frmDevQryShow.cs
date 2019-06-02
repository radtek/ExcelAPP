using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Skins.Info;
using DevExpress.UserSkins;

namespace ExcelClient
{
    public partial class frmDevQryShow : DevExpress.XtraEditors.XtraForm
    {
        public frmDevQryShow()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(DevQryShow.CultureInfo);
            DevExpress.UserSkins.BonusSkins.Register();
            //DevExpress.UserSkins.OfficeSkins.Register();
            //SkinManager.Default.RegisterAssembly(typeof(GSSkins).Assembly);
            //DevExpress.UserSkins.GSSkins.
            InitializeComponent();
        }

        frmDevBBShow frmBBShow = null;
     
        private void frmDevQryShow_Load(object sender, EventArgs e)
        {

            //defaultLookAndFeel1.LookAndFeel.Style = DevExpress.UserSkins.GSSkins;
            //SkinManager.Default.RegisterSkin(new SkinBlobXmlCreator("Coffee", "SkinData.", typeof(BonusSkins).Assembly, null));
            defaultLookAndFeel1.LookAndFeel.SkinName = "GS V5";
            //defaultLookAndFeel1.LookAndFeel.SkinName = "Office 2007 Silver";
            //defaultLookAndFeel1.LookAndFeel.SkinName = "The Asphalt World";
            //defaultLookAndFeel1.LookAndFeel.SkinName = "Office 2010 Silver"; 
           
             
            frmBBShow= new frmDevBBShow();
            frmBBShow.Text = "查询--" + this.PsTitle; 
            frmBBShow.ProcessID = this.ProcessID;
            frmBBShow.PsTitle = PsTitle;
            frmBBShow.PsID = PsID;
            frmBBShow.PsYear = PsYear;
            frmBBShow.PsJEJD = PsJEJD;
            frmBBShow.PsSLJD = PsSLJD;
            frmBBShow.PsTitleTable = PsTitleTable;
            frmBBShow.PsSelect = PsSelect;
            frmBBShow.PsSubTitle = PsSubTitle; 
            frmBBShow.PsDWBH = PsDWBH;
            frmBBShow.PsUsr = PsUsr; 
            frmBBShow.IsPivot = IsPivot;
            frmBBShow.OrderKey = this.OrderKey;
            frmBBShow.LinkQryID = this.LinkQryID;
            frmBBShow.IsRepeatDown = this.IsRepeatDown;
            frmBBShow.IsUseLocal = this.IsUseLocal;

            frmBBShow.TopLevel = false;
            frmBBShow.Location = new Point(0, 0);
            frmBBShow.TopMost = false;
            frmBBShow.ControlBox = false;            
            frmBBShow.Parent = this.tabpagegrid;
            frmBBShow.OpenStyle = this.OpenStyle;
            frmBBShow.Dock = DockStyle.Fill;
            frmBBShow.MainForm = this;
            frmBBShow.IsSql = this.IsSql;
            frmBBShow.ParamArr = this.ParamArr;
            frmBBShow.ValueArr = this.ValueArr;

            frmBBShow.QryParam = this.QryParam;
            frmBBShow.QryValue = this.QryValue;
            frmBBShow.Show();
        }
 

        #region 查询变量
        /*
       QueryResult.asp?psTitle=物业领用查询列表&psID=YSLYQuery&psPID=24395EB68809480389E447D1082343C2
       &psYear=2008&psSelect=SELECT * FROM YSLYQuery8509&psSubTitle=@YJDW@=,@LYDW@=&psJEJD=2&psSLJD=2
       &psLinkCol=F_ID&psLinkPathArg=../WYGL/WYGL_WEB/WYGL_QUERYWYYJBPG.aspx?psID=@F_ID@
         psTitle 
psID
psYear
psJEJD
psSLJD
psTitleTable 动态表头表
psSelect
psSubTitle    @ZXMC@="+vsZXMC+",@DATENAME@="+vsDateName+",@DATEVALUE@="+vsCurrDate
psPID
连查时使用psLinkCol psLinkPathArg*/

        private string _psTitle = "";
        private string _psID = "";
        private string _psYear = "";
        private string _psJEJD = "2";
        private string _psSLJD = "2";
        private string _psTitleTable = "";
        private string _psSelect = "";
        private string _psSubTitle = "";
        private string _psLinkCol = "";
        private string _psLinkPathArg = "";
        private string _psBBver = "";
        private string _psDWBH = "";
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
        private Form _MainForm;
        public Form MainForm
        {
            set { this._MainForm = value; }
            get { return this._MainForm; }
        }
        public string PsTitle
        {
            set { this._psTitle = value;
                this.Text = "查询--" + this.PsTitle;
            }
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
            get { return this._psYear.Trim(); }
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
            get { return this._psTitleTable.Trim(); }
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
         
        public string PsBBver
        {
            set { this._psBBver = value; }
            get { return this._psBBver; }
        }
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
        private string _OpenStyle = "show";
        public string OpenStyle
        {
            set { this._OpenStyle = value; }
            get { return this._OpenStyle; }
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
            set
            {
                this._ParamStr = value;
                this._ParamArr = this._ParamStr.Split('^');
            }
            get { return this._ParamStr; }
        }
        private string[] _ParamArr;
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
        private string[] _ValueArr;
        public string[] ValueArr
        {
            set { this._ValueArr = value; }
            get { return this._ValueArr; }
        }


        private string _QryParam=null;
        /// <summary>
        /// 多个参数用&拆分
        /// </summary>
        public string QryParam
        {
            set { this._QryParam = value; }
            get { return this._QryParam; }
        }
        private string _QryValue=null;
        /// <summary>
        /// 多个参数用&拆分，值有多个如时间等用`（esc下键）拆分
        /// </summary>
        public string QryValue
        {
            set { this._QryValue = value; }
            get { return this._QryValue; }
        }
        #endregion 查询变量

        private void frmDevQryShow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(frmBBShow!=null)
                frmBBShow.Close();
        }

      

        private void tabctrl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
           // e.Page.Controls[0].Focus();
            SetTabFocus(e.Page);
        }

        private void SetTabFocus(DevExpress.XtraTab.XtraTabPage pg)
        {
            if (pg.Name == "tabpagegrid")
            {
                frmDevBBShow frmbb = (pg.Controls[0] as frmDevBBShow);
                frmbb.Activate();
                frmbb.Focus();
            }
            else
            {
                frmDevPivotShow frmpivot = (pg.Controls[0] as frmDevPivotShow);
                frmpivot.Activate();
                frmpivot.Focus();
            }
        }

    

       
    }
}
