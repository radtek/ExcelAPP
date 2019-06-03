using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Configuration;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Columns;
using System.Text.RegularExpressions;
using DevExpress.LookAndFeel;

namespace ExcelClient
{
    public partial class frmDevBBShow : DevExpress.XtraEditors.XtraForm
    {
        public frmDevBBShow()
        {
            InitializeComponent();
        }
        #region  变量定义
        WsGetDataClient.WSGetData mgr = new WsGetDataClient.WSGetData();
        private DevBBTitleFont titleFont = new DevBBTitleFont();
        private DataTable dtQryStyle = new DataTable();
        //private int maxJS = 0;

        private DevExpress.XtraGrid.Views.BandedGrid.GridBand titleBand = null;//band  
        DevExpress.XtraGrid.Views.Grid.GridView bandedView;//主view
        public DataSet dsData;//数据集 
        public DataTable dtLSZBGS;
        public DataTable dtLSOTGS;
        public DataTable dtLSTIGS;
        private DataTable dtLSZBGSOrg;
        private DataTable dtLSOTGSOrg;
        private DataTable dtLSTIGSOrg;
        private DataTable dtLSTIGSDef;
        private DataTable dtGridRow = new DataTable();
        private DataTable dtDevQry = null;
        private DateTime startDt;
        private bool blLoadData = false;
        private bool blLoadPivot = false;
        private Dictionary<int, GridColumn> dicGroupColumns = new Dictionary<int, GridColumn>();
        private int gridGroupIndex = 0;
        private int selectedFormatIndex = 0;
        frmDevPivotShow frmPivot = null;
        private DataTable dtLinkQry = null;
        private string NumFormat = "N";
        private bool NumRedShow = true;
        private bool ShowZeroValue = true;

        public WsGetDataClient.WSGetData WSMgr
        {
            set { this.mgr = value; }
        }
        #endregion


        private void frmDevBBShow_Load(object sender, EventArgs e)
        {
            barTabInfoSet.Visibility = DevExpress.XtraBars.BarItemVisibility.Never; //隐藏账表设置按钮，设置格式有问题
            //gridControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            //gridControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.Text = "";
            dockBBSet.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            // mgr.Url = "http://localhost/JTGLPubQry2010/JTGLPubQry_Svr/WSGetData.asmx";     



            UserLookAndFeel.Default.SetSkinStyle("Office 2013");
            DevQryPubFun.WrapService(mgr);

        }

