using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts;
using DevExpress.LookAndFeel;

namespace ExcelClient
{
    public partial class frmDevChartShow : DevExpress.XtraEditors.XtraForm
    {
        public frmDevChartShow()
        {
            InitializeComponent();
        }

        private string title;
        public string Title
        {
            set { this.title = value; }
            get { return this.title; }
        }
        //private DataTable dtLSZBGS;
        //public DataTable DtLSZBGS
        //{
        //    set { this.dtLSZBGS = value; }
        //    get { return this.dtLSZBGS; }
        //}
        //private DataTable dtLSOTGS;
        //public DataTable DtLSOTGS
        //{
        //    set { this.dtLSOTGS = value; }
        //    get { return this.dtLSOTGS; }
        //}
        private DataTable dtLSTIGS;
        public DataTable DtLSTIGS
        {
            set { this.dtLSTIGS = value; }
            get { return this.dtLSTIGS; }
        }

        private DataSet dsData;
        public DataSet DsData
        {
            set { this.dsData = value; }
            get { return this.dsData; }
        }
        private string _psID;
        public string PsID {
            set { this._psID = value; }
            get { return this._psID; }
        }

        private void frmDevChartShow_Load(object sender, EventArgs e)
        {
            this.Text = "图形分析--" + this.Title;
            //设置图标样式
            setChartType();  
            setXY();
             
        }

        private void frmDevChartShow_Shown(object sender, EventArgs e)
        {
           //设置图标样式
            bindChart();
            //DevExpress.XtraCharts.rad
            //DevExpress.XtraCharts.Series series = new DevExpress.XtraCharts.Series();
            //DevExpress.XtraCharts.LineSeriesView line = new DevExpress.XtraCharts.LineSeriesView();
            //this.comboBoxEditSeriesDataMember.Properties.Items.Add(this.dS11.GSP.YearColumn.ColumnName);
            //this.comboBoxEditSeriesDataMember.Properties.Items.Add(this.dS11.GSP.RegionColumn.ColumnName);
            //this.comboBoxEditSeriesDataMember.SelectedIndex = 0;
            //((XYDiagram)this.chartControl.Diagram).AxisY.GridLines.MinorVisible = true;
            //((XYDiagram)this.chartControl.Diagram).AxisY.Title.Text = "Millions of Dollars";
            //((XYDiagram)this.chartControl.Diagram).AxisY.Title.Visible = true;
            //((XYDiagram)this.chartControl.Diagram).AxisX.Label.Antialiasing = true;
            //chartControl1.Series

            //UserLookAndFeel.Default.SetSkinStyle("Office 2013");
        }

     



        #region  图标样式设置
        private void setChartType()
        {
            DevExpress.XtraEditors.Controls.RadioGroupItem item = new DevExpress.XtraEditors.Controls.RadioGroupItem();
            item.Description = "曲线图";
            item.Value = "line";
            radioCharType2D.Properties.Items.Add(item);
            radioCharType3D.Properties.Items.Add(item);
            radioCharTypeRadar.Properties.Items.Add(item);
            item = new DevExpress.XtraEditors.Controls.RadioGroupItem();
            item.Description = "折线图";
            item.Value = "lineStep";
            radioCharType2D.Properties.Items.Add(item);
            radioCharType3D.Properties.Items.Add(item);
            item = new DevExpress.XtraEditors.Controls.RadioGroupItem();
            item.Description = "柱状图";
            item.Value = "bar";
            radioCharType2D.Properties.Items.Add(item);
            radioCharType3D.Properties.Items.Add(item);
            item = new DevExpress.XtraEditors.Controls.RadioGroupItem();
            item.Description = "饼图";
            item.Value = "pie";
            radioCharType2D.Properties.Items.Add(item);
            radioCharType3D.Properties.Items.Add(item);
            item = new DevExpress.XtraEditors.Controls.RadioGroupItem();
            item.Description = "点图";
            item.Value = "point";
            radioCharType2D.Properties.Items.Add(item);            
            radioCharTypeRadar.Properties.Items.Add(item);
            item = new DevExpress.XtraEditors.Controls.RadioGroupItem();
            item.Description = "面积图";
            item.Value = "area";
            radioCharType2D.Properties.Items.Add(item);
            radioCharType3D.Properties.Items.Add(item);
            radioCharTypeRadar.Properties.Items.Add(item);
            radioCharType2D.SelectedIndex = 0;
        }

