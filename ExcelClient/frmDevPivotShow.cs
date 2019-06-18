using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraCharts;
using DevExpress.LookAndFeel;

namespace ExcelClient
{
    public partial class frmDevPivotShow : DevExpress.XtraEditors.XtraForm
    {
        public frmDevPivotShow()
        {
            InitializeComponent();
        }
        #region 公共变量
        private string _psJEJD;
        public string PsJEJD
        {
            set { this._psJEJD = value; }
            get { return this._psJEJD; }
        }
        private string _psSLJD;
        public string PsSLJD
        {
            set { this._psSLJD = value; }
            get { return this._psSLJD; }
        }
        private string title;
        public string Title
        {
            set { this.title = value; }
            get { return this.title; }
        }
        private DataTable dtLSTIGS;
        public DataTable DtLSTIGS
        {
            set {                    
                this.dtLSTIGS = (value as DataTable).Copy(); 
                if(!dtLSTIGS.Columns.Contains("F_TSTEXT")){
                    DataColumn col=new DataColumn("F_TSTEXT",Type.GetType("System.String"));
                    dtLSTIGS.Columns.Add(col);
                    foreach(DataRow row in dtLSTIGS.Rows){
                        row["F_TSTEXT"] = row["F_TEXT"];
                    }
                } 
                int len=dtLSTIGS.Rows.Count;
                for (int i=len-1;i>=0;i--){
                     DataRow row=dtLSTIGS.Rows[i];
                     if(dtLSTIGS.Columns.Contains("F_TSHID") && row["F_TSHID"].ToString()=="1")
                         dtLSTIGS.Rows.Remove(row);
                }
                
            }
            get { 
                return this.dtLSTIGS;
             }
        }
        private DataSet dsData;
        public DataSet DsData
        {
            set { this.dsData = value; }
            get { return this.dsData; }
        }
        private string _psid = "";
        public string PsID
        {
            set { this._psid = value; }
            get { return this._psid; }
        }
        private string _psUsr = "";
        public string PsUsr
        {
            set { this._psUsr = value; }
            get { return this._psUsr; }
        }
        private bool _isSaveVisited = false;
        public bool IsSaveVisited
        {
            set { this._isSaveVisited = value; }
        }
        private string _processID = "";
        public string ProcessID
        {
            set { this._processID = value; }
            get { return this._processID; }
        }
        private string _IsPivot = "0";
        public string IsPivot
        {
            set { this._IsPivot = value; }
            get { return this._IsPivot; }
        }

        private DataTable dtFav;
        private DataTable dtFavVisit;
        private DataTable dtFavFilter;

        WsGetDataClient.WSGetData _mgr;
        public WsGetDataClient.WSGetData mgr
        {
            set { _mgr = value; }
        }

        private DevExpress.XtraPivotGrid.PivotGridField[] pivotfields;
        private DataRow[] rowsField;

        private string QryHashString = "";
        private string NumFormat = "F";
        private bool NumRedShow = true;

        #endregion

        private void frmDevPivotShow_Load(object sender, EventArgs e)
        {
            //UserLookAndFeel.Default.SetSkinStyle("Office 2013");

            //pivotGridControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            //pivotGridControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            //pivotGridControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
 
             
           
            chkTotalRow.ValueChecked = true;
            chkTotalCol.ValueChecked = true;
            dockChart.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            this.Text = "";
            //获取hash值
            QryHashString = ComputeHash.getHashStringFromDataTableField(dsData.Tables[0]);
            //加载pivotgrid 
            fullFavData(this.PsID, this.PsUsr);
            bindFavCtrl("");
            bindFirstGrid();
            dockField.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;

            if (IsPivot == "2")
            { 
                barBtnChartClick();

            }
            else if (IsPivot == "3")
            {
                dockField.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                barBtnChartClick();
            }
            
        }
        private void setFilterDefault()
        {
            dockFilter.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            lblVal.Visible = false;
            tbval1.Visible = false;
            cmbField.Properties.Items.Clear();
            foreach (PivotGridField pField in pivotGridControl1.Fields)
            {
                if (pField.Visible)
                {
                    cmbField.Properties.Items.Add(pField.Caption);
                }
            }
        }