        private void frmDevBBShow_Shown(object sender, EventArgs e)
        {
            try
            {

                //this.repCheckEdt.

                startDt = System.DateTime.Now;
                getProcData();
                setTitleFont();
                getDataTitle();
                bindLinkQryCtrl();

                this.Activate();
                this.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 绑定表头

        private void bindLinkQryCtrl()
        {
            getLinkQryTable();
            if (dtLinkQry.Rows.Count > 0)
            {
                if (dtLinkQry.Rows.Count == 1)
                {
                    barLinkQry.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(barLinkQry_ItemClick);
                }
                else
                {
                    for (int i = 0; i < dtLinkQry.Rows.Count; i++)
                    {
                        DataRow row = dtLinkQry.Rows[i];
                        DevExpress.XtraBars.BarButtonItem barItem = barManager1.Items.CreateButton(row["JTPUBLINKQRY_NAME"].ToString());
                        barLinkQry.ItemLinks.Add(barItem);
                        barItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(barLinkQry_ItemClick);
                        barItem.Name = "BARLINKITEM" + i;

                    }
                }
            }
            else
                barLinkQry.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;//barLinkQry_ItemClick
        }

        private void getLinkQryTable()
        {
            //读取本地缓存
            if (this.IsUseLocal == "1")
            {
                DataSet dstmp = new DataSet();
                bool bl = getLocalData("LK", ref   dstmp);
                if (bl)
                    dtLinkQry = dstmp.Tables[0];
                else
                    dtLinkQry = new DataTable();
            }
            else
            {
                string vsLinkQryID = this.PsID;
                string vsdelSql = "";
                if (!string.IsNullOrEmpty(this.LinkQryID) && this.LinkQryID.ToLower() != this.PsID.ToLower())
                {
                    vsLinkQryID = this.LinkQryID;

                    vsdelSql = "delete FROM JTPUBLINKQRY where JTPUBLINKQRY_QRYID='" + this.LinkQryID + "'";
                }
                string sql = "SELECT * FROM JTPUBLINKQRY where JTPUBLINKQRY_QRYID='" + vsLinkQryID + "'";
                dtLinkQry = getDataTable(sql);
                if (!string.IsNullOrEmpty(vsdelSql))
                    WebSvrGetData.execsql(DevQryPubFun.GSYDBSrc, vsdelSql, mgr);
                 
            }
        }
        private void bindFirstGrid()
        {

            getSvrData();
            DateTime dtLoadEnd = System.DateTime.Now;
            TimeSpan tsLoad = dtLoadEnd - startDt;
            barInfoCount.Caption = "数据行数：" + dsData.Tables[0].Rows.Count;
            barInfoDown.Caption = "数据下载解压耗时：" + tsLoad.Minutes + "分" + tsLoad.Seconds + "秒" + tsLoad.Milliseconds + "毫秒";
            bindGrid();
            //deleteTempTable();
            tsLoad = System.DateTime.Now - dtLoadEnd;
            barInfoShow.Caption = "数据展现耗时：" + tsLoad.Minutes + "分" + tsLoad.Seconds + "秒" + tsLoad.Milliseconds + "毫秒";

            if (!blLoadPivot)
                LoadPivotFrm();
        }
        private void getProcData()
        {
            if (!blLoadData)
            {
                //读取本地缓存
                if (this.IsUseLocal == "1")
                {
                    bool bl = getLocalData("DB", ref dsData);
                    if (!bl)
                    {
                        throw new Exception("没有缓存数据！");
                    }
                    else
                    {
                        blLoadData = true;
                    }
                }
                else
                {
                    if (this.IsSql != "1")
                    {
                        dsData = WebSvrGetData.getDataSet(ProcessID, PsSelect, ParamArr, ValueArr, mgr);
                        blLoadData = true;
                    }

                }
            }
        }
        private void getSvrData()
        {

            if (!blLoadData)
            {
                //读取本地缓存
                if (this.IsUseLocal == "1")
                {
                    bool bl = getLocalData("DB", ref dsData);
                    if (!bl) throw new Exception("没有缓存数据！");
                }
                else
                {
                    string fieldStr = ComputeHash.getFieldNamesFromDataTable(dtLSTIGS, "F_FIELD");
                    if (this.IsSql == "1")
                        dsData = WebSvrGetData.GetDataSet(ProcessID, this.PsSelect.Replace("@ZW@.", ""), this.OrderKey, mgr, fieldStr, IsRepeatDown);
                    else
                        dsData = WebSvrGetData.getDataSet(ProcessID, PsSelect, ParamArr, ValueArr, mgr);

                }
                blLoadData = true;
            }
        }
        private void deleteTempTable()
        {
            string sqldelete = "";
            string vstable = "";
            int posFrom = PsSelect.ToLower().IndexOf("from ");
            if (posFrom != -1)
            {
                vstable = PsSelect.Substring(posFrom + 5) + " ";
                vstable = vstable.Substring(0, vstable.IndexOf(" "));
                //不是多个表连查，
                string[] vstabarr = vstable.Split(',');
                foreach (string str in vstabarr)
                {
                    vstable = str.Trim();
                    if (vstable.ToLower().IndexOf("tmp") == 0)
                    {
                        sqldelete = "drop table " + vstable;
                        WebSvrGetData.execsql(DevQryPubFun.GSYDBSrc, sqldelete, mgr);                         
                    }
                }
            }             
        }
        private void bindGrid()
        {
            if (bandedView != null)
            {
                if (bandedView is BandedGridView)
                {
                    (bandedView as BandedGridView).Bands.Clear();
                }
                bandedView.Columns.Clear();
                gridControl1.DataSource = null;
            }
            bindTitle();
            if (dsData.Tables.Count > 0)
            {
                //处理格式化
                dealDataFmt();
                //
                gridControl1.UseEmbeddedNavigator = true;
           //   NavigatableControlHelper nav=new NavigatableControlHelper()
                gridControl1.BeginUpdate();
                gridControl1.DataSource = dsData.Tables[0].DefaultView;
                bandedView.IndicatorWidth = dsData.Tables[0].Rows.Count.ToString().Length * 10 + 15;
                gridControl1.EndUpdate();
                foreach (int i in dicGroupColumns.Keys)
                {
                    GridColumn column = dicGroupColumns[i];
                    column.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.True;
                    column.GroupIndex = i;
                }
            }
        }


        private void dealDataFmt()
        {
            DataRow[] rows = dtLSTIGS.Select("F_FORMAT='BASE64'");
            if (rows != null && rows.Length > 0)
            {
                foreach (DataRow row in rows)
                {
                    foreach (DataRow drow in dsData.Tables[0].Rows)
                    {
                        drow[row["F_FIELD"].ToString()] = BASE64.DeCode(drow[row["F_FIELD"].ToString()].ToString());
                    }
                }
            }
        }
        private void getDataTitle()
        {
            DataSet vsds = new DataSet();

            string sql = "";
            //下载数据库
            string vsTitleTable = "EATIGS" + PsYear;
            if (PsTitleTable.Trim() != "")
            {
                vsTitleTable = PsTitleTable;
                //自定义字段样式
                sql = string.Format("select * from  EATIGSDEVQRY where F_ID='{0}' and (f_dwbh='' or f_dwbh=' ' or f_dwbh='{1}')  ", this.PsID, this.PsDWBH);
                dtDevQry = getDataTable(sql);
                if (dtDevQry.Rows.Count == 0)
                {
                    sql = string.Format("select * from  EATIGSDEVQRY where F_ID='{0}' and (f_dwbh='' or f_dwbh=' ' ) ", this.PsID);
                    dtDevQry = getDataTable(sql);
                }
                if (dtDevQry.Rows.Count == 0)
                {
                    setDefaultDtDevQry(" ", "01");
                    BBGsHelp.insertDevRowGS(DevQryPubFun.GSYDBSrc, mgr, dtDevQry, " ", PsYear, "01");
                }
                //读取本地缓存
                if (this.IsUseLocal == "1")
                {
                    bool bl = getLocalData("GS", ref vsds);
                    if (!bl) throw new Exception("没有缓存数据！");
                    else
                        dtLSTIGSOrg = vsds.Tables[0];
                }
                else
                {
                    //所有字段样式
                    sql = string.Format("select * from {1} where f_id='{0}' and (f_dwbh='' or f_dwbh=' ' or f_dwbh='{2}')", this.PsID, vsTitleTable, this.PsDWBH);
                    dtLSTIGSOrg = getDataTable(sql);
                    if (dtLSTIGSOrg.Rows.Count == 0)
                    {
                        sql = string.Format("select * from {1} where f_id='{0}' and (f_dwbh='' or f_dwbh=' ' ) ", this.PsID, vsTitleTable);
                        dtLSTIGSOrg = getDataTable(sql);
                    }
                }
                //默认保存的样式
                sql = string.Format("select * from EATIGS{1} where f_id='{0}' and (f_dwbh='' or f_dwbh=' ' or f_dwbh='{2}') ", this.PsID, PsYear, this.PsDWBH);
                dtLSTIGSDef = getDataTable(sql);
                if (dtLSTIGSDef.Rows.Count == 0)
                {
                    sql = string.Format("select * from EATIGS{1} where f_id='{0}' and (f_dwbh='' or f_dwbh=' ' )  ", this.PsID, PsYear);
                    dtLSTIGSDef = getDataTable(sql);
                }
            }
            else
            {
                //读取本地缓存
                if (this.IsUseLocal == "1")
                {
                    bool bl = getLocalData("GS", ref vsds);
                    if (!bl) throw new Exception("没有缓存数据！");
                    else
                        dtLSTIGSOrg = vsds.Tables[0];
                }
                else
                {
                    sql = string.Format("select * from {1} where f_id='{0}' and (f_dwbh='' or f_dwbh=' ' or f_dwbh='{2}')", this.PsID, vsTitleTable, this.PsDWBH);
                    dtLSTIGSOrg = getDataTable(sql);
                    if (dtLSTIGSOrg.Rows.Count == 0)
                    {
                        sql = string.Format("select * from {1} where f_id='{0}' and (f_dwbh='' or f_dwbh=' ' ) ", this.PsID, vsTitleTable);
                        dtLSTIGSOrg = getDataTable(sql);
                    }
                }
            }
            sql = string.Format("select F_DWBH,F_ID,F_GSBH,F_GSMC,F_CAPT,F_CAPH,F_SCAPH,F_BTGD,F_ROWH,F_BWGD,F_SPACE,F_SYBZ from EAZBGS{1} where f_id='{0}' and (f_dwbh='' or f_dwbh=' ' or f_dwbh='{2}') ", this.PsID, this.PsYear, this.PsDWBH);
            dtLSZBGSOrg = getDataTable(sql);
            if (dtLSZBGSOrg.Rows.Count == 0)
            {
                sql = string.Format("select F_DWBH,F_ID,F_GSBH,F_GSMC,F_CAPT,F_CAPH,F_SCAPH,F_BTGD,F_ROWH,F_BWGD,F_SPACE,F_SYBZ from EAZBGS{1} where f_id='{0}' and (f_dwbh='' or f_dwbh=' ' )", this.PsID, this.PsYear);
                dtLSZBGSOrg = getDataTable(sql);
            }

            sql = string.Format("select * from EAOTGS{1} where f_id='{0}' and (f_dwbh='' or f_dwbh=' ' or f_dwbh='{2}') ", this.PsID, this.PsYear, this.PsDWBH);
            dtLSOTGSOrg = getDataTable(sql);
            if (dtLSOTGSOrg.Rows.Count == 0)
            {
                sql = string.Format("select * from EAOTGS{1} where f_id='{0}' and (f_dwbh='' or f_dwbh=' ' ) ", this.PsID, this.PsYear);
                dtLSOTGSOrg = getDataTable(sql);
            }
            bindQryStyle();
        }
        private void bindQryStyle()
        {
            cmbColFormat.Items.Clear();
            cmbColSaved.Properties.Items.Clear();
            dtQryStyle = dtLSZBGSOrg.DefaultView.ToTable(true, new string[] { "F_GSBH", "F_GSMC" });

            foreach (DataRow row in dtQryStyle.Rows)
            {
                cmbColFormat.Items.Add(row["F_GSMC"]);
                cmbColSaved.Properties.Items.Add(row["F_GSMC"]);
            }
            if (cmbColFormat.Items.Count > 0)
            {
                barEditFormat.EditValue = cmbColFormat.Items[selectedFormatIndex].ToString();
            }
        }

        private DataTable getDataTable(string sql)
        {
            DataTable dt = WebSvrGetData.getDataTable(ProcessID, mgr, sql);
         
            return dt;
        }

        private void bindTitle()
        {

            bandedView = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();
            bandedView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;

            bandedView.Appearance.HeaderPanel.BackColor = System.Drawing.Color.White;
            bandedView.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.White;
            bandedView.Appearance.EvenRow.BackColor = System.Drawing.Color.White;
            bandedView.Appearance.EvenRow.Options.UseBackColor = true;
            bandedView.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            bandedView.Appearance.OddRow.Options.UseBackColor = true;
            // bandedView = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();

            bandedView.OptionsView.ShowAutoFilterRow = true;
            bandedView.CustomDrawCell -= new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(gridView1_CustomDrawCell);
            bandedView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(gridView1_CustomDrawCell);
            bandedView.CustomDrawRowIndicator -= new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridview_CustomDrawRowIndicator);
            bandedView.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridview_CustomDrawRowIndicator);
            bandedView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
            bandedView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ShowFooter = true;
            //bandedView.OptionsView.ShowGroupedColumns = true;
            bandedView.OptionsView.EnableAppearanceEvenRow = true;
            bandedView.OptionsView.EnableAppearanceOddRow = true;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            bandedView.OptionsBehavior.Editable = false;
            bandedView.OptionsSelection.EnableAppearanceFocusedCell = false;

            (bandedView as BandedGridView).BandPanelRowHeight = Convert.ToInt32(dtLSZBGS.Rows[0]["F_SCAPH"] == DBNull.Value ? "0" : dtLSZBGS.Rows[0]["F_SCAPH"]);


            // bandedView.ColumnPanelRowHeight = Convert.ToInt32(dtLSZBGS.Rows[0]["F_BTGD"]);
            bandedView.ColumnPanelRowHeight = 0;
            bandedView.RowHeight = Convert.ToInt32(dtLSZBGS.Rows[0]["F_ROWH"] == DBNull.Value ? "0" : dtLSZBGS.Rows[0]["F_ROWH"]);
            gridControl1.MainView = bandedView;
            bandedView.GridControl = gridControl1;
            bandedView.Name = "bandedView";

            // maxJS = Convert.ToInt32(dtLSTIGS.Compute("max(F_JS)", "true"));

            DevExpress.XtraGrid.Views.BandedGrid.GridBand parenttitle = null;
            DevExpress.XtraGrid.Views.BandedGrid.GridBand title = null;
            for (int i = 0; i < dtLSZBGS.Rows.Count; i++)
            {
                title = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
                (bandedView as BandedGridView).Bands.Add(title);
                parenttitle = title;
                setGridBand(title, dtLSZBGS.Rows[i]["F_CAPT"].ToString(), 0, titleFont.titleFont, "M");
            }
            DevExpress.XtraGrid.Views.BandedGrid.GridBand subtitle = null;
            for (int i = 0; i < dtLSOTGS.Rows.Count; i++)
            {
                subtitle = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
                if (i == 0)
                {
                    if (title != null)
                    {
                        title.Children.Add(subtitle);
                    }
                    else
                    {
                        (bandedView as BandedGridView).Bands.Add(subtitle);
                    }
                    parenttitle = subtitle;
                }
                else
                {
                    parenttitle.Children.Add(subtitle);
                    parenttitle = subtitle;
                }
                setGridBand(subtitle, dtLSOTGS.Rows[i]["F_TEXT"].ToString(), 0, titleFont.subTitleFont, dtLSOTGS.Rows[i]["F_ALIGN"].ToString());
            }
            DevExpress.XtraGrid.Views.BandedGrid.GridBand pBand = null;
            bandedView.BeginUpdate();
            if (subtitle != null)
                pBand = subtitle;
            else
                pBand = title;
            titleBand = pBand;
            bindFJTitle(dtLSTIGS, 1, "", pBand);
            if (gridGroupIndex != 0)
            {
                bandedView.OptionsView.ShowGroupPanel = true;
                bandedView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;

                gridGroupIndex = 0;//重置groupindex      
            }

            bandedView.EndUpdate();
        }