        private void setXY()
        {
            DataRow[] rows = dtLSTIGS.Select("F_FIELD<>'' and F_YHBZ<>'1'", "F_TIBH");
            foreach (DataRow row in rows)
            {
                if (row["F_TYPE"].ToString() == "C" || row["F_TYPE"].ToString() == "D")
                {
                    DevExpress.XtraEditors.Controls.RadioGroupItem item = new DevExpress.XtraEditors.Controls.RadioGroupItem();
                    item.Description = row["F_TEXT"].ToString();
                    item.Value = row["F_FIELD"].ToString();
                    radioX.Properties.Items.Add(item);
                }
                else
                {
                    //checkY.Items.Add(
                    DevExpress.XtraEditors.Controls.CheckedListBoxItem item = new DevExpress.XtraEditors.Controls.CheckedListBoxItem();
                    item.Value = row["F_TEXT"].ToString();
                    //item.Value = row["F_FIELD"].ToString();
                    checkY.Items.Add(item);
                }
            }
            if (radioX.Properties.Items.Count > 0) radioX.SelectedIndex = 0;
            if (checkY.Items.Count > 0) checkY.Items[0].CheckState=CheckState.Checked;
        }

        private void bindChart()
        {
            try
            {
                string charType = "";
                chartControl1.Series.Clear();
                //DevExpress.XtraCharts.Series series = new DevExpress.XtraCharts.Series();
                DevExpress.XtraCharts.Series[] serarr = new DevExpress.XtraCharts.Series[checkY.CheckedItems.Count];
                for (int i = 0; i < serarr.Length; i++)// (Series series in serarr)
                {
                    DevExpress.XtraCharts.Series series = serarr[i];
                    series = new DevExpress.XtraCharts.Series();
                    series.DataSource = dsData.Tables[0].DefaultView;
                    chartControl1.Series.Add(series);
                    //series.ValueDataMembers.AddRange();
                    #region 2d
                    if (radioCharType2D.SelectedIndex != -1)
                    {
                        charType = radioCharType2D.Properties.Items[radioCharType2D.SelectedIndex].Value.ToString();
                        switch (charType)
                        {
                            case "line":
                                series.View = new DevExpress.XtraCharts.LineSeriesView();
                                break;
                            case "lineStep":
                                series.View = new DevExpress.XtraCharts.StepLineSeriesView();
                                break;
                            case "bar":
                                //chartControl1.SeriesTemplate.View = new DevExpress.XtraCharts.BarSeriesView();
                                break;
                            case "pie":
                                series.View = new DevExpress.XtraCharts.PieSeriesView();
                                break;
                            case "point":
                                series.View = new DevExpress.XtraCharts.PointSeriesView();
                                break;
                            case "area":
                                series.View = new DevExpress.XtraCharts.AreaSeriesView();
                                break;
                        }
                    }
                    #endregion

                    if (radioCharType3D.SelectedIndex != -1)
                    {
                        charType = radioCharType3D.Properties.Items[radioCharType3D.SelectedIndex].Value.ToString();
                        switch (charType)
                        {
                            case "line":
                                series.View = new DevExpress.XtraCharts.Line3DSeriesView();
                                break;
                            case "lineStep":
                                series.View = new DevExpress.XtraCharts.StepLine3DSeriesView();
                                break;
                            case "bar":
                                series.View = new DevExpress.XtraCharts.ManhattanBarSeriesView();
                                break;
                            case "pie":
                                series.View = new DevExpress.XtraCharts.Pie3DSeriesView();
                                break;
                            case "area":
                                series.View = new DevExpress.XtraCharts.Area3DSeriesView();
                                break;
                        }
                    }
                    if (radioCharTypeRadar.SelectedIndex != -1)
                    {
                        charType = radioCharTypeRadar.Properties.Items[radioCharTypeRadar.SelectedIndex].Value.ToString();
                        switch (charType)
                        {
                            case "line":
                                series.View = new DevExpress.XtraCharts.RadarLineSeriesView();
                                break;
                            case "point":
                                series.View = new DevExpress.XtraCharts.RadarPointSeriesView();
                                break;
                            case "area":
                                series.View = new DevExpress.XtraCharts.RadarAreaSeriesView();
                                break;
                        }
                    }
                    if (radioX.SelectedIndex != -1)
                    {
                        if (charType != "pie")
                        {
                            if (radioCharType3D.SelectedIndex != -1)
                            {
                                ((XYDiagram3D)this.chartControl1.Diagram).AxisX.Label.Antialiasing = true;
                                //  ((XYDiagram3D)this.chartControl1.Diagram).AxisX.Title.Text = radioX.Properties.Items[radioX.SelectedIndex].Description;
                            }
                            else
                                if (radioCharType2D.SelectedIndex != -1)
                                {
                                    ((XYDiagram)this.chartControl1.Diagram).AxisX.Label.Antialiasing = true;
                                    ((XYDiagram)this.chartControl1.Diagram).AxisX.Title.Text = radioX.Properties.Items[radioX.SelectedIndex].Description;
                                }
                                else
                                    if (radioCharTypeRadar.SelectedIndex != -1)
                                    {
                                        ((RadarDiagram)this.chartControl1.Diagram).AxisX.Label.Antialiasing = true;
                                    }
                        }
                        else
                        {
                            series.PointOptions.PointView = PointView.ArgumentAndValues;
                      
                            series.Label.Text = radioX.Properties.Items[radioX.SelectedIndex].Description;
                            //(this.chartControl1.Series[0].Label as PieSeriesLabel).Text = radioX.Properties.Items[radioX.SelectedIndex].Description;
                           //  ((Pie3DSeriesLabel)this.chartControl1.Diagram).AxisX.Title.Text = radioX.Properties.Items[radioX.SelectedIndex].Description;
                        }
                        series.Label.Visible = true;
                        series.ArgumentDataMember = radioX.Properties.Items[radioX.SelectedIndex].Value.ToString();
                    }
                    string vstext = checkY.CheckedItems[i].ToString();
                    series.LegendText = vstext;
                    series.ValueDataMembersSerializable = getValueFromData(dtLSTIGS, "F_FIELD", "F_Text='" + vstext + "'");

                }
            }
            catch (Exception ex)
            {
                
            }
            
            
            
            //if (checkY.SelectedIndex != -1)
            //{
            //    ((XYDiagram)this.chartControl1.Diagram).AxisY.GridLines.MinorVisible = true;
            //    ((XYDiagram)this.chartControl1.Diagram).AxisY.Title.Text = checkY.SelectedValue.ToString();
            //    ((XYDiagram)this.chartControl1.Diagram).AxisY.Title.Visible = true;
            //    this.chartControl1.SeriesTemplate.ArgumentDataMember = radioX.Properties.Items[radioX.SelectedIndex].Value.ToString();   

            //}
            /* ((XYDiagram)this.chartControl1.Diagram).AxisY.GridLines.MinorVisible = true;
                ((XYDiagram)this.chartControl1.Diagram).AxisY.Title.Text = "Millions of Dollars";
                ((XYDiagram)this.chartControl1.Diagram).AxisY.Title.Visible = true;*/
             
            

        }
        private string getValueFromData(DataTable dt, string fieldName, string sql)
        {
            DataRow[] rows = dt.Select(sql);
            if (rows.Length > 0)
            {
                return rows[0][fieldName].ToString();
            }
            return "";
        }
        #endregion

        