        private void btnFavSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbFavName.Text))
            {
                MessageBox.Show("收藏名称不能为空！", "保存提示", MessageBoxButtons.OK);
                tbFavName.Focus();
                return;
            }
            string sql = "select '1' from JTPUBQRYFAV where  QRYFAV_QRYID='{0}' and QRYFAV_NAME='{1}'   ";//and QRYFAV_HASH='{2}'
            sql = string.Format(sql, this.PsID, tbFavName.Text);//, this.QryHashString
            DataTable dtnum = getDataTable(sql);
            if (dtnum.Rows.Count > 0 && dtnum.Rows[0][0].ToString() == "1")
            {
                MessageBox.Show("收藏名称已存在，不能保存！", "保存提示", MessageBoxButtons.OK);
                return;
            }
            string fieldRow = "";
            string fieldCol = "";
            string fieldData = "";
            string fieldSel = "";
            string fieldFilter = "";
            string QRYFAV_FILTERVAL = "";
            if ((tbVal.Visible && tbVal.Text.Trim() != "") || (tbval1.Visible && tbval1.Text.Trim() != ""))
            {
                //cmbField.SelectedText cmbCMP.Text tbVal.Text tbval1.Text
                QRYFAV_FILTERVAL = cmbField.SelectedText + "^" + cmbCMP.Text + "^" + tbVal.Text + "^" + tbval1.Text;
            }
            getFieldString(ref fieldRow, ref fieldCol, ref fieldData, ref fieldSel, ref fieldFilter);
            sql = "insert into JTPUBQRYFAV (QRYFAV_QRYID,QRYFAV_NAME,QRYFAV_LOCATION,QRYFAV_CREATER,QRYFAV_ROW,QRYFAV_COL,QRYFAV_DATA,QRYFAV_SEL,QRYFAV_FILTER,QRYFAV_HASH,QRYFAV_DEFAULT,QRYFAV_FILTERVAL)"
                + "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','0','{10}'); ";
            sql = string.Format(sql, this.PsID, tbFavName.Text, rgFavLocation.Properties.Items[rgFavLocation.SelectedIndex].Value,
                this.PsUsr, fieldRow, fieldCol, fieldData, fieldSel, fieldFilter, this.QryHashString, QRYFAV_FILTERVAL);
            sql = "begin  " + sql + saveFilterValue(tbFavName.Text) + "   end;";
            WebSvrGetData.execsql(DevQryPubFun.GSYDBSrc, sql, _mgr);
            fullFavData(this.PsID, this.PsUsr);
            bindFavCtrl(tbFavName.Text);
            bindFavList();
            tbFavName.Text = "";
        }

        private void btnFavDel_Click(object sender, EventArgs e)
        {
            if (lstFavSaved.Items.Count > 0)
            {
                if (!string.IsNullOrEmpty(lstFavSaved.SelectedValue.ToString())
                    && MessageBox.Show("确实要删除'" + lstFavSaved.SelectedValue + "'吗？", "删除提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string sql = " begin delete from JTPUBQRYFAV where  QRYFAV_QRYID='{0}' and QRYFAV_NAME='{1}' ;";//and QRYFAV_HASH='{2}'
                    sql = string.Format(sql, this.PsID, lstFavSaved.SelectedValue.ToString().Split(':')[1]);//, this.QryHashString
                    sql += string.Format(@"delete from JTPUBQRYFAVFILTER where QRYFILTER_QRYID='{0}'
and QRYFILTER_USER='{1}' and QRYFILTER_HASH='{2}' and QRYFILTER_NAME='{3}' ; end;", this.PsID, this.PsUsr, this.QryHashString, lstFavSaved.SelectedValue.ToString().Split(':')[1]);
                    WebSvrGetData.execsql(DevQryPubFun.GSYDBSrc, sql, _mgr);
                    fullFavData(this.PsID, this.PsUsr);
                    bindFavCtrl("");
                    bindFavList();
                }
            }
        }

        private void btnFavClose_Click(object sender, EventArgs e)
        {
            dockFav.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
        }

        private void barFavSel_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(barFavSel.EditValue.ToString()))
            {
                bindFavSelGrid(barFavSel.EditValue.ToString().Split(':')[1]);
            }
            else
            {

                string vsFavName = "%";
                if (dtFavVisit.Rows.Count > 0)
                {
                    bindFavGrid(rowsField, dtFavVisit.Rows[0]["QRYFAVVISIT_ROW"].ToString(), dtFavVisit.Rows[0]["QRYFAVVISIT_COL"].ToString(),
        dtFavVisit.Rows[0]["QRYFAVVISIT_DATA"].ToString(), dtFavVisit.Rows[0]["QRYFAVVISIT_SEL"].ToString(),
        dtFavVisit.Rows[0]["QRYFAVVISIT_FILTER"].ToString(), "%");

                }
                else
                {
                    vsFavName = "";
                    bindDefaultGrid(rowsField);
                }
                bindPivotGrid(vsFavName);
            }
        }

        private void lstFavSaved_SelectedValueChanged(object sender, EventArgs e)
        {
            if (lstFavSaved.SelectedItem != null && !string.IsNullOrEmpty(lstFavSaved.SelectedItem.ToString()))
            {
                string[] vsFavArr = lstFavSaved.SelectedItem.ToString().Split(':');
                //rgFavLocation.SelectedIndex=vsFavArr[0] == "公有" ? 0 : 1;
                //tbFavName.Text = vsFavArr[1];
                DataRow[] rowFav = dtFav.Select("QRYFAV_NAME='" + vsFavArr[1] + "'");
                if (rowFav.Length > 0 && rowFav[0]["QRYFAV_CREATER"].ToString() == this.PsUsr)
                {
                    btnFavDel.Enabled = true;
                }
                else
                {
                    btnFavDel.Enabled = false;
                }
            }
        }



        private void rgFavLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbField.Properties.Items.Clear();
            bindFavList();
        }

        private void barBtnItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string filename = "";
            //DevExpress.XtraBars.BarButtonItem item = sender as DevExpress.XtraBars.BarButtonItem;
            switch (e.Item.Name)
            {
                case "barView":
                    DevPrint vprt = new DevPrint();
                    vprt.PrintID = "pivot" + this.PsID;
                    vprt.PrtCtrl = pivotGridControl1;
                    vprt.Preview();
                    break;
                case "barPrint":
                    DevPrint prt = new DevPrint();
                    prt.PrintID = "pivot" + this.PsID;
                    prt.PrtCtrl = pivotGridControl1;
                    prt.Preview();
                    break;
                case "barsaveasxls":
                    filename = OpenSaveFileDlg(".xls", "excel文件(*.xls)|*.xls");
                    if (filename == "") return;
                    this.pivotGridControl1.ExportToXls(filename);
                    break;
                case "barsaveashtml":
                    break;
                case "":
                    filename = OpenSaveFileDlg(".html", "网页文件(*.html)|*.html");
                    if (filename == "") return;
                    this.pivotGridControl1.ExportToHtml(filename);
                    break;
                case "barsaveaspdf":
                    filename = OpenSaveFileDlg(".pdf", "pdf文件(*.pdf)|*.pdf");
                    if (filename == "") return;
                    this.pivotGridControl1.ExportToPdf(filename);
                    break;
                case "barsaveasTxt":
                    filename = OpenSaveFileDlg(".txt", "文本文件(*.txt)|*.txt");
                    if (filename == "") return;
                    this.pivotGridControl1.ExportToText(filename);
                    break;
                case "barFavAdd":
                    dockFav.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                    break;
                case "barBtnExpand":
                    pivotGridControl1.ExpandAll();
                    break;
                case "barbtnCollapse":
                    pivotGridControl1.CollapseAll();
                    break;
                case "barbtnChart":

                    barBtnChartClick();
                    break;
                case "barItemFilter":
                    dockFilter.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                    break;
                case "barbtndelfilter":
                    delFilter();                    
                    break;

            }

        }
        private void delFilter()
        {
            try
            {

                pivotGridControl1.BeginUpdate();
                foreach (PivotGridField pivotField in pivotGridControl1.Fields)
                {
                    pivotField.FilterValues.Clear();
                    if (pivotField.GetUniqueValues() != null)
                        pivotField.FilterValues.SetValues(pivotField.GetUniqueValues(), DevExpress.XtraPivotGrid.PivotFilterType.Included, true);

                }
                baritemfilterinfo.Caption = "";
            }
            finally
            {
                pivotGridControl1.EndUpdate();
            }
        }
        public void barBtnChartClick()
        {
            dockChart.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            chartControl1.SeriesTemplate.ChangeView(ViewType.Line);
            chartControl1.DataSource = pivotGridControl1;
            chartControl1.SeriesDataMember = "Series";
            chartControl1.SeriesTemplate.ArgumentDataMember = "Arguments";
            chartControl1.SeriesTemplate.ValueDataMembers.AddRange(new string[] { "Values" });

            chartControl1.SeriesTemplate.PointOptions.PointView = PointView.ArgumentAndValues;
            pivotGridControl1.Cells.Selection = new Rectangle(0, 0, pivotGridControl1.Cells.ColumnCount, 2);
        }
        private DataTable getDataTable(string sql)
        {
            DataTable dt = new DataTable();
            DataSet vsds = StringToDataSet.getDataSetFromZipDataFormat(_mgr.getZipDataFormatByte(DevQryPubFun.GSYDBSrc, sql));
            if (vsds.Tables.Count > 0)
                return vsds.Tables[0];
            else
                return dt;
        }

        private void fullFavData(string QryID, string usrID)
        {
            string sql = "select * from JTPUBQRYFAV  "
                + " where QRYFAV_QRYID='{0}' and QRYFAV_HASH='{2}' and (QRYFAV_LOCATION='公有' or(QRYFAV_LOCATION='私有' and QRYFAV_CREATER='{1}') )  order by   QRYFAV_LOCATION,QRYFAV_NAME";
            sql = string.Format(sql, QryID, usrID, this.QryHashString);
            /*天德特殊处理 不按hash值进行过滤 
             */
                sql = "select * from JTPUBQRYFAV  "
                + " where QRYFAV_QRYID='{0}' and (QRYFAV_LOCATION='公有' or(QRYFAV_LOCATION='私有' and QRYFAV_CREATER='{1}') )  order by   QRYFAV_LOCATION,QRYFAV_NAME";
            sql = string.Format(sql, QryID, usrID);
             /* */
            dtFav = getDataTable(sql);
            sql = "SELECT QRYFAVVISIT_QRYID,QRYFAVVISIT_USER,QRYFAVVISIT_ROW,QRYFAVVISIT_COL,QRYFAVVISIT_DATA,QRYFAVVISIT_SEL,QRYFAVVISIT_FILTER FROM JTPUBQRYFAVVISIT "
                + " WHERE QRYFAVVISIT_QRYID='{0}' and QRYFAVVISIT_USER='{1}' and QRYFAVVISIT_HASH='{2}' ";
            sql = string.Format(sql, QryID, usrID, this.QryHashString);
            dtFavVisit = getDataTable(sql);

            sql = "SELECT * FROM JTPUBQRYFAVFILTER "
                + " WHERE QRYFILTER_QRYID='{0}' and QRYFILTER_USER='{1}' and QRYFILTER_HASH='{2}' ";
            sql = string.Format(sql, QryID, usrID, this.QryHashString);
            dtFavFilter = getDataTable(sql);
        }
        private void bindFavCtrl(string selVal)
        {
            barCmbFav.Items.Clear();
            barCmbFav.Items.Add("");
            if (dtFav.Rows.Count > 0)
            {
                foreach (DataRow row in dtFav.Rows)
                {
                    barCmbFav.Items.Add(row["QRYFAV_LOCATION"].ToString() + ":" + row["QRYFAV_NAME"].ToString());
                    if (!string.IsNullOrEmpty(selVal) && row["QRYFAV_NAME"].ToString() == selVal)
                    {
                        rgFavLocation.SelectedIndex = barCmbFav.Items.Count - 1;
                    }                       
                }
                if(string.IsNullOrEmpty(selVal))  rgFavLocation.SelectedIndex = 0; 
            }
        }
        private void bindFavList()
        {
            lstFavSaved.Items.Clear();
            if (dtFav.Rows.Count > 0)
            {
                DataRow[] rows = dtFav.Select("QRYFAV_LOCATION='" +
                    rgFavLocation.Properties.Items[rgFavLocation.SelectedIndex].Value + "'");
                foreach (DataRow row in rows)
                {
                    lstFavSaved.Items.Add(row["QRYFAV_LOCATION"].ToString() + ":" + row["QRYFAV_NAME"].ToString());
                }
            }
        }

        private void bindFirstGrid()
        {
            rowsField = dtLSTIGS.Select("F_FIELD<>'' and F_FIELD<>' ' ", "F_TIBH");// and F_YHBZ<>'1'
            pivotfields = new DevExpress.XtraPivotGrid.PivotGridField[rowsField.Length];
            
            string vsFavName = "";
            if (dtFavVisit.Rows.Count > 0)
            {
                vsFavName = "%";
                bindFavGrid(rowsField, dtFavVisit.Rows[0]["QRYFAVVISIT_ROW"].ToString(), dtFavVisit.Rows[0]["QRYFAVVISIT_COL"].ToString(),
                    dtFavVisit.Rows[0]["QRYFAVVISIT_DATA"].ToString(), dtFavVisit.Rows[0]["QRYFAVVISIT_SEL"].ToString(),
                    dtFavVisit.Rows[0]["QRYFAVVISIT_FILTER"].ToString(), "%");
            }
            else
            {
                if (dtFav.Rows.Count > 0)
                {
                    DataRow[] rowsDefault = dtFav.Select("QRYFAV_DEFAULT='1'");
                    if (rowsDefault.Length > 0)
                    {
                        vsFavName = rowsDefault[0]["QRYFAV_NAME"].ToString();
                        bindFavGrid(rowsField, rowsDefault[0]["QRYFAV_ROW"].ToString(), rowsDefault[0]["QRYFAV_COL"].ToString(),
                         rowsDefault[0]["QRYFAV_DATA"].ToString(), rowsDefault[0]["QRYFAV_SEL"].ToString(),
                         rowsDefault[0]["QRYFAV_FILTER"].ToString(), rowsDefault[0]["QRYFAV_NAME"].ToString() );
                    }
                    else
                    {
                        vsFavName = dtFav.Rows[0]["QRYFAV_NAME"].ToString();
                        bindFavGrid(rowsField, dtFav.Rows[0]["QRYFAV_ROW"].ToString(), dtFav.Rows[0]["QRYFAV_COL"].ToString(),
    dtFav.Rows[0]["QRYFAV_DATA"].ToString(), dtFav.Rows[0]["QRYFAV_SEL"].ToString(), dtFav.Rows[0]["QRYFAV_FILTER"].ToString(),
    dtFav.Rows[0]["QRYFAV_NAME"].ToString() );
                    }
                }
                else
                {
                    bindDefaultGrid(rowsField);
                }
            }
            bindPivotGrid(vsFavName);
        }
        private void bindFavSelGrid(string psName)
        {
            DataRow[] rowFav = dtFav.Select("QRYFAV_NAME='" + psName + "'");
            if (rowFav.Length > 0)
            {
                bindFavGrid(rowsField, rowFav[0]["QRYFAV_ROW"].ToString(), rowFav[0]["QRYFAV_COL"].ToString(),
                   rowFav[0]["QRYFAV_DATA"].ToString(), rowFav[0]["QRYFAV_SEL"].ToString(), rowFav[0]["QRYFAV_FILTER"].ToString(),
                   rowFav[0]["QRYFAV_NAME"].ToString());
                bindPivotGrid(psName);
            }
        }
        private void bindPivotGrid(string psFavName)
        {
            try
            {  
                pivotGridControl1.BeginUpdate();
                pivotGridControl1.DataSource = dsData.Tables[0]; 
                setFilterValue(psFavName);
                setFilterDefault();
                setConditionFilter(psFavName);

            }
            finally
            {
                pivotGridControl1.EndUpdate();
            }
        }

        private int getPivotAreaIndex(string[] fieldAreaArr, string fieldname)
        {
            int rei = 0;

            for (int i = 0; i < fieldAreaArr.Length; i++)
            {
                if (fieldAreaArr[i].IndexOf("~") == -1) break;
                if (fieldAreaArr[i].IndexOf(fieldname) == 0)
                {
                    rei = Convert.ToInt32(fieldAreaArr[i].Split('~')[1]);
                    break;
                }
            }
            return rei;
        }
        private void bindFavGrid(DataRow[] rows, string fieldRow, string fieldCol, string fieldData, string fieldSelect, string fieldFilter, string psFavName)
        {
            try
            {
                pivotGridControl1.BeginUpdate();
                pivotGridControl1.Fields.Clear();
                fieldRow = "," + fieldRow.Trim(',') + ",";
                fieldCol = "," + fieldCol.Trim(',') + ",";
                fieldData = "," + fieldData.Trim(',') + ",";
                fieldSelect = "," + fieldSelect.Trim(',') + ",";
                fieldFilter = "," + fieldFilter.Trim(',') + ",";
                string[] fieldRowArr = fieldRow.Trim(',').Split(',');
                string[] fieldColArr = fieldCol.Trim(',').Split(',');
                string[] fieldDataArr = fieldData.Trim(',').Split(',');
                string[] fieldSelectArr = fieldSelect.Trim(',').Split(',');
                string[] fieldFilterArr = fieldFilter.Trim(',').Split(',');

                for (int i = 0; i < rows.Length; i++)
                {
                    pivotfields[i] = pivotGridControl1.Fields.Add();//new DevExpress.XtraPivotGrid.PivotGridField();
                    pivotfields[i].FieldName = rows[i]["F_FIELD"].ToString();
                    pivotfields[i].Name = "F" + rows[i]["F_FIELD"].ToString();
                    pivotfields[i].Caption = rows[i]["F_TSTEXT"].ToString();
                    string vsfield = "," + rows[i]["F_FIELD"].ToString();

                    pivotfields[i].AreaIndex = getPivotAreaIndex(fieldDataArr, vsfield.Trim(','));
                    if (rows[i].Table.Columns.Contains("F_PSUMTYPE") && rows[i]["F_PSUMTYPE"]!=DBNull.Value)
                    {
                        switch (rows[i]["F_PSUMTYPE"].ToString().ToUpper().Trim())
                        {
                            case "AVERAGE":
                                pivotfields[i].SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.Average;
                                break;
                            case "MAX":
                                pivotfields[i].SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.Max;
                                break;
                            case "MIN":
                                pivotfields[i].SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.Min;
                                break;
                            case "COUNT":
                                pivotfields[i].SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.Count;
                                break;
                            case "VAR":
                                pivotfields[i].SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.Var;
                                break;
                            case "VARP":
                                pivotfields[i].SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.Varp;
                                break;
                            case "STDDEV":
                                pivotfields[i].SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.StdDev;
                                break;
                            case "STDDEVP":
                                pivotfields[i].SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.StdDevp;
                                break;
                            default:
                                pivotfields[i].SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.Sum;
                                break;
                        }
                    }

                    if (fieldRow.ToUpper().IndexOf(vsfield.ToUpper() + "~") != -1)
                    {
                        pivotfields[i].Visible = true;
                        pivotfields[i].Area = PivotArea.RowArea;
                        pivotfields[i].AreaIndex = getPivotAreaIndex(fieldRowArr, vsfield.Trim(','));
                    }
                    else if (fieldCol.ToUpper().IndexOf(vsfield.ToUpper() + "~") != -1)
                    {
                        pivotfields[i].Visible = true;
                        pivotfields[i].Area = PivotArea.ColumnArea;
                        pivotfields[i].AreaIndex = getPivotAreaIndex(fieldColArr, vsfield.Trim(','));
                        
                    }
                    else if (fieldData.ToUpper().IndexOf(vsfield.ToUpper() + "~") != -1)
                    {
                        pivotfields[i].Visible = true;
                        pivotfields[i].Area = PivotArea.DataArea;

                       
                    }
                    else if (fieldFilter.ToUpper().IndexOf(vsfield.ToUpper() + "~") != -1)
                    {
                        pivotfields[i].Visible = true;
                        pivotfields[i].Area = PivotArea.FilterArea;
                        pivotfields[i].AreaIndex = getPivotAreaIndex(fieldFilterArr, vsfield.Trim(','));
                    
                    }
                    else                       
                    {
                        pivotfields[i].Visible = false;
                    }
                        /*if (fieldSelect.ToUpper().IndexOf(","+vsfield.ToUpper()+",") != -1)
                        {
                            pivotfields[i].Visible = false;
                        }
                        else
                        {
                            pivotfields[i].Visible = true;
                            pivotfields[i].Area = PivotArea.DataArea; 
                        }*/
                    setPivotFormat(pivotfields[i],rows[i]);
                }
            }
            finally
            {
                pivotGridControl1.EndUpdate(); 
            }
        }
        private void bindDefaultGrid(DataRow[] rows)
        {
            try
            {
                pivotGridControl1.BeginUpdate();
                pivotGridControl1.Fields.Clear();
                int intRowarea = 0;
                int intdataarea = 0;
                for (int i = 0; i < rows.Length; i++)
                {
                    pivotfields[i] = pivotGridControl1.Fields.Add();// new DevExpress.XtraPivotGrid.PivotGridField();
                    pivotfields[i].FieldName = rows[i]["F_FIELD"].ToString();
                    pivotfields[i].Name = "F" + rows[i]["F_FIELD"].ToString();
                    pivotfields[i].Caption = rows[i]["F_TSTEXT"].ToString();

                    if (rows[i]["F_TYPE"].ToString().ToUpper() == "C" || rows[i]["F_TYPE"].ToString().ToUpper() == "D"
                        || rows[i]["F_TYPE"].ToString().ToUpper() == "L")
                    {
                        if (intRowarea < 2)
                        {
                            pivotfields[i].Visible = true;
                            //pivotfields[i].SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.Var;
                            pivotfields[i].Area = PivotArea.RowArea;
                        }
                        else
                        {
                            if (intRowarea < 4)
                            {
                                pivotfields[i].Visible = true;
                                pivotfields[i].Area = PivotArea.ColumnArea;
                            }
                            else
                            {
                                pivotfields[i].Visible = false;
                            }
                        }
                        intRowarea++;
                    }
                    else
                    {
                        if (intdataarea < 1)
                        {
                            pivotfields[i].Visible = true;
                            pivotfields[i].Area = PivotArea.DataArea;
                            
                        }
                        else
                        {
                            pivotfields[i].Visible = false; 
                        }
                        intdataarea++;
                    }
                    setPivotFormat(pivotfields[i],rows[i]);
                }
            }
            finally
            {
                pivotGridControl1.EndUpdate(); 
            }
        }

        private string OpenSaveFileDlg(string fileext, string fileFilter)
        {
            saveFileDialog1.DefaultExt = fileext;// ".xlsx";
            saveFileDialog1.Filter = fileFilter;
            string vsRe = "";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                vsRe = saveFileDialog1.FileName;
            return vsRe;
        }

        private void getFieldString(ref string fieldRow, ref string fieldCol, ref string fieldData, ref string fieldSel, ref string fieldFilter)
        {
            fieldRow = ",";
            fieldCol = ",";
            fieldData = ",";
            fieldSel = ",";
            fieldFilter = ",";
            foreach (PivotGridField vsfield in pivotGridControl1.Fields)
            {
                //vsfield.AreaIndex 
                if (vsfield.Visible == false)
                {
                    fieldSel += vsfield.FieldName + ",";
                }
                else
                {
                    switch (vsfield.Area)
                    {
                        case PivotArea.RowArea:
                            fieldRow += vsfield.FieldName + "~" + vsfield.AreaIndex + ",";
                            break;
                        case PivotArea.ColumnArea:
                            fieldCol += vsfield.FieldName + "~" + vsfield.AreaIndex + ",";
                            break;
                        case PivotArea.DataArea:
                            fieldData += vsfield.FieldName + "~" + vsfield.AreaIndex + ",";
                            break;
                        case PivotArea.FilterArea:
                            fieldFilter +=  vsfield.FieldName + "~" + vsfield.AreaIndex + ",";
                            break;
                    }
                }
            }
        }

        private void frmDevPivotShow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.QryHashString == "") return;
            if (!_isSaveVisited) return;
            //保存浏览信息
            string fieldRow = "";
            string fieldCol = "";
            string fieldData = "";
            string fieldSel = "";
            string fieldFilter = "";
            getFieldString(ref fieldRow, ref fieldCol, ref fieldData, ref fieldSel, ref fieldFilter);
            if (fieldRow == "" && fieldCol == "" && fieldData == "") return;
            string sql = "select '1' from JTPUBQRYFAVVISIT where QRYFAVVISIT_QRYID='{0}' and QRYFAVVISIT_USER='{1}' and QRYFAVVISIT_HASH='{2}'";
            sql = string.Format(sql, this.PsID, this.PsUsr, this.QryHashString);
            DataTable dtnum = getDataTable(sql);
            if (dtnum.Rows.Count > 0 && dtnum.Rows[0][0].ToString() == "1")
            {
                sql = "update JTPUBQRYFAVVISIT set QRYFAVVISIT_ROW='{2}',QRYFAVVISIT_COL='{3}',QRYFAVVISIT_DATA='{4}',QRYFAVVISIT_SEL='{5}',QRYFAVVISIT_FILTER='{6}' "
                    + " where QRYFAVVISIT_QRYID='{0}' and QRYFAVVISIT_USER='{1}' and QRYFAVVISIT_HASH='{7}'; ";
            }
            else
            {
                sql = "insert into JTPUBQRYFAVVISIT(QRYFAVVISIT_QRYID,QRYFAVVISIT_USER,QRYFAVVISIT_ROW,QRYFAVVISIT_COL,QRYFAVVISIT_DATA,QRYFAVVISIT_SEL,QRYFAVVISIT_FILTER,QRYFAVVISIT_HASH)"
                   + "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'); ";
            }
            sql = "begin  " + string.Format(sql, this.PsID, this.PsUsr, fieldRow, fieldCol,
                 fieldData, fieldSel, fieldFilter, this.QryHashString) + "   end;";
            try
            {
               // WebSvrGetData.execsqlAysnc(DevQryPubFun.GSYDBSrc, sql, _mgr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(sql+"====1111==="+ex.Message);
            }
            sql = "begin  " + saveFilterValue("%") + "   end;";
            try
            {
                //WebSvrGetData.execsqlAysnc(DevQryPubFun.GSYDBSrc, sql, _mgr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(sql+"====222==="+ex.Message);
            }
        }

        private string saveFilterValue(string psName)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"delete from JTPUBQRYFAVFILTER where QRYFILTER_QRYID='{0}'