        private void bindFJTitle(DataTable dt, int intjs, string BH, DevExpress.XtraGrid.Views.BandedGrid.GridBand pBand)
        {
            int vsjs = intjs;
            DataRow[] rows = dt.Select(string.Format("F_JS={0} and F_TIBH like '{1}%'", intjs + "", BH), "F_TIBH");
            if (rows.Length != 0)
            {//绑定多级表头 F_FIELD='' 
                int vsgr = vsjs + 1;//级数加1
                for (int i = 0; i < rows.Length; i++)
                {
                    GridColumn column = null;

                    if (rows[i]["F_FIELD"].ToString().Trim() == "")
                    {
                        DevExpress.XtraGrid.Views.BandedGrid.GridBand vsband = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
                        pBand.Children.Add(vsband);
                        if (rows[i].Table.Columns.Contains("F_ISGD") && rows[i]["F_ISGD"].ToString() == "1")
                            pBand.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;

                        setGridBand(vsband, rows[i]["F_TEXT"].ToString(), 0, titleFont.subTitleFont, "M");
                        string vsBH = rows[i]["F_TIBH"].ToString();
                        if (rows[i]["F_YHBZ"].ToString().Trim() == "1")
                            vsband.Visible = false;
                        bindFJTitle(dt, vsgr, vsBH, vsband);
                    }
                    else
                    {

                        column = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                        setColumnBand(column, rows[i], titleFont.columnFont, titleFont.rowFont);

                        DevExpress.XtraGrid.Views.BandedGrid.GridBand vsband = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
                        pBand.Children.Add(vsband);
                        if (rows[i].Table.Columns.Contains("F_ISGD") && rows[i]["F_ISGD"].ToString() == "1")
                            pBand.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                        setGridBand(vsband, rows[i]["F_TEXT"].ToString(), 0, titleFont.columnFont, "M");
                        vsband.Columns.Add(column as BandedGridColumn);
                        if (rows[i]["F_YHBZ"].ToString().Trim() == "1")
                            vsband.Visible = false;

                    }

                }
            }
        }

        private void setGridBand(DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand, string Caption, int width, PrintFont psFont, string psAlign)
        {
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.Caption = replaceColumnTitle(Caption);

            // gridBand.View.BandPanelRowHeight = 30;//调整表头高度
            //gridBand.Name = "gridBands" + index;
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Font = new Font(psFont.Name, psFont.Size, psFont.Style);
            if (width != 0)
            {
                gridBand.Width = width;

            }
            switch (psAlign)
            {
                case "L":
                    gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                    break;
                case "M":
                    gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    break;
                case "R":
                    gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    break;
            }
        }


