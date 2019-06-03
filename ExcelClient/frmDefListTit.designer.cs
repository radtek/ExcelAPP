namespace ExcelClient
{
    partial class frmDefListTit
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
            this.rpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.controlContainer1 = new DevExpress.XtraBars.Docking.ControlContainer();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnTest = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.btnDel = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.F_TIBH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rpEdt = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.F_JS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.F_FIELD = new DevExpress.XtraGrid.Columns.GridColumn();
            this.F_TEXT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.F_TSTEXT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.F_TYPE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rpType = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.F_ALIGN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rpAlign = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.rpPSUMType = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.rpFMTType = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.F_WIDTH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.F_PREC = new DevExpress.XtraGrid.Columns.GridColumn();
            this.F_HJBZ = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rpYesOrNo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.F_YHBZ = new DevExpress.XtraGrid.Columns.GridColumn();
            this.F_GROUP = new DevExpress.XtraGrid.Columns.GridColumn();
            this.F_TSHID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.F_ISGD = new DevExpress.XtraGrid.Columns.GridColumn();
            this.F_PSUMTYPE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.F_FORMAT = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.rpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpEdt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpAlign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpPSUMType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpFMTType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpYesOrNo)).BeginInit();
            this.SuspendLayout();
            // 
            // rpEdit
            // 
            this.rpEdit.AutoHeight = false;
            this.rpEdit.Name = "rpEdit";
            // 
            // controlContainer1
            // 
            this.controlContainer1.Location = new System.Drawing.Point(2, 24);
            this.controlContainer1.Name = "controlContainer1";
            this.controlContainer1.Size = new System.Drawing.Size(322, 234);
            this.controlContainer1.TabIndex = 0;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnTest);
            this.panelControl1.Controls.Add(this.simpleButton1);
            this.panelControl1.Controls.Add(this.btnDel);
            this.panelControl1.Controls.Add(this.btnAdd);
            this.panelControl1.Controls.Add(this.btnSave);
            this.panelControl1.Controls.Add(this.btnClose);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 522);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1057, 55);
            this.panelControl1.TabIndex = 27;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(646, 14);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(87, 27);
            this.btnTest.TabIndex = 8;
            this.btnTest.Text = "测试";
            this.btnTest.ToolTip = "需保存后才有效";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(526, 14);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(87, 27);
            this.simpleButton1.TabIndex = 7;
            this.simpleButton1.Text = "删除行";
            this.simpleButton1.ToolTip = "保存后才有效";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(405, 14);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(87, 27);
            this.btnDel.TabIndex = 6;
            this.btnDel.Text = "删除";
            this.btnDel.ToolTip = "直接删除，重新加载字段信息";
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(170, 14);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(87, 27);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "增加";
            this.btnAdd.ToolTip = "需保存后才有效";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(282, 14);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 27);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(755, 14);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(87, 27);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "关闭";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.gridControl1);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1057, 522);
            this.panelControl2.TabIndex = 29;
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(2, 2);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.rpType,
            this.rpAlign,
            this.rpPSUMType,
            this.rpFMTType,
            this.rpYesOrNo,
            this.rpEdt});
            this.gridControl1.Size = new System.Drawing.Size(1053, 518);
            this.gridControl1.TabIndex = 5;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.F_TIBH,
            this.F_JS,
            this.F_FIELD,
            this.F_TEXT,
            this.F_TSTEXT,
            this.F_TYPE,
            this.F_ALIGN,
            this.F_WIDTH,
            this.F_PREC,
            this.F_HJBZ,
            this.F_YHBZ,
            this.F_GROUP,
            this.F_TSHID,
            this.F_ISGD,
            this.F_PSUMTYPE,
            this.F_FORMAT});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // F_TIBH
            // 
            this.F_TIBH.AppearanceHeader.Options.UseTextOptions = true;
            this.F_TIBH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_TIBH.Caption = "编号（分级码）";
            this.F_TIBH.ColumnEdit = this.rpEdt;
            this.F_TIBH.FieldName = "F_TIBH";
            this.F_TIBH.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.F_TIBH.Name = "F_TIBH";
            this.F_TIBH.Visible = true;
            this.F_TIBH.VisibleIndex = 0;
            // 
            // rpEdt
            // 
            this.rpEdt.AutoHeight = false;
            this.rpEdt.Name = "rpEdt";
            // 
            // F_JS
            // 
            this.F_JS.AppearanceHeader.Options.UseTextOptions = true;
            this.F_JS.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_JS.Caption = "级数";
            this.F_JS.ColumnEdit = this.rpEdt;
            this.F_JS.FieldName = "F_JS";
            this.F_JS.Name = "F_JS";
            this.F_JS.Visible = true;
            this.F_JS.VisibleIndex = 1;
            // 
            // F_FIELD
            // 
            this.F_FIELD.AppearanceHeader.Options.UseTextOptions = true;
            this.F_FIELD.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_FIELD.Caption = "字段名";
            this.F_FIELD.ColumnEdit = this.rpEdt;
            this.F_FIELD.FieldName = "F_FIELD";
            this.F_FIELD.Name = "F_FIELD";
            this.F_FIELD.Visible = true;
            this.F_FIELD.VisibleIndex = 2;
            // 
            // F_TEXT
            // 
            this.F_TEXT.AppearanceHeader.Options.UseTextOptions = true;
            this.F_TEXT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_TEXT.Caption = "显示名";
            this.F_TEXT.ColumnEdit = this.rpEdt;
            this.F_TEXT.FieldName = "F_TEXT";
            this.F_TEXT.Name = "F_TEXT";
            this.F_TEXT.Visible = true;
            this.F_TEXT.VisibleIndex = 4;
            // 
            // F_TSTEXT
            // 
            this.F_TSTEXT.Caption = "透视显示名";
            this.F_TSTEXT.ColumnEdit = this.rpEdt;
            this.F_TSTEXT.FieldName = "F_TSTEXT";
            this.F_TSTEXT.Name = "F_TSTEXT";
            this.F_TSTEXT.Visible = true;
            this.F_TSTEXT.VisibleIndex = 3;
            // 
            // F_TYPE
            // 
            this.F_TYPE.AppearanceHeader.Options.UseTextOptions = true;
            this.F_TYPE.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_TYPE.Caption = "字符类型";
            this.F_TYPE.ColumnEdit = this.rpType;
            this.F_TYPE.FieldName = "F_TYPE";
            this.F_TYPE.Name = "F_TYPE";
            this.F_TYPE.Visible = true;
            this.F_TYPE.VisibleIndex = 5;
            // 
            // rpType
            // 
            this.rpType.AutoHeight = false;
            this.rpType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.rpType.Items.AddRange(new object[] {
            "C",
            "N"});
            this.rpType.Name = "rpType";
            // 
            // F_ALIGN
            // 
            this.F_ALIGN.AppearanceHeader.Options.UseTextOptions = true;
            this.F_ALIGN.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_ALIGN.Caption = "对齐方式";
            this.F_ALIGN.ColumnEdit = this.rpAlign;
            this.F_ALIGN.FieldName = "F_ALIGN";
            this.F_ALIGN.Name = "F_ALIGN";
            this.F_ALIGN.Visible = true;
            this.F_ALIGN.VisibleIndex = 6;
            // 
            // rpAlign
            // 
            this.rpAlign.AutoHeight = false;
            this.rpAlign.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.rpAlign.Items.AddRange(new object[] {
            "L",
            "M",
            "R"});
            this.rpAlign.Name = "rpAlign";
            // 
            // F_WIDTH
            // 
            this.F_WIDTH.AppearanceHeader.Options.UseTextOptions = true;
            this.F_WIDTH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_WIDTH.Caption = "宽度";
            this.F_WIDTH.ColumnEdit = this.rpEdt;
            this.F_WIDTH.FieldName = "F_WIDTH";
            this.F_WIDTH.Name = "F_WIDTH";
            this.F_WIDTH.ToolTip = "数字具体宽度";
            this.F_WIDTH.Visible = true;
            this.F_WIDTH.VisibleIndex = 7;
            // 
            // F_PREC
            // 
            this.F_PREC.AppearanceHeader.Options.UseTextOptions = true;
            this.F_PREC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_PREC.Caption = "精度";
            this.F_PREC.ColumnEdit = this.rpEdt;
            this.F_PREC.FieldName = "F_PREC";
            this.F_PREC.Name = "F_PREC";
            this.F_PREC.ToolTip = "S J 或具体的数字（S 数量精度 J 金额精度)";
            this.F_PREC.Visible = true;
            this.F_PREC.VisibleIndex = 8;
            // 
            // F_HJBZ
            // 
            this.F_HJBZ.AppearanceHeader.Options.UseTextOptions = true;
            this.F_HJBZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_HJBZ.Caption = "是否合计";
            this.F_HJBZ.ColumnEdit = this.rpYesOrNo;
            this.F_HJBZ.FieldName = "F_HJBZ";
            this.F_HJBZ.Name = "F_HJBZ";
            this.F_HJBZ.ToolTip = "1 是 0 否";
            this.F_HJBZ.Visible = true;
            this.F_HJBZ.VisibleIndex = 9;
            // 
            // rpYesOrNo
            // 
            this.rpYesOrNo.AutoHeight = false;
            this.rpYesOrNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.rpYesOrNo.Items.AddRange(new object[] {
            "0",
            "1"});
            this.rpYesOrNo.Name = "rpYesOrNo";
            // 
            // F_YHBZ
            // 
            this.F_YHBZ.AppearanceHeader.Options.UseTextOptions = true;
            this.F_YHBZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_YHBZ.Caption = "隐藏标志";
            this.F_YHBZ.ColumnEdit = this.rpYesOrNo;
            this.F_YHBZ.FieldName = "F_YHBZ";
            this.F_YHBZ.Name = "F_YHBZ";
            this.F_YHBZ.ToolTip = "1 是 0 否";
            this.F_YHBZ.Visible = true;
            this.F_YHBZ.VisibleIndex = 10;
            // 
            // F_GROUP
            // 
            this.F_GROUP.AppearanceHeader.Options.UseTextOptions = true;
            this.F_GROUP.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_GROUP.Caption = "分组字段";
            this.F_GROUP.ColumnEdit = this.rpYesOrNo;
            this.F_GROUP.FieldName = "F_GROUP";
            this.F_GROUP.Name = "F_GROUP";
            this.F_GROUP.ToolTip = "1 是 0 否";
            this.F_GROUP.Visible = true;
            this.F_GROUP.VisibleIndex = 11;
            // 
            // F_TSHID
            // 
            this.F_TSHID.AppearanceHeader.Options.UseTextOptions = true;
            this.F_TSHID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_TSHID.Caption = "透视隐藏";
            this.F_TSHID.ColumnEdit = this.rpYesOrNo;
            this.F_TSHID.FieldName = "F_TSHID";
            this.F_TSHID.Name = "F_TSHID";
            this.F_TSHID.ToolTip = "1 是 0 否";
            this.F_TSHID.Visible = true;
            this.F_TSHID.VisibleIndex = 12;
            // 
            // F_ISGD
            // 
            this.F_ISGD.Caption = "固定表列";
            this.F_ISGD.ColumnEdit = this.rpYesOrNo;
            this.F_ISGD.FieldName = "F_ISGD";
            this.F_ISGD.Name = "F_ISGD";
            this.F_ISGD.ToolTip = "1 是 0 否";
            this.F_ISGD.Visible = true;
            this.F_ISGD.VisibleIndex = 13;
            // 
            // F_PSUMTYPE
            // 
            this.F_PSUMTYPE.AppearanceHeader.Options.UseTextOptions = true;
            this.F_PSUMTYPE.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_PSUMTYPE.Caption = "Pivot合计方式";
            this.F_PSUMTYPE.ColumnEdit = this.rpPSUMType;
            this.F_PSUMTYPE.FieldName = "F_PSUMTYPE";
            this.F_PSUMTYPE.Name = "F_PSUMTYPE";
            this.F_PSUMTYPE.Visible = true;
            this.F_PSUMTYPE.VisibleIndex = 14;
            // 
            // rpPSUMType
            // 
            this.rpPSUMType.AutoHeight = false;
            this.rpPSUMType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.rpPSUMType.Items.AddRange(new object[] {
            "SUM",
            "AVERAGE",
            "MAX",
            "MIN",
            "COUNT",
            "VAR",
            "VARP",
            "STDDEV",
            "STDDEVP"});
            this.rpPSUMType.Name = "rpPSUMType";
            // 
            // F_FORMAT
            // 
            this.F_FORMAT.AppearanceHeader.Options.UseTextOptions = true;
            this.F_FORMAT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.F_FORMAT.Caption = "格式方式";
            this.F_FORMAT.ColumnEdit = this.rpFMTType;
            this.F_FORMAT.FieldName = "F_FORMAT";
            this.F_FORMAT.Name = "F_FORMAT";
            this.F_FORMAT.Visible = true;
            this.F_FORMAT.VisibleIndex = 15;
            // 
            // rpFMTType
            // 
            this.rpFMTType.AutoHeight = false;
            this.rpFMTType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.rpFMTType.Items.AddRange(new object[] {
                " ",
            "BASE64"});
            this.rpFMTType.Name = "rpFMTType";
            // 
            // frmDefListTit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1057, 577);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.Name = "frmDefListTit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "查询表头定义";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmDefListTit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpEdt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpAlign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpPSUMType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpFMTType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpYesOrNo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion


        private DevExpress.XtraBars.Docking.ControlContainer controlContainer1;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit rpEdit;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton btnDel;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn F_TIBH;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit rpEdt;
        private DevExpress.XtraGrid.Columns.GridColumn F_JS;
        private DevExpress.XtraGrid.Columns.GridColumn F_FIELD;
        private DevExpress.XtraGrid.Columns.GridColumn F_TEXT;
        private DevExpress.XtraGrid.Columns.GridColumn F_TYPE;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox rpType;
        private DevExpress.XtraGrid.Columns.GridColumn F_ALIGN;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox rpAlign;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox rpPSUMType;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox rpFMTType;
        private DevExpress.XtraGrid.Columns.GridColumn F_WIDTH;
        private DevExpress.XtraGrid.Columns.GridColumn F_PREC;
        private DevExpress.XtraGrid.Columns.GridColumn F_HJBZ;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox rpYesOrNo;
        private DevExpress.XtraGrid.Columns.GridColumn F_YHBZ;
        private DevExpress.XtraEditors.SimpleButton btnTest;
        private DevExpress.XtraGrid.Columns.GridColumn F_TSTEXT;
        private DevExpress.XtraGrid.Columns.GridColumn F_GROUP;
        private DevExpress.XtraGrid.Columns.GridColumn F_TSHID;
        private DevExpress.XtraGrid.Columns.GridColumn F_ISGD;
        private DevExpress.XtraGrid.Columns.GridColumn F_PSUMTYPE;
        private DevExpress.XtraGrid.Columns.GridColumn F_FORMAT;
 
    }
}