and QRYFILTER_USER='{1}' and QRYFILTER_HASH='{2}' and QRYFILTER_NAME='{3}' ;", this.PsID, this.PsUsr, this.QryHashString, psName);
            string sql = @"insert into  JTPUBQRYFAVFILTER(QRYFILTER_QRYID,QRYFILTER_USER,QRYFILTER_HASH,QRYFILTER_NAME,QRYFILTER_FIELD,QRYFILTER_VALUE)
                values('{0}','{1}','{2}','{3}','{4}','{5}');";
            foreach (PivotGridField pivotField in pivotGridControl1.Fields)
            {
                if (pivotField.GetUniqueValues() == null) continue;
                if (pivotField.FilterValues.ValuesExcluded.Length != 0)
                {
                    foreach (object obj in pivotField.FilterValues.ValuesIncluded)
                    {
                        if(!string.IsNullOrEmpty(pivotField.FieldName))
                        sb.AppendFormat(sql, this.PsID, this.PsUsr, this.QryHashString, psName, pivotField.FieldName, obj + "");
                    }
                }

                if (pivotField.FilterValues.ShowBlanks)
                {
                    if (!string.IsNullOrEmpty(pivotField.FieldName))
                    {
                        if (pivotField.FilterValues.ValuesExcluded.Length == 0)
                            sb.AppendFormat(sql, this.PsID, this.PsUsr, this.QryHashString, psName, pivotField.FieldName, "!`ShowBlanks`ALL!");
                        else
                            sb.AppendFormat(sql, this.PsID, this.PsUsr, this.QryHashString, psName, pivotField.FieldName, "!`ShowBlanks`NOTALL!");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(pivotField.FieldName))
                    {
                        if (pivotField.FilterValues.ValuesExcluded.Length == 0)
                            sb.AppendFormat(sql, this.PsID, this.PsUsr, this.QryHashString, psName, pivotField.FieldName, "!`NOTShowBlanks`ALL!");
                        else
                            sb.AppendFormat(sql, this.PsID, this.PsUsr, this.QryHashString, psName, pivotField.FieldName, "!`NOTShowBlanks`NOTALL!");
                    }
                }
            }
            return sb.ToString();
        }

        private void setFilterValue(string psFavName)
        {
            if (dtFavFilter != null && dtFavFilter.Rows.Count > 0)
            {
                foreach (PivotGridField pivotField in pivotGridControl1.Fields)
                {
                    if (pivotField.Visible)
                    {
                        try
                        {
                            pivotField.FilterValues.FilterType = DevExpress.XtraPivotGrid.PivotFilterType.Included;
                            pivotField.FilterValues.Clear();
                            object[] objarr = pivotField.GetUniqueValues();
                            if (objarr == null) continue;
                            DataRow[] rowFilters = dtFavFilter.Select("QRYFILTER_NAME='" + psFavName + "' and QRYFILTER_FIELD='" + pivotField.FieldName + "' ");
                            if (rowFilters.Length > 0)
                            {
                                foreach (DataRow row in rowFilters)
                                {
                                    if (row["QRYFILTER_VALUE"].ToString() == "!`ShowBlanks`ALL!")
                                    {
                                        pivotField.FilterValues.ShowBlanks = true;
                                        foreach (object obj in objarr) pivotField.FilterValues.Add(obj);
                                        break;
                                    }
                                    else if (row["QRYFILTER_VALUE"].ToString() == "!`NOTShowBlanks`ALL!")
                                    {
                                        pivotField.FilterValues.ShowBlanks = false;
                                        foreach (object obj in objarr) pivotField.FilterValues.Add(obj);
                                        break;
                                    }
                                    else if (row["QRYFILTER_VALUE"].ToString() == "!`ShowBlanks`NOTALL!")
                                    {
                                        pivotField.FilterValues.ShowBlanks = true;
                                    }
                                    else if (row["QRYFILTER_VALUE"].ToString() == "!``NOTShowBlanks`NOTALL!")
                                        pivotField.FilterValues.ShowBlanks = false;
                                    else
                                        pivotField.FilterValues.Add(row["QRYFILTER_VALUE"]);
                                }
                            }
                            else
                            {
                                pivotField.FilterValues.ShowBlanks = true;
                                foreach (object obj in objarr)
                                {
                                    pivotField.FilterValues.Add(obj);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(pivotField.FieldName + "    " + ex.Message);
                        }
                    }
                }
            }
        }

        
        private void btnChartBtn_Click(object sender, EventArgs e)
        {
            SimpleButton btn = sender as SimpleButton;

            DevPrint prt = new DevPrint();
            switch (btn.Name)
            {
                case "btnChartView":
                    chartControl1.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Stretch;
                    prt.PrtCtrl = chartControl1;
                    prt.PrintID = "chart" + this.PsID;
                    prt.Preview();
                    break;
                case "btnChartPrint":
                    chartControl1.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Stretch;
                    prt.PrtCtrl = chartControl1;
                    prt.PrintID = "chart" + this.PsID;
                    prt.Print();
                    break;
                case "btnChangeRC":
                    //pivotGridControl1.ChartDataVertical = !pivotGridControl1.ChartDataVertical;
                    break;
            }
        }




        private void cmbChartSaveAs_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filename = "";
            switch (cmbChartSaveAs.Properties.Items[cmbChartSaveAs.SelectedIndex].ToString())
            {
                case ".jpg((&I 图片文件)":
                    filename = OpenSaveFileDlg(".jpg", "jpg图像文件(*.jpg)|*.jpg");
                    if (filename == "") return;
                    this.chartControl1.ExportToImage(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case ".xls(&Excel文件)":
                    filename = OpenSaveFileDlg(".xls", "excel文件(*.xls)|*.xls");
                    if (filename == "") return;
                    this.chartControl1.ExportToXls(filename);
                    break;
                case ".html(&H 网页文件)":
                    filename = OpenSaveFileDlg(".html", "网页文件(*.html)|*.html");
                    if (filename == "") return;
                    this.chartControl1.ExportToHtml(filename);
                    break;
                case ".Pdf(Adobe &Reader)":
                    filename = OpenSaveFileDlg(".pdf", "pdf文件(*.pdf)|*.pdf");
                    if (filename == "") return;
                    this.chartControl1.ExportToPdf(filename);
                    break;
            }

        }

        private void cmbChartType_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*Bar
StackedBar
FullStackedBar
Pie
Point
Line
StepLine
Area
StackedArea
FullStackedArea
SideBySideRangeBar
RangeBar
SideBySideGantt
Gantt
RadarPoint
RadarLine
RadarArea
ManhattanBar
Pie3D
Line3D
StepLine3D
Area3D
StackedArea3D
FullStackedArea3D*/
            switch (cmbChartType.Properties.Items[cmbChartType.SelectedIndex].ToString())
            {
                case "曲线图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.Line);
                    break;
                case "3D曲线图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.Line3D);
                    break;
                case "雷达曲线图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.RadarLine);
                    break;
                case "折线图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.StepLine);
                    break;
                case "3D折线图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.StepLine3D);
                    break;
                case "柱状图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.Bar);
                    break;
                case "3D柱状图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.ManhattanBar);
                    break;
                case "饼图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.Pie);
                    break;
                case "3D饼图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.Pie3D);
                    break;
                case "点图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.Point);
                    break;
                case "雷达点图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.RadarPoint);
                    break;
                case "面积图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.Area);
                    break;
                case "3D面积图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.Area3D);
                    break;
                case "雷达面积图":
                    chartControl1.SeriesTemplate.ChangeView(ViewType.RadarArea);
                    break;

            }
            if (chartControl1.Diagram is DevExpress.XtraCharts.Diagram3D)
            {
                DevExpress.XtraCharts.Diagram3D diagram = (DevExpress.XtraCharts.Diagram3D)chartControl1.Diagram;
                diagram.RuntimeRotation = true;
                diagram.RuntimeZooming = true;
                diagram.RuntimeScrolling = true;
            }
        }


        private void setPivotFormat(DevExpress.XtraPivotGrid.PivotGridField pivotField,DataRow row)
        {
            //if (pivotField.Area == PivotArea.DataArea)
            //{
            if (row["F_PREC"].ToString() == "S")
                pivotField.CellFormat.FormatString = "{0:"+NumFormat + this.PsSLJD + "}";
            else if (row["F_PREC"].ToString() == "J")
                pivotField.CellFormat.FormatString = "{0:" + NumFormat + this.PsJEJD + "}";
            else
            {               
                int intJD=2;
                if (int.TryParse(row["F_PREC"].ToString(), out intJD))
                {
                    pivotField.CellFormat.FormatString = "{0:" + NumFormat + intJD + "}";
                }
                else
                {
                    pivotField.CellFormat.FormatString = "{0:"+NumFormat+"2}";
                } 
            }
            pivotField.CellFormat.FormatType = DevExpress.Utils.FormatType.Custom;
             
            //}
        }

        private void dockField_VisibilityChanged(object sender, DevExpress.XtraBars.Docking.VisibilityChangedEventArgs e)
        {
            if (dockField.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible)//|| dockField.Visibility == DevExpress.XtraBars.Docking.DockVisibility.AutoHide
            {
                pivotGridControl1.FieldsCustomization(dockField);
            }
            else
            {
                 pivotGridControl1.DestroyCustomization();
            }
        }

        private void pivotGridControl1_FieldFilterChanged(object sender, PivotFieldEventArgs e)
        {
           //e.Field
            /*   object[] valuesYear = fieldOrderYear.GetUniqueValues();
            object[] valuesQuarter = fieldOrderQuarter.GetUniqueValues();
            pivotGridControl1.BeginUpdate();
            try {
                foreach(object obj in valuesYear) fieldOrderYear.FilterValues.Add(obj);
                fieldOrderYear.FilterValues.Remove(cbeYear.SelectedItem);
                pivotGridControl1.OptionsView.ShowColumnTotals = cbeQuarter.SelectedIndex == 0;
                if(cbeQuarter.SelectedIndex != 0) {
                    foreach(object obj in valuesQuarter) fieldOrderQuarter.FilterValues.Add(obj);
                    fieldOrderQuarter.FilterValues.Remove(cbeQuarter.SelectedItem);
                }
                else
                    fieldOrderQuarter.FilterValues.Clear();
            }
            finally {
                pivotGridControl1.EndUpdate();
            }*/

        }

        private void pivotGridControl1_CustomCellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
        {
            //值为0 显示空
            decimal dec;
            if (Decimal.TryParse(e.DisplayText, out dec))
            {
                if (dec == 0)
                    e.DisplayText = "";               
                //if (dec < 0)
                //{
                //    if (NumRedShow)
                //    {
                //        e.
                //        e.back.BackColor = Color.Red;
                //    }
                //    else
                //    {
                //        e.Appearance.BackColor = Color.White;
                //    }
                //}
            }
        }

    

        private void repCheckEdt_EditValueChanged(object sender, EventArgs e)
        {
            if ((sender as DevExpress.XtraEditors.CheckEdit).Checked)
                NumFormat = "N";
            else
                NumFormat = "F";
            foreach (PivotGridField pgfield in pivotGridControl1.Fields)
            {
                if (pgfield.Area == PivotArea.DataArea)
                {
                    DataRow[] tmprows = dtLSTIGS.Select("F_FIELD='" + pgfield.FieldName + "'");
                    setPivotFormat(pgfield, tmprows[0]);

                    // setPivotFormat(
                }
            }
            pivotGridControl1.RefreshData();
        }
        private void repCheckNumRed_EditValueChanged(object sender, EventArgs e)
        {
            NumRedShow = (sender as DevExpress.XtraEditors.CheckEdit).Checked;

            pivotGridControl1.RefreshData();
        }
        private void chkTotalCol_EditValueChanged(object sender, EventArgs e)
        {
            if ((sender as DevExpress.XtraEditors.CheckEdit).Checked)
            {
                pivotGridControl1.OptionsView.ShowColumnTotals = true;
                pivotGridControl1.OptionsView.ShowColumnGrandTotals = true;
            }
            else
            {
                pivotGridControl1.OptionsView.ShowColumnTotals = false;
                pivotGridControl1.OptionsView.ShowColumnGrandTotals = false;
            }
            pivotGridControl1.RefreshData();
        }

        private void chkTotalRow_EditValueChanged(object sender, EventArgs e)
        {
            if ((sender as DevExpress.XtraEditors.CheckEdit).Checked)
            {
                pivotGridControl1.OptionsView.ShowRowTotals = true;
                pivotGridControl1.OptionsView.ShowRowGrandTotals = true;
            }
            else
            {
                pivotGridControl1.OptionsView.ShowRowTotals = false;
                pivotGridControl1.OptionsView.ShowRowGrandTotals = false;
            }
            pivotGridControl1.RefreshData();

        }

      
 


        private void chkShowLabel_CheckedChanged(object sender, EventArgs e)
        {
            chartControl1.SeriesTemplate.Label.Visible = chkShowLabel.Checked;
        }

        private void pivotGridControl1_CustomAppearance(object sender, PivotCustomAppearanceEventArgs e)
        {
            decimal dec;
            if (Decimal.TryParse(e.DisplayText, out dec))
            {
                
                if (dec < 0)
                {
                    if (NumRedShow)
                    {
                        e.Appearance.BackColor = Color.Red;
                    }
                    else
                    {
                        e.Appearance.BackColor = Color.White;
                    }
                }
            }
        }

        private void pivotGridControl1_FieldAreaChanged(object sender, PivotFieldEventArgs e)
        {
            try
            {
                if (e.Field.Visible)
                {
                    if (!cmbField.Properties.Items.Contains(e.Field.Caption))
                        cmbField.Properties.Items.Add(e.Field.Caption);
                }
                else
                {
                    if (cmbField.Properties.Items.Contains(e.Field.Caption))
                    {
                        if (e.Field.Caption == cmbField.SelectedText)
                        {
                            e.Field.FilterValues.ShowBlanks = true;
                            foreach (object obj in e.Field.GetUniqueValues()) e.Field.FilterValues.Add(obj);
                            cmbField.SelectedIndex = -1;
                        }
                        cmbField.Properties.Items.Remove(e.Field.Caption);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(e.Field.Caption);
            }

        }

        private void cmbField_SelectedValueChanged(object sender, EventArgs e)
        {
           // cmbField.SelectedText
            DataRow[] rows = dtLSTIGS.Select("F_TSTEXT='"+cmbField.SelectedText+"'");
            if (rows.Length > 0)
            {
                if (rows[0]["F_TYPE"].ToString() == "C")
                {
                    cmbCMP.Properties.Items.Clear();
                    cmbCMP.Properties.Items.Add("包含");
                    cmbCMP.Properties.Items.Add("介于");
                    cmbCMP.Text = "包含";
                    lblVal.Visible = false;
                    tbval1.Visible = false;
                }
                else
                {
                    cmbCMP.Properties.Items.Clear();
                    cmbCMP.Properties.Items.Add("介于");
                    cmbCMP.Text = "介于";
                    lblVal.Visible = true;
                    tbval1.Visible = true;
                }

            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            filterClick(); 
        }


        private void filterClick()
        {
            pivotGridControl1.BeginUpdate();
            if (!chkIncFilter.Checked)
            {
                delFilter();
            }

            DataRow[] rows = dtLSTIGS.Select("F_TSTEXT='" + cmbField.SelectedText + "'");
            string vsFilterInfo = "";
            if (rows.Length > 0)
            {
                PivotGridField pField = pivotGridControl1.Fields.GetFieldByName("F" + rows[0]["F_FIELD"].ToString());
                pField.FilterValues.Clear();
                if (rows[0]["F_TYPE"].ToString() == "C")
                {
                    if (cmbCMP.Text == "包含")
                    {
                        foreach (object obj in pField.GetUniqueValues())
                        {
                            if ((obj as string).IndexOf(tbVal.Text) != -1)
                            {
                                pField.FilterValues.Add(obj);
                            }
                        }
                        vsFilterInfo += cmbField.SelectedText + " 包含：" + tbVal.Text + "；";
                    }
                    if (cmbCMP.Text == "介于")
                    {
                        foreach (object obj in pField.GetUniqueValues())
                        {
                            string vsval = obj as string;

                            if (vsval.CompareTo(tbVal.Text) >= 0 && vsval.CompareTo(tbval1.Text) <= 0)
                            {
                                pField.FilterValues.Add(obj);
                            }
                        }
                        vsFilterInfo += cmbField.SelectedText + " 介于： " + tbVal.Text + " 至 " + tbval1.Text + "；";
                    }
                }
                else if (rows[0]["F_TYPE"].ToString() == "D")
                {
                    bool bldt= true;
                    bool bldt1 = true;
                    DateTime dt = DateTime.Now;
                    DateTime dt1 = DateTime.Now;
                    if (tbVal.Text.Trim() != "")
                    {
                        
                        DateTime.TryParse(tbVal.Text.Trim(), out dt);
                    }
                    else
                    {
                        bldt = false;
                    }
                    if (tbval1.Text.Trim() != "")
                    {
                        DateTime.TryParse(tbval1.Text.Trim(), out dt1);
                    }
                    else
                    {
                        bldt1 = false;
                    }
                    foreach (object obj in pField.GetUniqueValues())
                    {
                        DateTime dtv = DateTime.Now;
                        DateTime.TryParse(obj.ToString(), out dtv);
                        if (bldt && bldt1)
                        {
                            if (dtv >= dt && dtv <= dt1 && !pField.FilterValues.Contains(obj))
                                pField.FilterValues.Add(obj);
                        }
                        else if (bldt && !bldt1)
                        {
                            if (dtv >= dt && !pField.FilterValues.Contains(obj))
                                pField.FilterValues.Add(obj);
                        }
                        if (!bldt && bldt1)
                        {
                            if (dtv <= dt1 && !pField.FilterValues.Contains(obj))
                                pField.FilterValues.Add(obj);
                        }
                    }

                    vsFilterInfo += cmbField.SelectedText + " 介于：" + tbVal.Text + "至" + tbval1.Text + "；";

                }
                else if (rows[0]["F_TYPE"].ToString() == "N")
                {
                    bool bldec = true;
                    bool bldec1 = true;
                    decimal dec = 0;
                    decimal dec1 = 0;
                    if (tbVal.Text.Trim() != "")
                    {
                        Decimal.TryParse(tbVal.Text.Trim(), out dec);
                    }
                    else
                    {
                        bldec = false;
                    }
                    if (tbval1.Text.Trim() != "")
                    {
                        Decimal.TryParse(tbval1.Text.Trim(), out dec1);
                    }
                    else
                    {
                        bldec1 = false;
                    }
                    foreach (object obj in pField.GetUniqueValues())
                    {
                        decimal decval = 0;
                        Decimal.TryParse(obj.ToString(), out decval);
                        if (bldec && bldec1)
                        {
                            if (decval >= dec && decval <= dec1 && !pField.FilterValues.Contains(obj))
                                pField.FilterValues.Add(obj);
                        }
                        else if (bldec && !bldec1)
                        {
                            if (decval >= dec && !pField.FilterValues.Contains(obj))
                                pField.FilterValues.Add(obj);
                        }
                        if (!bldec && bldec1)
                        {
                            if (decval <= dec1 && !pField.FilterValues.Contains(obj))
                                pField.FilterValues.Add(obj);
                        }
                    }

                    vsFilterInfo += cmbField.SelectedText + " 介于：" + tbVal.Text + "至" + tbval1.Text + "；";
                }
                baritemfilterinfo.Caption += vsFilterInfo;
            }
            pivotGridControl1.EndUpdate();
        }

        private void cmbCMP_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmpValCtrlVisible(cmbCMP.Text);        
        }
        private void cmpValCtrlVisible( string cmpText )
        {
            if (cmpText == "介于")
            {
                lblVal.Visible = true;
                tbval1.Visible = true;
            }
            else
            {
                lblVal.Visible = false;
                tbval1.Visible = false;
            }
        }

        private void setConditionFilter(string psFavName){
            if (psFavName.Trim() == "") return;
            DataRow[] rowsFav = dtFav.Select("QRYFAV_NAME='" + psFavName + "' ");
            if (rowsFav.Length > 0)
            {
                string filterval = rowsFav[0]["QRYFAV_FILTERVAL"].ToString();
                if (filterval.Trim() != "")
                {
                    //cmbField.SelectedText^ cmbCMP.Text^ tbVal.Text^ tbval1.Text
                    string[] valarr = filterval.Split('^');
                    cmbField.SelectedText = valarr[0];
                    cmbCMP.Text = valarr[1];
                    tbVal.Text = valarr[2];
                    tbval1.Text = valarr[3];
                    cmpValCtrlVisible(valarr[1]);                     
                    filterClick();
                }
            }
                       
        }


        
     
    }

}