        private void setColumnBand(DevExpress.XtraGrid.Columns.GridColumn column, DataRow row, PrintFont colFont, PrintFont rowFont)
        {

            column.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            column.AppearanceHeader.Options.UseTextOptions = true;
            column.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            DevExpress.XtraGrid.GridGroupSummaryItem sumItem = new DevExpress.XtraGrid.GridGroupSummaryItem();

            column.FieldName = row["F_FIELD"].ToString();
            //column.OptionsFilter.AllowFilter = true;
            //column.OptionsFilter.AllowAutoFilter = true;
            column.Caption = "　";
            column.Visible = true;
            if (row["F_YHBZ"].ToString() == "1")
                column.Visible = false;
            else
                column.Width = Convert.ToInt32(row["F_WIDTH"]);
            if (row["F_HJBZ"].ToString() != "0")
            {

                if (row["F_HJBZ"].ToString() == "1")
                {
                    column.SummaryItem.FieldName = row["F_FIELD"].ToString();
                    column.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    sumItem.FieldName = row["F_FIELD"].ToString();
                    sumItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                }
                else if (row["F_HJBZ"].ToString() == "2")
                {
                    column.SummaryItem.FieldName = row["F_FIELD"].ToString();
                    column.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Average;
                    sumItem.FieldName = row["F_FIELD"].ToString();
                    sumItem.SummaryType = DevExpress.Data.SummaryItemType.Average;
                }
                else if (row["F_HJBZ"].ToString() == "3")
                {
                    column.SummaryItem.FieldName = row["F_FIELD"].ToString();
                    column.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Max;
                    sumItem.FieldName = row["F_FIELD"].ToString();
                    sumItem.SummaryType = DevExpress.Data.SummaryItemType.Max;
                }
                else if (row["F_HJBZ"].ToString() == "4")
                {
                    column.SummaryItem.FieldName = row["F_FIELD"].ToString();
                    column.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Min;
                    sumItem.FieldName = row["F_FIELD"].ToString();
                    sumItem.SummaryType = DevExpress.Data.SummaryItemType.Min;
                }
                sumItem.Tag = 1;
                sumItem.ShowInGroupColumnFooter = column;
                bandedView.GroupSummary.Add(sumItem.SummaryType, sumItem.FieldName, column);

            }

            if (row.Table.Columns.Contains("F_GROUP") && row["F_GROUP"].ToString() == "1")
            {
                if (!dicGroupColumns.ContainsKey(gridGroupIndex))
                {
                    dicGroupColumns.Add(gridGroupIndex, column);
                }
                gridGroupIndex = gridGroupIndex + 1;
            }
            if (row.Table.Columns.Contains("F_ISGD") && row["F_ISGD"].ToString() == "1")
            {
                column.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            }

            column.AppearanceHeader.Options.UseFont = true;
            column.AppearanceHeader.Font = new Font(colFont.Name, colFont.Size, colFont.Style);
            column.AppearanceCell.Options.UseFont = true;
            column.AppearanceCell.Font = new Font(rowFont.Name, rowFont.Size, rowFont.Style);

            if (row["F_TYPE"].ToString() == "C")
            {
                column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
                column.AppearanceCell.Options.UseTextOptions = true;
                column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            }
            else
                if (row["F_TYPE"].ToString() == "J")
                {
                    column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    column.AppearanceCell.Options.UseTextOptions = true;
                    column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    if (row["F_PREC"].ToString() == "S")
                    {
                        column.DisplayFormat.FormatString = "{0:" + NumFormat + PsSLJD + "}";
                    }
                    else if (row["F_PREC"].ToString() == "J")
                    {
                        column.DisplayFormat.FormatString = "{0:" + NumFormat + PsJEJD + "}";
                    }
                    else
                    {
                        int intJD = 2;
                        if (int.TryParse(row["F_PREC"].ToString(), out intJD))
                        {
                            column.DisplayFormat.FormatString = "{0:" + NumFormat + intJD + "}";
                        }
                        else
                        {
                            column.DisplayFormat.FormatString = "{0:" + NumFormat + "2}";
                        }
                    }
                    column.SummaryItem.DisplayFormat = column.DisplayFormat.FormatString;
                }
                else
                    if (row["F_TYPE"].ToString() == "N")
                    {
                        column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        column.AppearanceCell.Options.UseTextOptions = true;
                        column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                        column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        if (row["F_PREC"].ToString() == "S")
                        {
                            column.DisplayFormat.FormatString = "{0:" + NumFormat + PsSLJD + "}";
                        }
                        else if (row["F_PREC"].ToString() == "J")
                        {
                            column.DisplayFormat.FormatString = "{0:" + NumFormat + PsJEJD + "}";
                        }
                        else
                        {
                            int intJD = 2;
                            if (int.TryParse(row["F_PREC"].ToString(), out intJD))
                            {
                                column.DisplayFormat.FormatString = "{0:" + NumFormat + intJD + "}";
                            }
                            else
                            {
                                column.DisplayFormat.FormatString = "{0:" + NumFormat + "2}";
                            }
                        }
                        sumItem.DisplayFormat = column.SummaryItem.DisplayFormat = column.DisplayFormat.FormatString;
                    }

            switch (row["F_ALIGN"].ToString())
            {
                case "L":
                    column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                    break;
                case "M":
                    column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    break;
                case "R":
                    column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    break;
            }
        }

        private string replaceColumnTitle(string str)
        {
            string[] paramarr = this.PsSubTitle.Split(',');
            foreach (string strps in paramarr)
            {
                string[] psarr = strps.Split('=');
                if (psarr.Length == 2)
                    str = str.Replace(psarr[0], psarr[1]);

            }
            return str;
        }
        #endregion 绑定表头


        #region 菜单事件
        private void barTabInfoSet_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dockBBSet.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
        }



