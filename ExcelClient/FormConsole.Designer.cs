namespace ExcelClient
{
    partial class FormConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConsole));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.bsiSys = new DevExpress.XtraBars.BarSubItem();
            this.biClose = new DevExpress.XtraBars.BarButtonItem();
            this.biCloseAll = new DevExpress.XtraBars.BarButtonItem();
            this.biCloseAllExcept = new DevExpress.XtraBars.BarButtonItem();
            this.bsiSkin = new DevExpress.XtraBars.BarSubItem();
            this.pnlTop = new DevExpress.XtraEditors.PanelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.buttonEditDW = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.pnlMain = new DevExpress.XtraEditors.PanelControl();
            this.nbMain = new DevExpress.XtraNavBar.NavBarControl();
            this.nbgBusiness = new DevExpress.XtraNavBar.NavBarGroup();
            this.nbiSale = new DevExpress.XtraNavBar.NavBarItem();
            this.nbiBuy = new DevExpress.XtraNavBar.NavBarItem();
            this.nbiCheck = new DevExpress.XtraNavBar.NavBarItem();
            this.nbiVIP = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarItem1 = new DevExpress.XtraNavBar.NavBarItem();
            this.ngbCash = new DevExpress.XtraNavBar.NavBarGroup();
            this.nbgAnalysis = new DevExpress.XtraNavBar.NavBarGroup();
            this.nbgBase = new DevExpress.XtraNavBar.NavBarGroup();
            this.nbiGoods = new DevExpress.XtraNavBar.NavBarItem();
            this.nbiCustomer = new DevExpress.XtraNavBar.NavBarItem();
            this.nbiSupplier = new DevExpress.XtraNavBar.NavBarItem();
            this.nbiEmployee = new DevExpress.XtraNavBar.NavBarItem();
            this.nbgConfig = new DevExpress.XtraNavBar.NavBarGroup();
            this.nbiConfigure = new DevExpress.XtraNavBar.NavBarItem();
            this.nbiHardware = new DevExpress.XtraNavBar.NavBarItem();
            this.nbiRBAC = new DevExpress.XtraNavBar.NavBarItem();
            this.nbiDatabase = new DevExpress.XtraNavBar.NavBarItem();
            this.nbiLog = new DevExpress.XtraNavBar.NavBarItem();
            this.treeList1 = new DevExpress.XtraTreeList.TreeList();
            this.NAME = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.mdiManager1 = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(this.components);
            this.pmTabbed = new DevExpress.XtraBars.PopupMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlTop)).BeginInit();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.buttonEditDW.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nbMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mdiManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pmTabbed)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar3});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bsiSys,
            this.biClose,
            this.biCloseAll,
            this.biCloseAllExcept,
            this.bsiSkin});
            this.barManager1.MaxItemId = 5;
            this.barManager1.StatusBar = this.bar3;
            this.barManager1.Merge += new DevExpress.XtraBars.BarManagerMergeEventHandler(this.barManager1_Merge);
            this.barManager1.UnMerge += new DevExpress.XtraBars.BarManagerMergeEventHandler(this.barManager1_UnMerge);
            // 
            // bar3
            // 
            this.bar3.BarName = "Status bar";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            this.bar3.Text = "状态栏";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1189, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 550);
            this.barDockControlBottom.Size = new System.Drawing.Size(1189, 23);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 550);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1189, 0);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 550);
            // 
            // bsiSys
            // 
            this.bsiSys.Caption = "系统(&S)";
            this.bsiSys.Id = 0;
            this.bsiSys.Name = "bsiSys";
            // 
            // biClose
            // 
            this.biClose.Caption = "关闭";
            this.biClose.Id = 1;
            this.biClose.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4));
            this.biClose.Name = "biClose";
            this.biClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.biClose_ItemClick);
            // 
            // biCloseAll
            // 
            this.biCloseAll.Caption = "关闭所有文档(&L)";
            this.biCloseAll.Id = 2;
            this.biCloseAll.Name = "biCloseAll";
            this.biCloseAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.biCloseAll_ItemClick);
            // 
            // biCloseAllExcept
            // 
            this.biCloseAllExcept.Caption = "除此之外全部关闭(&A)";
            this.biCloseAllExcept.Id = 3;
            this.biCloseAllExcept.Name = "biCloseAllExcept";
            this.biCloseAllExcept.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.biCloseAllExcept_ItemClick);
            // 
            // bsiSkin
            // 
            this.bsiSkin.Caption = "样式(&V)";
            this.bsiSkin.Id = 4;
            this.bsiSkin.Name = "bsiSkin";
            // 
            // pnlTop
            // 
            this.pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlTop.Controls.Add(this.panelControl1);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1189, 35);
            this.pnlTop.TabIndex = 4;
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.buttonEditDW);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControl1.Location = new System.Drawing.Point(835, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(354, 35);
            this.panelControl1.TabIndex = 3;
            // 
            // buttonEditDW
            // 
            this.buttonEditDW.Location = new System.Drawing.Point(59, 8);
            this.buttonEditDW.MenuManager = this.barManager1;
            this.buttonEditDW.Name = "buttonEditDW";
            this.buttonEditDW.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.buttonEditDW.Size = new System.Drawing.Size(283, 20);
            this.buttonEditDW.TabIndex = 1;
            this.buttonEditDW.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEditDW_ButtonClick);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(5, 10);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(48, 14);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "选择单位";
            // 
            // pnlMain
            // 
            this.pnlMain.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlMain.Controls.Add(this.nbMain);
            this.pnlMain.Controls.Add(this.treeList1);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMain.Location = new System.Drawing.Point(0, 35);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(8, 6, 12, 4);
            this.pnlMain.Size = new System.Drawing.Size(269, 515);
            this.pnlMain.TabIndex = 5;
            // 
            // nbMain
            // 
            this.nbMain.ActiveGroup = this.nbgBusiness;
            this.nbMain.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.nbgBusiness,
            this.ngbCash,
            this.nbgAnalysis,
            this.nbgBase,
            this.nbgConfig});
            this.nbMain.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] {
            this.nbiGoods,
            this.nbiCustomer,
            this.nbiSupplier,
            this.nbiEmployee,
            this.nbiSale,
            this.nbiBuy,
            this.nbiCheck,
            this.nbiVIP,
            this.nbiConfigure,
            this.nbiHardware,
            this.nbiRBAC,
            this.nbiDatabase,
            this.nbiLog,
            this.navBarItem1});
            this.nbMain.LinkSelectionMode = DevExpress.XtraNavBar.LinkSelectionModeType.OneInControl;
            this.nbMain.Location = new System.Drawing.Point(84, 332);
            this.nbMain.Name = "nbMain";
            this.nbMain.OptionsNavPane.ExpandedWidth = 54;
            this.nbMain.OptionsNavPane.ShowOverflowButton = false;
            this.nbMain.OptionsNavPane.ShowOverflowPanel = false;
            this.nbMain.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.NavigationPane;
            this.nbMain.Size = new System.Drawing.Size(54, 66);
            this.nbMain.TabIndex = 0;
            this.nbMain.Text = "功能模块";
            this.nbMain.Visible = false;
            this.nbMain.Resize += new System.EventHandler(this.nbMain_Resize);
            // 
            // nbgBusiness
            // 
            this.nbgBusiness.Caption = "日常业务";
            this.nbgBusiness.Expanded = true;
            this.nbgBusiness.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.LargeIconsText;
            this.nbgBusiness.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiSale),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiBuy),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiCheck),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiVIP),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarItem1)});
            this.nbgBusiness.Name = "nbgBusiness";
            this.nbgBusiness.SelectedLinkIndex = 4;
            this.nbgBusiness.TopVisibleLinkIndex = 1;
            // 
            // nbiSale
            // 
            this.nbiSale.Caption = "销售出库";
            this.nbiSale.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiSale.LargeImage")));
            this.nbiSale.Name = "nbiSale";
            // 
            // nbiBuy
            // 
            this.nbiBuy.Caption = "进货入库";
            this.nbiBuy.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiBuy.LargeImage")));
            this.nbiBuy.Name = "nbiBuy";
            // 
            // nbiCheck
            // 
            this.nbiCheck.Caption = "库存盘点";
            this.nbiCheck.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiCheck.LargeImage")));
            this.nbiCheck.Name = "nbiCheck";
            // 
            // nbiVIP
            // 
            this.nbiVIP.Caption = "会员管理";
            this.nbiVIP.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiVIP.LargeImage")));
            this.nbiVIP.Name = "nbiVIP";
            // 
            // navBarItem1
            // 
            this.navBarItem1.Caption = "navBarItem1";
            this.navBarItem1.Name = "navBarItem1";
            // 
            // ngbCash
            // 
            this.ngbCash.Caption = "现金收支";
            this.ngbCash.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.LargeIconsText;
            this.ngbCash.Name = "ngbCash";
            // 
            // nbgAnalysis
            // 
            this.nbgAnalysis.Caption = "经营分析";
            this.nbgAnalysis.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.LargeIconsText;
            this.nbgAnalysis.Name = "nbgAnalysis";
            // 
            // nbgBase
            // 
            this.nbgBase.Caption = "基础资料";
            this.nbgBase.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.LargeIconsText;
            this.nbgBase.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiGoods),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiCustomer),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiSupplier),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiEmployee)});
            this.nbgBase.Name = "nbgBase";
            this.nbgBase.TopVisibleLinkIndex = 1;
            // 
            // nbiGoods
            // 
            this.nbiGoods.Caption = "商品资料";
            this.nbiGoods.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiGoods.LargeImage")));
            this.nbiGoods.Name = "nbiGoods";
            this.nbiGoods.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nbiGoods_LinkClicked);
            // 
            // nbiCustomer
            // 
            this.nbiCustomer.Caption = "客户管理";
            this.nbiCustomer.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiCustomer.LargeImage")));
            this.nbiCustomer.Name = "nbiCustomer";
            this.nbiCustomer.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nbiCustomer_LinkClicked);
            // 
            // nbiSupplier
            // 
            this.nbiSupplier.Caption = "供应商管理";
            this.nbiSupplier.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiSupplier.LargeImage")));
            this.nbiSupplier.Name = "nbiSupplier";
            this.nbiSupplier.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nbiSupplier_LinkClicked);
            // 
            // nbiEmployee
            // 
            this.nbiEmployee.Caption = "员工";
            this.nbiEmployee.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiEmployee.LargeImage")));
            this.nbiEmployee.Name = "nbiEmployee";
            // 
            // nbgConfig
            // 
            this.nbgConfig.Caption = "系统设置";
            this.nbgConfig.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.LargeIconsText;
            this.nbgConfig.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiConfigure),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiHardware),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiRBAC),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiDatabase),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiLog)});
            this.nbgConfig.Name = "nbgConfig";
            this.nbgConfig.TopVisibleLinkIndex = 2;
            // 
            // nbiConfigure
            // 
            this.nbiConfigure.Caption = "参数设置";
            this.nbiConfigure.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiConfigure.LargeImage")));
            this.nbiConfigure.Name = "nbiConfigure";
            // 
            // nbiHardware
            // 
            this.nbiHardware.Caption = "硬件设置";
            this.nbiHardware.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiHardware.LargeImage")));
            this.nbiHardware.Name = "nbiHardware";
            // 
            // nbiRBAC
            // 
            this.nbiRBAC.Caption = "用户与权限";
            this.nbiRBAC.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiRBAC.LargeImage")));
            this.nbiRBAC.Name = "nbiRBAC";
            // 
            // nbiDatabase
            // 
            this.nbiDatabase.Caption = "数据维护";
            this.nbiDatabase.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiDatabase.LargeImage")));
            this.nbiDatabase.Name = "nbiDatabase";
            // 
            // nbiLog
            // 
            this.nbiLog.Caption = "操作日志";
            this.nbiLog.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiLog.LargeImage")));
            this.nbiLog.Name = "nbiLog";
            // 
            // treeList1
            // 
            this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.NAME});
            this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList1.Location = new System.Drawing.Point(8, 6);
            this.treeList1.Name = "treeList1";
            this.treeList1.OptionsView.ShowVertLines = false;
            this.treeList1.SelectImageList = this.imageList1;
            this.treeList1.Size = new System.Drawing.Size(249, 505);
            this.treeList1.TabIndex = 0;
            this.treeList1.DoubleClick += new System.EventHandler(this.treeList1_DoubleClick);
            // 
            // NAME
            // 
            this.NAME.Caption = "菜单名称";
            this.NAME.FieldName = "NAME";
            this.NAME.MinWidth = 33;
            this.NAME.Name = "NAME";
            this.NAME.OptionsColumn.AllowEdit = false;
            this.NAME.Visible = true;
            this.NAME.VisibleIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "png.png");
            this.imageList1.Images.SetKeyName(1, "file.gif");
            // 
            // mdiManager1
            // 
            this.mdiManager1.MdiParent = this;
            this.mdiManager1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mdiManager1_MouseUp);
            // 
            // pmTabbed
            // 
            this.pmTabbed.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.biClose),
            new DevExpress.XtraBars.LinkPersistInfo(this.biCloseAll),
            new DevExpress.XtraBars.LinkPersistInfo(this.biCloseAllExcept)});
            this.pmTabbed.Manager = this.barManager1;
            this.pmTabbed.Name = "pmTabbed";
            // 
            // FormConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1189, 573);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.IsMdiContainer = true;
            this.Name = "FormConsole";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmConsole";
            this.Load += new System.EventHandler(this.frmConsole_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlTop)).EndInit();
            this.pnlTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.buttonEditDW.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nbMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mdiManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pmTabbed)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarSubItem bsiSys;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.PanelControl pnlTop;
        private DevExpress.XtraEditors.PanelControl pnlMain;
        private DevExpress.XtraNavBar.NavBarControl nbMain;
        private DevExpress.XtraNavBar.NavBarGroup nbgBusiness;
        private DevExpress.XtraNavBar.NavBarGroup ngbCash;
        private DevExpress.XtraNavBar.NavBarGroup nbgAnalysis;
        private DevExpress.XtraNavBar.NavBarGroup nbgBase;
        private DevExpress.XtraNavBar.NavBarGroup nbgConfig;
        private DevExpress.XtraTabbedMdi.XtraTabbedMdiManager mdiManager1;
        private DevExpress.XtraNavBar.NavBarItem nbiGoods;
        private DevExpress.XtraNavBar.NavBarItem nbiCustomer;
        private DevExpress.XtraNavBar.NavBarItem nbiSupplier;
        private DevExpress.XtraNavBar.NavBarItem nbiEmployee;
        private DevExpress.XtraNavBar.NavBarItem nbiSale;
        private DevExpress.XtraNavBar.NavBarItem nbiBuy;
        private DevExpress.XtraNavBar.NavBarItem nbiCheck;
        private DevExpress.XtraNavBar.NavBarItem nbiVIP;
        private DevExpress.XtraBars.BarButtonItem biClose;
        private DevExpress.XtraBars.BarButtonItem biCloseAll;
        private DevExpress.XtraBars.BarButtonItem biCloseAllExcept;
        private DevExpress.XtraBars.PopupMenu pmTabbed;
        private DevExpress.XtraBars.BarSubItem bsiSkin;
        private DevExpress.XtraNavBar.NavBarItem nbiConfigure;
        private DevExpress.XtraNavBar.NavBarItem nbiHardware;
        private DevExpress.XtraNavBar.NavBarItem nbiRBAC;
        private DevExpress.XtraNavBar.NavBarItem nbiDatabase;
        private DevExpress.XtraNavBar.NavBarItem nbiLog;
        private DevExpress.XtraNavBar.NavBarItem navBarItem1;
        private DevExpress.XtraEditors.ButtonEdit buttonEditDW;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraTreeList.TreeList treeList1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn NAME;
        private System.Windows.Forms.ImageList imageList1;
    }
}