namespace ExcelClient
{
    partial class frmDevChartShow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraCharts.XYDiagram xyDiagram1 = new DevExpress.XtraCharts.XYDiagram();
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel1 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
            DevExpress.XtraCharts.Series series2 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel2 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
            DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel3 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
            this.chartControl1 = new DevExpress.XtraCharts.ChartControl();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockXYSet = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel2_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.checkY = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.radioX = new DevExpress.XtraEditors.RadioGroup();
            this.dockSet = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.radioCharTypeRadar = new DevExpress.XtraEditors.RadioGroup();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.radioCharType3D = new DevExpress.XtraEditors.RadioGroup();
            this.radioCharType2D = new DevExpress.XtraEditors.RadioGroup();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.barChartMenu = new DevExpress.XtraBars.Bar();
            this.barView = new DevExpress.XtraBars.BarButtonItem();
            this.barPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barsaveas = new DevExpress.XtraBars.BarSubItem();
            this.barsaveasjpg = new DevExpress.XtraBars.BarButtonItem();
            this.barsaveasxls = new DevExpress.XtraBars.BarButtonItem();
            this.barsaveashtml = new DevExpress.XtraBars.BarButtonItem();
            this.barsaveaspdf = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockXYSet.SuspendLayout();
            this.dockPanel2_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioX.Properties)).BeginInit();
            this.dockSet.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioCharTypeRadar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioCharType3D.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioCharType2D.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // chartControl1
            // 
            xyDiagram1.AxisX.Range.ScrollingRange.SideMarginsEnabled = true;
            xyDiagram1.AxisX.Range.SideMarginsEnabled = true;
            xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
            xyDiagram1.AxisY.Range.ScrollingRange.SideMarginsEnabled = true;
            xyDiagram1.AxisY.Range.SideMarginsEnabled = true;
            xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
            this.chartControl1.Diagram = xyDiagram1;
            this.chartControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl1.Location = new System.Drawing.Point(200, 31);
            this.chartControl1.Margin = new System.Windows.Forms.Padding(4);
            this.chartControl1.Name = "chartControl1";
            sideBySideBarSeriesLabel1.LineVisible = true;
            series1.Label = sideBySideBarSeriesLabel1;
            series1.Name = "Series 1";
            sideBySideBarSeriesLabel2.LineVisible = true;
            series2.Label = sideBySideBarSeriesLabel2;
            series2.Name = "Series 2";
            this.chartControl1.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1,
        series2};
            sideBySideBarSeriesLabel3.LineVisible = true;
            this.chartControl1.SeriesTemplate.Label = sideBySideBarSeriesLabel3;
            this.chartControl1.Size = new System.Drawing.Size(595, 620);
            this.chartControl1.TabIndex = 0;
            // 
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockXYSet,
            this.dockSet});
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
            // 
            // dockXYSet
            // 
            this.dockXYSet.Controls.Add(this.dockPanel2_Container);
            this.dockXYSet.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dockXYSet.FloatSize = new System.Drawing.Size(223, 466);
            this.dockXYSet.ID = new System.Guid("d26a0636-7ae3-4116-a5eb-2e83ed493043");
            this.dockXYSet.Location = new System.Drawing.Point(795, 31);
            this.dockXYSet.Margin = new System.Windows.Forms.Padding(4);
            this.dockXYSet.Name = "dockXYSet";
            this.dockXYSet.Options.AllowDockBottom = false;
            this.dockXYSet.Options.AllowDockTop = false;
            this.dockXYSet.Options.ShowCloseButton = false;
            this.dockXYSet.OriginalSize = new System.Drawing.Size(200, 200);
            this.dockXYSet.Size = new System.Drawing.Size(200, 620);
            this.dockXYSet.TabsPosition = DevExpress.XtraBars.Docking.TabsPosition.Right;
            this.dockXYSet.Text = "轴设置";
            // 
            // dockPanel2_Container
            // 
            this.dockPanel2_Container.Controls.Add(this.labelControl2);
            this.dockPanel2_Container.Controls.Add(this.labelControl1);
            this.dockPanel2_Container.Controls.Add(this.checkY);
            this.dockPanel2_Container.Controls.Add(this.radioX);
            this.dockPanel2_Container.Location = new System.Drawing.Point(4, 23);
            this.dockPanel2_Container.Margin = new System.Windows.Forms.Padding(4);
            this.dockPanel2_Container.Name = "dockPanel2_Container";
            this.dockPanel2_Container.Size = new System.Drawing.Size(192, 593);
            this.dockPanel2_Container.TabIndex = 0;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(4, 231);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(20, 14);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Y轴";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(4, 0);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(19, 14);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "X轴";
            // 
            // checkY
            // 
            this.checkY.Location = new System.Drawing.Point(4, 254);
            this.checkY.Margin = new System.Windows.Forms.Padding(4);
            this.checkY.Name = "checkY";
            this.checkY.Size = new System.Drawing.Size(246, 250);
            this.checkY.TabIndex = 1;
            this.checkY.SelectedIndexChanged += new System.EventHandler(this.radioX_Properties_SelectedIndexChanged);
            // 
            // radioX
            // 
            this.radioX.Location = new System.Drawing.Point(4, 21);
            this.radioX.Margin = new System.Windows.Forms.Padding(4);
            this.radioX.Name = "radioX";
            this.radioX.Properties.SelectedIndexChanged += new System.EventHandler(this.radioX_Properties_SelectedIndexChanged);
            this.radioX.Size = new System.Drawing.Size(246, 203);
            this.radioX.TabIndex = 0;
            // 
            // dockSet
            // 
            this.dockSet.Controls.Add(this.dockPanel1_Container);
            this.dockSet.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.dockSet.FloatSize = new System.Drawing.Size(200, 561);
            this.dockSet.ID = new System.Guid("ace6f7e9-2b16-4fa6-9afa-b24c0c993bc8");
            this.dockSet.Location = new System.Drawing.Point(0, 31);
            this.dockSet.Margin = new System.Windows.Forms.Padding(4);
            this.dockSet.Name = "dockSet";
            this.dockSet.Options.AllowDockBottom = false;
            this.dockSet.Options.AllowDockTop = false;
            this.dockSet.Options.ShowCloseButton = false;
            this.dockSet.OriginalSize = new System.Drawing.Size(200, 200);
            this.dockSet.Size = new System.Drawing.Size(200, 620);
            this.dockSet.TabsPosition = DevExpress.XtraBars.Docking.TabsPosition.Left;
            this.dockSet.Text = "图表设置";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.labelControl5);
            this.dockPanel1_Container.Controls.Add(this.labelControl4);
            this.dockPanel1_Container.Controls.Add(this.radioCharTypeRadar);
            this.dockPanel1_Container.Controls.Add(this.labelControl3);
            this.dockPanel1_Container.Controls.Add(this.radioCharType3D);
            this.dockPanel1_Container.Controls.Add(this.radioCharType2D);
            this.dockPanel1_Container.Location = new System.Drawing.Point(4, 23);
            this.dockPanel1_Container.Margin = new System.Windows.Forms.Padding(4);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(192, 593);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(10, 394);
            this.labelControl5.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(36, 14);
            this.labelControl5.TabIndex = 7;
            this.labelControl5.Text = "雷达图";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(14, 195);
            this.labelControl4.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(15, 14);
            this.labelControl4.TabIndex = 5;
            this.labelControl4.Text = "3D";
            // 
            // radioCharTypeRadar
            // 
            this.radioCharTypeRadar.Location = new System.Drawing.Point(4, 415);
            this.radioCharTypeRadar.Margin = new System.Windows.Forms.Padding(4);
            this.radioCharTypeRadar.Name = "radioCharTypeRadar";
            this.radioCharTypeRadar.Size = new System.Drawing.Size(220, 192);
            this.radioCharTypeRadar.TabIndex = 6;
            this.radioCharTypeRadar.SelectedIndexChanged += new System.EventHandler(this.radioCharType2D_SelectedIndexChanged);
            this.radioCharTypeRadar.Enter += new System.EventHandler(this.radioCharType2D_Enter);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(10, 0);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(15, 14);
            this.labelControl3.TabIndex = 3;
            this.labelControl3.Text = "2D";
            // 
            // radioCharType3D
            // 
            this.radioCharType3D.Location = new System.Drawing.Point(4, 216);
            this.radioCharType3D.Margin = new System.Windows.Forms.Padding(4);
            this.radioCharType3D.Name = "radioCharType3D";
            this.radioCharType3D.Size = new System.Drawing.Size(220, 174);
            this.radioCharType3D.TabIndex = 4;
            this.radioCharType3D.SelectedIndexChanged += new System.EventHandler(this.radioCharType2D_SelectedIndexChanged);
            this.radioCharType3D.Enter += new System.EventHandler(this.radioCharType2D_Enter);
            // 
            // radioCharType2D
            // 
            this.radioCharType2D.Location = new System.Drawing.Point(4, 21);
            this.radioCharType2D.Margin = new System.Windows.Forms.Padding(4);
            this.radioCharType2D.Name = "radioCharType2D";
            this.radioCharType2D.Size = new System.Drawing.Size(220, 170);
            this.radioCharType2D.TabIndex = 0;
            this.radioCharType2D.SelectedIndexChanged += new System.EventHandler(this.radioCharType2D_SelectedIndexChanged);
            this.radioCharType2D.Enter += new System.EventHandler(this.radioCharType2D_Enter);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barChartMenu});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.DockManager = this.dockManager1;
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barView,
            this.barPrint,
            this.barButtonItem1,
            this.barsaveas,
            this.barButtonItem2,
            this.barsaveasjpg,
            this.barsaveasxls,
            this.barsaveashtml,
            this.barsaveaspdf});
            this.barManager1.MaxItemId = 9;
            // 
            // barChartMenu
            // 
            this.barChartMenu.BarName = "Custom 3";
            this.barChartMenu.DockCol = 0;
            this.barChartMenu.DockRow = 0;
            this.barChartMenu.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barChartMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barView),
            new DevExpress.XtraBars.LinkPersistInfo(this.barPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.barsaveas),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2)});
            this.barChartMenu.Text = "图形分析菜单";
            // 
            // barView
            // 
            this.barView.Caption = "预览(&V)";
            this.barView.Id = 0;
            this.barView.Name = "barView";
            this.barView.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barView_ItemClick);
            // 
            // barPrint
            // 
            this.barPrint.Caption = "打印(&P)";
            this.barPrint.Id = 1;
            this.barPrint.Name = "barPrint";
            this.barPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barPrint_ItemClick);
            // 
            // barsaveas
            // 
            this.barsaveas.Caption = "另存为(&A)";
            this.barsaveas.Id = 3;
            this.barsaveas.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barsaveasjpg),
            new DevExpress.XtraBars.LinkPersistInfo(this.barsaveasxls),
            new DevExpress.XtraBars.LinkPersistInfo(this.barsaveashtml),
            new DevExpress.XtraBars.LinkPersistInfo(this.barsaveaspdf)});
            this.barsaveas.Name = "barsaveas";
            // 
            // barsaveasjpg
            // 
            this.barsaveasjpg.Caption = ".jpg((&I 图片文件)";
            this.barsaveasjpg.Id = 5;
            this.barsaveasjpg.Name = "barsaveasjpg";
            this.barsaveasjpg.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barsaveasjpg_ItemClick);
            // 
            // barsaveasxls
            // 
            this.barsaveasxls.Caption = ".xls(&Excel文件)";
            this.barsaveasxls.Id = 6;
            this.barsaveasxls.Name = "barsaveasxls";
            this.barsaveasxls.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barsaveasxls_ItemClick);
            // 
            // barsaveashtml
            // 
            this.barsaveashtml.Caption = ".html(&H 网页文件)";
            this.barsaveashtml.Id = 7;
            this.barsaveashtml.Name = "barsaveashtml";
            this.barsaveashtml.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barsaveashtml_ItemClick);
            // 
            // barsaveaspdf
            // 
            this.barsaveaspdf.Caption = ".Pdf(Adobe &Reader)";
            this.barsaveaspdf.Id = 8;
            this.barsaveaspdf.Name = "barsaveaspdf";
            this.barsaveaspdf.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barsaveaspdf_ItemClick);
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Id = 4;
            this.barButtonItem2.Name = "barButtonItem2";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(4);
            this.barDockControlTop.Size = new System.Drawing.Size(995, 31);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 651);
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(4);
            this.barDockControlBottom.Size = new System.Drawing.Size(995, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 31);
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 620);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(995, 31);
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 620);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "另存为";
            this.barButtonItem1.Id = 2;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // frmDevChartShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(995, 651);
            this.Controls.Add(this.chartControl1);
            this.Controls.Add(this.dockXYSet);
            this.Controls.Add(this.dockSet);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmDevChartShow";
            this.Text = "frmDevChartShow";
            this.Load += new System.EventHandler(this.frmDevChartShow_Load);
            this.Shown += new System.EventHandler(this.frmDevChartShow_Shown);
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockXYSet.ResumeLayout(false);
            this.dockPanel2_Container.ResumeLayout(false);
            this.dockPanel2_Container.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioX.Properties)).EndInit();
            this.dockSet.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            this.dockPanel1_Container.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioCharTypeRadar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioCharType3D.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioCharType2D.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraCharts.ChartControl chartControl1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockSet;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraBars.Docking.DockPanel dockXYSet;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel2_Container;
        private DevExpress.XtraEditors.RadioGroup radioCharType2D;
        private DevExpress.XtraEditors.CheckedListBoxControl checkY;
        private DevExpress.XtraEditors.RadioGroup radioX;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.RadioGroup radioCharType3D;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.RadioGroup radioCharTypeRadar;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar barChartMenu;
        private DevExpress.XtraBars.BarButtonItem barView;
        private DevExpress.XtraBars.BarButtonItem barPrint;
        private DevExpress.XtraBars.BarSubItem barsaveas;
        private DevExpress.XtraBars.BarButtonItem barsaveasjpg;
        private DevExpress.XtraBars.BarButtonItem barsaveasxls;
        private DevExpress.XtraBars.BarButtonItem barsaveashtml;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private DevExpress.XtraBars.BarButtonItem barsaveaspdf;
    }
}