        private void barGraAnal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDevChartShow frm = new frmDevChartShow();
            frm.Title = this.PsTitle;
            frm.DtLSTIGS = dtLSTIGS;
            frm.DsData = dsData;
            frm.PsID = this.PsID;
            frm.Show();
            frm.Focus();
        }
        private void barPivotAnal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.MainForm.tabctrl.SelectedTabPageIndex = 1;
            #region 屏蔽原来show方式代码
            /*
            frmDevPivotShow frm = new frmDevPivotShow();
            frm.Title = this.PsTitle;
            frm.DtLSTIGS = dtLSTIGS;
            frm.DsData = dsData;
            frm.PsID = this.PsID;
            frm.mgr = this.mgr;
            frm.PsUsr = this.PsUsr;
            frm.Show();
             */
            #endregion

        }

        private void barBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string filename = "";
            DevPrint prt;
            switch (e.Item.Name)
            {
                case "barview":
                    prt = new DevPrint();
                    bandedView.OptionsPrint.AutoWidth = false;
                    bandedView.OptionsPrint.PrintHeader = false;
                    prt.PrintID = this.PsID;
                    prt.PrtCtrl = gridControl1;
                    prt.Preview();
                    break;
                case "barPrint":
                    prt = new DevPrint();
                    bandedView.OptionsPrint.AutoWidth = false;
                    bandedView.OptionsPrint.PrintHeader = false;
                    prt.PrintID = this.PsID;
                    prt.PrtCtrl = gridControl1;
                    prt.Print();
                    break;
                case "barSaveAsXls":
                    bandedView.OptionsView.ShowColumnHeaders = false;
                    filename = OpenSaveFileDlg(".xls", "Excel(*.xls)|*.xls");
                    if (filename == "") return;
                    gridControl1.ExportToXls(filename);
                    bandedView.OptionsView.ShowColumnHeaders = true;
                    break;
                case "barSaveAsPdf":
                    bandedView.OptionsView.ShowColumnHeaders = false;
                    filename = OpenSaveFileDlg(".pdf", "Adobe Reader(*.pdf)|*.pdf");
                    if (filename == "") return;



                    gridControl1.ExportToPdf(filename);

                    bandedView.OptionsView.ShowColumnHeaders = true;
                    break;
                case "barSaveAsTxt":
                    bandedView.OptionsView.ShowColumnHeaders = false;
                    filename = OpenSaveFileDlg(".txt", "txt文本(*.txt)|*.txt");
                    if (filename == "") return;
                    gridControl1.ExportToText(filename);
                    bandedView.OptionsView.ShowColumnHeaders = true;
                    break;
                case "barSaveAsHtml":
                    bandedView.OptionsView.ShowColumnHeaders = false;
                    filename = OpenSaveFileDlg(".html", "网页文件(*.html)|*.html");
                    if (filename == "") return;
                    gridControl1.ExportToHtml(filename);
                    bandedView.OptionsView.ShowColumnHeaders = true;
                    break;
            }
        }



        private string OpenSaveFileDlg(string fileext, string fileFilter)
        {
            saveFileDlg.DefaultExt = fileext;
            saveFileDlg.Filter = fileFilter;
            string vsRe = "";
            if (saveFileDlg.ShowDialog() == DialogResult.OK)
                vsRe = saveFileDlg.FileName;
            return vsRe;
        }
        private void barLinkQry_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dtLinkQry.Rows.Count > 0)
            {
                DataRow rowLinkQry = null;
                if (dtLinkQry.Rows.Count == 1)
                {
                    rowLinkQry = dtLinkQry.Rows[0];
                }
                else
                {
                    int itemIndex = Convert.ToInt32(e.Item.Name.Replace("BARLINKITEM", ""));
                    rowLinkQry = dtLinkQry.Rows[itemIndex];
                }
                LinkQryShow(rowLinkQry);
            }

        }
        private void LinkQryShow(DataRow row)
        {
            int[] selrows = bandedView.GetSelectedRows();
            if (selrows.Length == 0) throw new Exception("请选择记录！");
            DataRowView rowData = (bandedView.GetRow(selrows[0]) as DataRowView);
            //JTPUBLINKQRY_TYPE,JTPUBLINKQRY_URL_PATH,JTPUBLINKQRY_CLASS,
            //JTPUBLINKQRY_PARAMS,JTPUBLINKQRY_REPLACEVALUE
            if (row["JTPUBLINKQRY_TYPE"].ToString().ToLower().Trim() == "web")
            {//web窗体
                string vsLinkUrl = row["JTPUBLINKQRY_URL_PATH"].ToString();
                string[] linkValueArr = row["JTPUBLINKQRY_REPLACEVALUE"].ToString().Split(',');

                foreach (string str in linkValueArr)
                {
                    string rpval = getReplaceValue(str, rowData);
                    if (rpval.IndexOf("`") != -1)
                    {
                        string[] rpvArr = rpval.Split('`');
                        vsLinkUrl = vsLinkUrl.Replace("@" + str + "_1" + "@", rpvArr[0]);
                        vsLinkUrl = vsLinkUrl.Replace("@" + str + "_2" + "@", rpvArr[1]);
                    }
                    else
                    {
                        vsLinkUrl = vsLinkUrl.Replace("@" + str + "@", rpval);
                    }
                }
                if (vsLinkUrl.IndexOf("../") != -1)
                    vsLinkUrl = vsLinkUrl.Replace("../", "/cwbase/jtgl/");
                //frmView frm = new frmView();
                //frm.url = vsLinkUrl;
                //frm.frmName = "联查--" + row["JTPUBLINKQRY_NAME"].ToString();
                //if (this.OpenStyle == "show")
                //{
                //    frm.Show();
                //}
                //else
                //{
                //    //frm.Show();
                //    //UIContent dUI = new UIContent(frm, "联查--" + row["JTPUBLINKQRY_NAME"].ToString(), null);
                //    //Genersoft.Platform.AppFrameworkGui.Gui.WorkbenchSingleton.Workbench.ShowView(dUI);
                //    vsLinkUrl = frm.CWWebHost + vsLinkUrl;

                //    Uri vsuri = new Uri(vsLinkUrl);
                //    Genersoft.Platform.AppFrameworkGui.BrowserDisplayBinding.BrowserPane br = new Genersoft.Platform.AppFrameworkGui.BrowserDisplayBinding.BrowserPane(vsuri); //url:要显示的页面地址
                //    br.ID = "" + row["JTPUBLINKQRY_QRYID"] + row["JTPUBLINKQRY_CODE"];                                                            //打开功能的唯一标识
                //    br.TitleName = "联查--" + row["JTPUBLINKQRY_NAME"].ToString();                                         //tab页显示的名称
                //    Genersoft.Platform.AppFrameworkGui.Gui.WorkbenchSingleton.Workbench.ShowView(br);
                //    //WorkbenchSingleton.Workbench.ShowView(br);
                //}
            }
            else if (row["JTPUBLINKQRY_TYPE"].ToString().ToLower().Trim() == "adp")
            {
                string vsPath = Application.StartupPath.TrimEnd('\\') + "\\" + row["JTPUBLINKQRY_URL_PATH"].ToString().Trim('\\');

                string[] paramArr = row["JTPUBLINKQRY_PARAMS"].ToString().Split(',');
                string[] valueArr = row["JTPUBLINKQRY_REPLACEVALUE"].ToString().Split(',');
                object[] args = new object[valueArr.Length];
                for (int i = 0; i < valueArr.Length; i++)
                {
                    //string vsValue = valueArr[i];
                    //vsValue = getReplaceValue(vsValue, rowData);
                    //args[i]=vsValue;					 
                    string vsValue = valueArr[i];
                    if (!string.IsNullOrEmpty(vsValue) && rowData.DataView.Table.Columns.Contains(vsValue) && rowData[vsValue] != DBNull.Value)
                    {
                        //在这里替换 argumentstring的值
                        vsValue = rowData[valueArr[i]].ToString();
                    }
                    if (i == 3 && vsValue != "")
                    {
                        Regex regex = new Regex("\\[\\w+\\]", RegexOptions.IgnoreCase);//匹配字段[field]
                        MatchCollection matchCollection = regex.Matches(vsValue);
                        foreach (Match match in matchCollection)
                        {
                            string expval = match.Value;
                            string field = expval.Substring(1, expval.Length - 2);
                            if (rowData.DataView.Table.Columns.Contains(field) && rowData[field] != DBNull.Value)
                            {
                                vsValue = vsValue.Replace(expval, rowData[field].ToString());//批量替换表达式中的字段值
                            }
                        }
                    }
                    args[i] = vsValue;
                }
                invokeAdpFrm(args);
                //ReflectForm refFrm = new ReflectForm(vsPath, row["JTPUBLINKQRY_CLASS"].ToString(), args);
                //object objfrm = refFrm.returnFormObject();
                //Form frm = objfrm as Form;
                //if (this.OpenStyle == "show")
                //{
                //    frm.Text = "联查--" + row["JTPUBLINKQRY_NAME"].ToString();
                //    frm.StartPosition = FormStartPosition.CenterScreen;
                //    frm.ShowDialog();
                //}
                //else
                //{
                //    Genersoft.Platform.AppFramework.UIService.UIContent dUI = new Genersoft.Platform.AppFramework.UIService.UIContent(frm, "联查--" + row["JTPUBLINKQRY_NAME"].ToString(), null);
                //    Genersoft.Platform.AppFrameworkGui.Gui.WorkbenchSingleton.Workbench.ShowView(dUI);
                //}
            }
            else
            {//win窗体
                string vsPath = Application.StartupPath.TrimEnd('\\') + "\\" + row["JTPUBLINKQRY_URL_PATH"].ToString().Trim('\\');
                ReflectForm refFrm = new ReflectForm(vsPath, row["JTPUBLINKQRY_CLASS"].ToString());
                string[] paramArr = row["JTPUBLINKQRY_PARAMS"].ToString().Split(',');
                string[] valueArr = row["JTPUBLINKQRY_REPLACEVALUE"].ToString().Split(',');
                for (int i = 0; i < paramArr.Length; i++)
                {
                    if (!string.IsNullOrEmpty(paramArr[i]))
                    {
                        string vsValue = valueArr[i];
                        vsValue = getReplaceValue(vsValue, rowData);
                        /*
                         PropertyInfo propertyInfo = frm.GetType().GetProperty(name);
                         FieldInfo fi = frm.GetType().GetField(name);*/
                        refFrm.SetProperty(paramArr[i], vsValue);
                    }
                }
                object objfrm = refFrm.returnFormObject();
                Form frm = objfrm as Form;
                if (this.OpenStyle == "show")
                {
                    frm.Text = "联查--" + row["JTPUBLINKQRY_NAME"].ToString();
                    frm.Show();
                }
                else
                {
                    //Genersoft.Platform.AppFramework.UIService.UIContent dUI = new Genersoft.Platform.AppFramework.UIService.UIContent(frm, "联查--" + row["JTPUBLINKQRY_NAME"].ToString(), null);
                    //Genersoft.Platform.AppFrameworkGui.Gui.WorkbenchSingleton.Workbench.ShowView(dUI);
                }
            }
        }


        public void invokeAdpFrm(object[] args)
        {
            //XForm form = new XForm(args[0].ToString(), args[1].ToString(), args[2].ToString(), args[3].ToString(), ClientContext.Current.FramworkState);

            //form.StartPosition = FormStartPosition.CenterParent;
            //DialogResult result = form.ShowDialog();
        }

        private string getReplaceValue(string paramName, DataRowView selRow)
        {
            string vsValue = "";

            if (!string.IsNullOrEmpty(paramName))
            {
                if (selRow.DataView.Table.Columns.Contains(paramName) && selRow[paramName] != DBNull.Value)
                {
                    vsValue = selRow[paramName].ToString();
                }
                else if (!string.IsNullOrEmpty(this.QryParam) && !string.IsNullOrEmpty(this.QryValue))
                {//从传递值中取数
                    if (("&" + this.QryParam + "&").IndexOf("&" + paramName + "&") != -1)
                    {
                        for (int i = 0; i < QryParamArr.Length; i++)
                        {
                            string qryPrm = QryParamArr[i];
                            if (qryPrm.Equals(paramName))
                            {
                                vsValue = QryValueArr[i];
                                break;
                            }
                        }
                    }

                }
            }
            return vsValue;
        }
        #endregion
        #region 帐表设置事件
        private void btntitleSave_Click(object sender, EventArgs e)
        {
            dtLSZBGS.Rows[0]["F_CAPT"] = tbTitle.Text;
            dtLSZBGS.Rows[0]["F_CAPH"] = tbTitleH.Text;
            dtLSZBGS.Rows[0]["F_SCAPH"] = tbSubTitleH.Text;
            dtLSZBGS.Rows[0]["F_BTGD"] = tbcolH.Text;
            dtLSZBGS.Rows[0]["F_ROWH"] = tbrowh.Text;
            string psBH = dtLSZBGS.Rows[0]["F_GSBH"].ToString();
            string psMC = dtLSZBGS.Rows[0]["F_GSMC"].ToString();
            string psType = "edit";
            bool defAdd = false;
            if (string.IsNullOrEmpty(dtLSZBGS.Rows[0]["F_DWBH"].ToString().Trim())
                && !string.IsNullOrEmpty(this.PsDWBH))
            {
                psType = "add";
                defAdd = true;
            }
            if (chColSaveAs.Checked)
            {
                psMC = tbColSaveAs.Text;
                if (psMC.Trim() == "")
                {
                    MessageBox.Show("另存名称不能为空！", "另存提示", MessageBoxButtons.OK);
                    return;
                }
                DataRow[] rows = dtQryStyle.Select("F_GSMC='" + psMC + "'");
                if (rows.Length > 0)
                {
                    MessageBox.Show("另存名称不能重复！", "另存提示", MessageBoxButtons.OK);
                    return;
                }
                object obj = dtQryStyle.Compute("max(F_GSBH)", "true");
                int maxbh = Convert.ToInt32(obj) + 1;
                psBH = maxbh.ToString().PadLeft(2, '0');
                psType = "add";
            }
            BBGsHelp.saveBBTitle(DevQryPubFun.GSYDBSrc, mgr, dtLSZBGS, PsDWBH, PsYear, psType, psBH, psMC);
            BBGsHelp.saveBBSubTitle(DevQryPubFun.GSYDBSrc, mgr, dtLSOTGS, PsDWBH, PsYear, psType, psBH);
            //字段保存
            BBGsHelp.saveBBRowGS(DevQryPubFun.GSYDBSrc, mgr, dtGridRow, PsDWBH, PsYear, psType, psBH);
            DevBBTitleFontMgr.saveFont(titleFont, this.PsID);
            if (psType == "add")
            {
                DataRow row = dtQryStyle.NewRow();
                row["F_GSBH"] = psBH;
                row["F_GSMC"] = psMC;
                dtQryStyle.Rows.Add(row);
                if (!defAdd)
                    selectedFormatIndex = cmbColSaved.Properties.Items.Count;
                else
                    selectedFormatIndex = cmbColSaved.Properties.Items.Count - 1;
                getDataTitle();
            }
        }

        private void btntitleUse_Click(object sender, EventArgs e)
        {
            selectedFormatIndex = cmbColSaved.SelectedIndex;
            dtLSZBGS.Rows[0]["F_CAPT"] = tbTitle.Text;
            dtLSZBGS.Rows[0]["F_CAPH"] = tbTitleH.Text;
            dtLSZBGS.Rows[0]["F_SCAPH"] = tbSubTitleH.Text;
            dtLSZBGS.Rows[0]["F_BTGD"] = tbcolH.Text;
            dtLSZBGS.Rows[0]["F_ROWH"] = tbrowh.Text;
            if (!string.IsNullOrEmpty(this.PsTitleTable))
            {
                DataRow[] rows = dtQryStyle.Select("F_GSMC='" + cmbColSaved.SelectedItem + "'");
                dtLSTIGS = getTIGSDatatable("F_GSBH", rows[0]["F_GSBH"].ToString());
            }
            bindGrid();
        }

        private void btxttitleFont_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            switch (((ButtonEdit)sender).Name)
            {
                case "btxttitleFont":
                    ((ButtonEdit)sender).Text = getFont(ref titleFont.titleFont);
                    break;
                case "btxtSubTitleFont":
                    ((ButtonEdit)sender).Text = getFont(ref titleFont.subTitleFont);
                    break;
                case "btxtColumnFont":
                    ((ButtonEdit)sender).Text = getFont(ref titleFont.columnFont);
                    break;
                case "btxtRowFont":
                    ((ButtonEdit)sender).Text = getFont(ref titleFont.rowFont);
                    break;
            }
        }
        #endregion
        #region 帐表格式 设置字体
        /// <summary>
        /// buttonedit 获取设置字体
        /// </summary>
        /// <param name="psPrtFont"></param>
        /// <returns></returns>
        private string getFont(ref PrintFont psPrtFont)
        {
            Font vsFont = new Font("宋体", 9);
            fontDialog1.Font = new Font(psPrtFont.Name, psPrtFont.Size, psPrtFont.Style);
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                vsFont = fontDialog1.Font;
            }
            psPrtFont.Name = vsFont.Name;
            psPrtFont.Size = vsFont.Size;
            psPrtFont.Style = vsFont.Style;
            string vsreval = psPrtFont.Name + " " + psPrtFont.Size + " " + psPrtFont.Style;
            return vsreval;
        }
        /// <summary>
        /// 启动时设置默认字体
        /// </summary>
        private void setTitleFont()
        {
            titleFont = DevBBTitleFontMgr.getFont(this.PsID);
            btxttitleFont.Text = titleFont.titleFont.Name + " " + titleFont.titleFont.Size + " " + titleFont.titleFont.Style.ToString();
            btxtSubTitleFont.Text = titleFont.subTitleFont.Name + " " + titleFont.subTitleFont.Size + " " + titleFont.subTitleFont.Style.ToString();
            btxtColumnFont.Text = titleFont.columnFont.Name + " " + titleFont.columnFont.Size + " " + titleFont.columnFont.Style.ToString();
            btxtRowFont.Text = titleFont.rowFont.Name + " " + titleFont.rowFont.Size + " " + titleFont.rowFont.Style.ToString();

        }

        private void setBBSet()
        {
            //设置帐表格式信息
            tbTitle.Text = dtLSZBGS.Rows[0]["F_CAPT"].ToString();
            tbTitleH.Text = dtLSZBGS.Rows[0]["F_CAPH"].ToString();
            tbSubTitleH.Text = dtLSZBGS.Rows[0]["F_SCAPH"].ToString();
            tbcolH.Text = dtLSZBGS.Rows[0]["F_BTGD"].ToString();
            tbrowh.Text = dtLSZBGS.Rows[0]["F_ROWH"].ToString();
            gridSubTitle.DataSource = dtLSOTGS;
            gridrow.DataSource = dtGridRow;
        }
        #endregion

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
        private frmDevQryShow _MainForm;
        public frmDevQryShow MainForm
        {
            set { this._MainForm = value; }
            get { return this._MainForm; }
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
        public string PsLinkCol
        {
            set { this._psLinkCol = value; }
            get { return this._psLinkCol; }
        }
        public string PsLinkPathArg
        {
            set { this._psLinkPathArg = value; }
            get { return this._psLinkPathArg; }
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
        private string _psLinkCaption = "";
        public string PsLinkCaption
        {
            set { this._psLinkCaption = value; }
            get { return this._psLinkCaption; }
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

        private string[] _ParamArr;
        public string[] ParamArr
        {
            set { this._ParamArr = value; }
            get { return this._ParamArr; }
        }

        private string[] _ValueArr;
        public string[] ValueArr
        {
            set { this._ValueArr = value; }
            get { return this._ValueArr; }
        }


        private string _QryParam;
        private string[] QryParamArr = null;
        /// <summary>
        /// 多个参数用&拆分
        /// </summary>
        public string QryParam
        {
            set
            {
                this._QryParam = value;
                if (!string.IsNullOrEmpty(this._QryParam)) QryParamArr = this._QryParam.Split('&');
            }
            get { return this._QryParam; }
        }
        private string _QryValue;
        private string[] QryValueArr = null;
        /// <summary>
        /// 多个参数用&拆分，值有多个如时间等用`（esc下键）拆分
        /// </summary>
        public string QryValue
        {
            set
            {
                this._QryValue = value;
                if (!string.IsNullOrEmpty(this._QryValue))
                    QryValueArr = this._QryValue.Split('&');
            }
            get { return this._QryValue; }
        }
        #endregion 查询变量

        private void chColSaveAs_CheckedChanged(object sender, EventArgs e)
        {
            if (chColSaveAs.Checked)
            {
                tbColSaveAs.Enabled = true;
                tbColSaveAs.Focus();
            }
            else
            {
                tbColSaveAs.Enabled = false;
                tbColSaveAs.Text = "";
            }
        }

        private void LoadPivotFrm()
        {
            try
            {
                blLoadPivot = true;
                frmPivot = new frmDevPivotShow();
                frmPivot.Title = this.PsTitle;
                frmPivot.DtLSTIGS = dtLSTIGS;
                frmPivot.DsData = dsData;
                frmPivot.ProcessID = this.ProcessID;
                frmPivot.PsID = this.PsID;
                frmPivot.mgr = this.mgr;
                frmPivot.PsUsr = this.PsUsr;
                frmPivot.PsSLJD = this.PsSLJD;
                frmPivot.PsJEJD = this.PsJEJD;
                frmPivot.TopLevel = false;
                frmPivot.Location = new Point(0, 0);
                frmPivot.TopMost = false;
                frmPivot.ControlBox = false;
                frmPivot.Parent = MainForm.tabpagePivot;
                frmPivot.Dock = DockStyle.Fill;
                frmPivot.IsPivot = this.IsPivot;
                frmPivot.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (this.IsPivot.CompareTo("0") > 0)
            {
                this.MainForm.tabctrl.SelectedTabPageIndex = 1;
                if (frmPivot != null)
                {
                    frmPivot.Activate();
                    frmPivot.Show();
                }
            }

        }

        private void barEditFormat_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(barEditFormat.EditValue.ToString()))
            {
                DataRow[] rows = dtQryStyle.Select("F_GSMC='" + barEditFormat.EditValue + "'");
                if (rows.Length > 0)
                {
                    dtGridRow.Rows.Clear();
                    dtLSOTGS = getDataTableFromDataTableByWhere(dtLSOTGSOrg, "F_GSBH", rows[0]["F_GSBH"].ToString());
                    if (dtLSTIGSOrg != null)
                    {
                        dtLSTIGS = getDataTableFromDataTableByWhere(dtLSTIGSOrg, "F_GSBH", rows[0]["F_GSBH"].ToString());
                        dtGridRow = dtLSTIGS.Copy();
                    }
                    else
                    {
                        DataTable dtdef = getDataTableFromDataTableByWhere(dtLSTIGSDef, "F_GSBH", rows[0]["F_GSBH"].ToString());
                        DataTable dtqry = getDataTableFromDataTableByWhere(dtDevQry, "F_GSBH", rows[0]["F_GSBH"].ToString());
                        dtGridRow.Merge(dtdef);
                        dtGridRow.Merge(dtqry);
                        dtLSTIGS = getTIGSDatatable("F_GSBH", rows[0]["F_GSBH"].ToString());
                    }
                    dtLSZBGS = getDataTableFromDataTableByWhere(dtLSZBGSOrg, "F_GSBH", rows[0]["F_GSBH"].ToString());
                    cmbColSaved.SelectedIndex = selectedFormatIndex;
                    bindFirstGrid();
                }
            }
        }
        private DataTable getTIGSDatatable(string fieldName, string fieldValue)
        {
            DataTable dtnew = new DataTable();
            dtnew = dtLSTIGSOrg.Copy();
            foreach (DataRow row in dtnew.Rows)
            {
                DataRow[] rows = dtGridRow.Select("F_FIELD='" + row["F_FIELD"] + "'");
                if (rows.Length > 0)
                {//F_TIBH,F_TEXT,F_WIDTH,F_YHBZ,F_ALIGN
                    row["F_WIDTH"] = rows[0]["F_WIDTH"];
                    row["F_YHBZ"] = rows[0]["F_YHBZ"];
                    row["F_ALIGN"] = rows[0]["F_ALIGN"];
                }
                else
                {
                    switch (row["F_TYPE"].ToString())
                    {
                        case "D":
                            rows = dtGridRow.Select("F_FIELD='F_DT_FIELD'");
                            if (rows.Length > 0)
                            {//F_TIBH,F_TEXT,F_WIDTH,F_YHBZ,F_ALIGN
                                row["F_WIDTH"] = rows[0]["F_WIDTH"];
                                row["F_ALIGN"] = rows[0]["F_ALIGN"];
                            }
                            break;
                        case "N":
                            rows = dtGridRow.Select("F_FIELD='F_NUM_FIELD'");
                            if (rows.Length > 0)
                            {//F_TIBH,F_TEXT,F_WIDTH,F_YHBZ,F_ALIGN
                                row["F_WIDTH"] = rows[0]["F_WIDTH"];
                                row["F_ALIGN"] = rows[0]["F_ALIGN"];
                            }
                            break;
                        default:
                            rows = dtGridRow.Select("F_FIELD='F_TXT_FIELD'");
                            if (rows.Length > 0)
                            {//F_TIBH,F_TEXT,F_WIDTH,F_YHBZ,F_ALIGN
                                row["F_WIDTH"] = rows[0]["F_WIDTH"];
                                row["F_ALIGN"] = rows[0]["F_ALIGN"];
                            }
                            break;
                    }
                    //DataRow rowdev = dtqry.Select("");
                }
            }
            return dtnew;
        }


        private DataTable getDataTableFromDataTableByWhere(DataTable dt, string fieldName, string fieldValue)
        {
            DataTable dtnew = new DataTable();
            dtnew = dt.Clone();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row[fieldName].ToString() == fieldValue)
                    dtnew.Rows.Add(row.ItemArray);
            }
            return dtnew;
        }

        private void cmbColSaved_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindGSCtrl();
        }

        private void bindGSCtrl()
        {
            if (!string.IsNullOrEmpty(cmbColSaved.SelectedItem.ToString()))
            {
                DataRow[] rows = dtQryStyle.Select("F_GSMC='" + cmbColSaved.SelectedItem + "'");
                if (rows.Length > 0)
                {
                    dtLSOTGS = getDataTableFromDataTableByWhere(dtLSOTGSOrg, "F_GSBH", rows[0]["F_GSBH"].ToString());
                    dtGridRow.Rows.Clear();
                    dtLSZBGS = getDataTableFromDataTableByWhere(dtLSZBGSOrg, "F_GSBH", rows[0]["F_GSBH"].ToString());
                    if (dtLSTIGSOrg != null)
                    {
                        dtLSTIGS = getDataTableFromDataTableByWhere(dtLSTIGSOrg, "F_GSBH", rows[0]["F_GSBH"].ToString());
                        dtGridRow = dtLSTIGS.Copy();
                    }
                    else
                    {
                        DataTable dtdef = getDataTableFromDataTableByWhere(dtLSTIGSDef, "F_GSBH", rows[0]["F_GSBH"].ToString());
                        DataTable dtqry = getDataTableFromDataTableByWhere(dtDevQry, "F_GSBH", rows[0]["F_GSBH"].ToString());
                        dtGridRow.Merge(dtdef);
                        dtGridRow.Merge(dtqry);
                        dtLSTIGS = getTIGSDatatable("F_GSBH", rows[0]["F_GSBH"].ToString());
                    }
                    setBBSet();
                }
            }
        }
        private void dockBBSet_ClosedPanel(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            getDataTitle();
            bindGSCtrl();
        }

        private void frmDevBBShow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.frmPivot != null)
            {
                frmPivot.IsSaveVisited = true;
                frmPivot.Close();
            }
            //本地缓存 存储到文件
            if (this.IsUseLocal == "0")
            {
                //缓存数据
                if (dsData.Tables.Count > 0)
                {
                    LocalDataOper.saveToFile(dsData, this.PsID, "DB_" + this.PsDWBH + "_" + this.PsUsr);
                }
                //缓存格式表
                if (dtLSTIGSOrg.Rows.Count > 0)
                {
                    LocalDataOper.saveToFile(dtLSTIGSOrg.DataSet, this.PsID, "GS_" + this.PsDWBH + "_" + this.PsUsr);
                }
                //缓存联查表格式 dtLinkQry
                if (dtLinkQry.Rows.Count > 0)
                {
                    LocalDataOper.saveToFile(dtLinkQry.DataSet, this.PsID, "LK_" + this.PsDWBH + "_" + this.PsUsr);
                }
            }

        }



        private void setDefaultDtDevQry(string dwbh, string gsbh)
        {
            DataRow row = dtDevQry.NewRow();
            //F_DWBH,F_ID,F_GSBH,F_TIBH,F_FIELD,F_TEXT,F_TYPE,F_ALIGN,F_WIDTH
            row["F_DWBH"] = dwbh;
            row["F_ID"] = this.PsID;
            row["F_GSBH"] = gsbh;
            row["F_TIBH"] = "900";
            row["F_FIELD"] = "F_TXT_FIELD";
            row["F_TEXT"] = "文本字段";
            row["F_TYPE"] = "C";
            row["F_ALIGN"] = "L";
            row["F_WIDTH"] = "100";
            dtDevQry.Rows.Add(row);

            row = dtDevQry.NewRow();
            row["F_DWBH"] = dwbh;
            row["F_ID"] = this.PsID;
            row["F_GSBH"] = gsbh;
            row["F_TIBH"] = "901";
            row["F_FIELD"] = "F_DT_FIELD";
            row["F_TEXT"] = "时间字段";
            row["F_TYPE"] = "D";
            row["F_ALIGN"] = "L";
            row["F_WIDTH"] = "100";
            dtDevQry.Rows.Add(row);

            row = dtDevQry.NewRow();
            row["F_DWBH"] = dwbh;
            row["F_ID"] = this.PsID;
            row["F_GSBH"] = gsbh;
            row["F_TIBH"] = "902";
            row["F_FIELD"] = "F_NUM_FIELD";
            row["F_TEXT"] = "数字字段";
            row["F_TYPE"] = "N";
            row["F_ALIGN"] = "R";
            row["F_WIDTH"] = "100";
            dtDevQry.Rows.Add(row);

        }

        private void btnDelGS_Click(object sender, EventArgs e)
        {
            if (cmbColSaved.Properties.Items.Count > 0)
            {
                if (cmbColSaved.SelectedItem.ToString() == "系统格式")
                {
                    MessageBox.Show("系统格式不能删除！");
                    return;
                }
                if (!string.IsNullOrEmpty(cmbColSaved.SelectedItem.ToString())
                    && MessageBox.Show("确实要删除查询格式'" + cmbColSaved.SelectedItem + "'吗？", "删除提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataRow[] rows = dtQryStyle.Select("F_GSMC='" + cmbColSaved.SelectedItem + "'");
                    if (rows.Length > 0)
                    {
                        string sql = @"begin 
                        delete from EAOTGS{0} where F_ID = '{1}'   and F_GSBH='{2}' and  F_DWBH='{3}';
                        delete from EAZBGS{0} where F_ID = '{1}'   and F_GSBH='{2}' and F_DWBH='{3}';
                        delete from EATIGS{0} where F_ID = '{1}'   and F_GSBH='{2}' and F_DWBH='{3}'; 
                        delete   from EATIGSDEVQRY  where F_ID='{1}' and F_GSBH='{2}' and F_DWBH='{3}';
                        end;";
                        sql = string.Format(sql, this.PsYear, this.PsID, rows[0]["F_GSBH"].ToString(), this.PsDWBH);
                        WebSvrGetData.execsql(DevQryPubFun.GSYDBSrc, sql, mgr);
                        selectedFormatIndex = cmbColSaved.SelectedIndex == 0 ? 0 : cmbColSaved.SelectedIndex - 1;
                        getDataTitle();
                    }

                }
            }
        }


        public bool getLocalData(string psType, ref DataSet ds)
        {
            bool bl = true;
            ds = new DataSet();
            try
            {
                ds = LocalDataOper.LoadFromFile(this.PsID, psType + "_" + this.PsDWBH + "_" + this.PsUsr);
            }
            catch (Exception ex)
            {
                bl = false;
            }
            return bl;
        }







        private void repCheckEdt_EditValueChanged(object sender, EventArgs e)
        {

            if ((sender as DevExpress.XtraEditors.CheckEdit).Checked)
                NumFormat = "N";
            else
                NumFormat = "F";

            bindGrid();

        }

        private void repcheckNUM_EditValueChanged(object sender, EventArgs e)
        {
            NumRedShow = (sender as DevExpress.XtraEditors.CheckEdit).Checked;
            bindGrid();
        }

        private void repcheckZero_EditValueChanged(object sender, EventArgs e)
        {
            ShowZeroValue = (sender as DevExpress.XtraEditors.CheckEdit).Checked;
            bindGrid();
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            //值为0 显示空
            decimal dec;
            if (Decimal.TryParse(e.DisplayText, out dec))
            {
                if (dec == 0 && !ShowZeroValue)
                    e.DisplayText = "";
                if (dec < 0 && e.DisplayText.IndexOf('-') == 0)
                {
                    if (NumRedShow)
                    {
                        e.Appearance.BackColor = Color.Red;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                        e.Appearance.BackColor = Color.White;

                    }
                }
            }


            /*
             
              if (e.RowHandle==1)
                {

                    //字体颜色，也可改变背景颜色等属性
                    e.Appearance.ForeColor = Color.Red;
                }
               if (e.RowHandle==3)
                {

                    //字体颜色，也可改变背景颜色等属性
                    e.Appearance.ForeColor = Color.Red;
                } 
            System.Drawing.Drawing2D.ColorBlend colorBlend = null;
            colorBlend = new System.Drawing.Drawing2D.ColorBlend();
            colorBlend.Colors = new Color[] { Color.Orchid, Color.White, Color.Orchid };
            colorBlend.Positions = new float[] { 0.0f, 0.5f, 1.0f };

            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds,
                Color.White, Color.Black, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
            //Provide custom color blending
            brush.InterpolationColors = colorBlend;
            //Fill the cell's background
            using (brush)
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            //Draw the cell's text
            e.Graphics.DrawString("111", e.Appearance.Font, Brushes.Black, e.Bounds);
            //Prohibit the default painting
            e.Handled = true;*/

        }


        private void gridview_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                e.Info.DisplayText = (e.RowHandle + 1).ToString().Trim();
            }
        }












    }


}