        private void radioX_Properties_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindChart();
        }

        private void radioCharType2D_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((RadioGroup)sender).Name == "radioCharType2D")
            {
                radioCharType3D.SelectedIndex = -1;
                radioCharTypeRadar.SelectedIndex = -1;
            }
            else
                if (((RadioGroup)sender).Name == "radioCharType3D")
                {
                    radioCharType2D.SelectedIndex = -1;
                    radioCharTypeRadar.SelectedIndex = -1;
                }
                else
                {
                    radioCharType2D.SelectedIndex = -1;
                    radioCharType3D.SelectedIndex = -1;
                }
            bindChart();
        }

        private void radioCharType2D_Enter(object sender, EventArgs e)
        {
            ((RadioGroup)sender).SelectedIndex = 0;
        }

        private void barsaveasjpg_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string filename = OpenSaveFileDlg(".jpg", "jpg图像文件(*.jpg)|*.jpg");
            if (filename == "") return;
           this.chartControl1.ExportToImage(filename,System.Drawing.Imaging.ImageFormat.Jpeg);
        }



        private string OpenSaveFileDlg(string fileext,string fileFilter)
        {
            saveFileDialog1.DefaultExt = fileext;// ".xlsx";
            saveFileDialog1.Filter = fileFilter;
            string vsRe = "";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                vsRe = saveFileDialog1.FileName;
            //DialogResult.
            return vsRe;
        }

        private void barsaveasxls_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string filename = OpenSaveFileDlg(".xls", "excel文件(*.xls)|*.xls");
            if (filename == "") return;
            this.chartControl1.ExportToXls(filename);
        }

        private void barsaveashtml_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string filename = OpenSaveFileDlg(".html", "网页文件(*.html)|*.html");
            if (filename == "") return;
            this.chartControl1.ExportToHtml(filename);
        }

        private void barsaveaspdf_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string filename = OpenSaveFileDlg(".pdf", "pdf文件(*.pdf)|*.pdf");
            if (filename == "") return;
            this.chartControl1.ExportToPdf(filename);
        }

        private void barView_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevPrint prt = new DevPrint();
            chartControl1.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Stretch;
            prt.PrtCtrl = chartControl1;
            prt.PrintID = "chart" + this.PsID;
            prt.Preview();
        }

        private void barPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevPrint prt = new DevPrint();
            chartControl1.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Stretch;
            prt.PrtCtrl = chartControl1;
            prt.PrintID = "chart"+this.PsID;
            prt.Print();
        }
    }
}