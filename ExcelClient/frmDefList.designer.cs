namespace ExcelClient
{
    partial class frmDefList
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
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.controlContainer1 = new DevExpress.XtraBars.Docking.ControlContainer();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tbMC = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.btnQry = new DevExpress.XtraEditors.SimpleButton();
            this.tbBH = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.btnmenu = new DevExpress.XtraEditors.SimpleButton();
            this.btnOut = new DevExpress.XtraEditors.SimpleButton();
            this.btnTit = new DevExpress.XtraEditors.SimpleButton();
            this.btnDel = new DevExpress.XtraEditors.SimpleButton();
            this.btnadd = new DevExpress.XtraEditors.SimpleButton();
            this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colbh = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colmc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTItle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSubTil = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDWField = new DevExpress.XtraGrid.Columns.GridColumn();
            this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbMC.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBH.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
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
            this.panelControl1.Controls.Add(this.tbMC);
            this.panelControl1.Controls.Add(this.labelControl3);
            this.panelControl1.Controls.Add(this.btnQry);
            this.panelControl1.Controls.Add(this.tbBH);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1176, 40);
            this.panelControl1.TabIndex = 5;
            // 
            // tbMC
            // 
            this.tbMC.EditValue = "";
            this.tbMC.Location = new System.Drawing.Point(322, 9);
            this.tbMC.Name = "tbMC";
            this.tbMC.Size = new System.Drawing.Size(175, 20);
            this.tbMC.TabIndex = 1;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(279, 12);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(36, 14);
            this.labelControl3.TabIndex = 8;
            this.labelControl3.Text = "名称：";
            // 
            // btnQry
            // 
            this.btnQry.Location = new System.Drawing.Point(540, 7);
            this.btnQry.Name = "btnQry";
            this.btnQry.Size = new System.Drawing.Size(87, 27);
            this.btnQry.TabIndex = 2;
            this.btnQry.Text = "查询";
            this.btnQry.Click += new System.EventHandler(this.btnQry_Click);
            // 
            // tbBH
            // 
            this.tbBH.EditValue = "";
            this.tbBH.Location = new System.Drawing.Point(77, 9);
            this.tbBH.Name = "tbBH";
            this.tbBH.Size = new System.Drawing.Size(175, 20);
            this.tbBH.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(33, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(36, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "编号：";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.btnmenu);
            this.panelControl2.Controls.Add(this.btnOut);
            this.panelControl2.Controls.Add(this.btnTit);
            this.panelControl2.Controls.Add(this.btnDel);
            this.panelControl2.Controls.Add(this.btnadd);
            this.panelControl2.Controls.Add(this.btnEdit);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(0, 684);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1176, 65);
            this.panelControl2.TabIndex = 6;
            // 
            // btnmenu
            // 
            this.btnmenu.Location = new System.Drawing.Point(909, 24);
            this.btnmenu.Name = "btnmenu";
            this.btnmenu.Size = new System.Drawing.Size(91, 27);
            this.btnmenu.TabIndex = 8;
            this.btnmenu.Text = "菜单定义";
            this.btnmenu.Click += new System.EventHandler(this.btnmenu_Click);
            // 
            // btnOut
            // 
            this.btnOut.Location = new System.Drawing.Point(778, 24);
            this.btnOut.Name = "btnOut";
            this.btnOut.Size = new System.Drawing.Size(87, 27);
            this.btnOut.TabIndex = 7;
            this.btnOut.Text = "导出";
            this.btnOut.Click += new System.EventHandler(this.btnOut_Click);
            // 
            // btnTit
            // 
            this.btnTit.Location = new System.Drawing.Point(635, 24);
            this.btnTit.Name = "btnTit";
            this.btnTit.Size = new System.Drawing.Size(85, 27);
            this.btnTit.TabIndex = 6;
            this.btnTit.Text = "格式定义";
            this.btnTit.Click += new System.EventHandler(this.btnTit_Click);
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(500, 24);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(87, 27);
            this.btnDel.TabIndex = 5;
            this.btnDel.Text = "删除";
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnadd
            // 
            this.btnadd.Location = new System.Drawing.Point(248, 24);
            this.btnadd.Name = "btnadd";
            this.btnadd.Size = new System.Drawing.Size(87, 27);
            this.btnadd.TabIndex = 3;
            this.btnadd.Text = "增加";
            this.btnadd.Click += new System.EventHandler(this.btnadd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(370, 24);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(87, 27);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "修改";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.gridControl1);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl3.Location = new System.Drawing.Point(0, 40);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(1176, 644);
            this.panelControl3.TabIndex = 7;
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(2, 2);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1172, 640);
            this.gridControl1.TabIndex = 5;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colbh,
            this.colmc,
            this.colTItle,
            this.colSubTil,
            this.colDWField});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // colbh
            // 
            this.colbh.Caption = "编号";
            this.colbh.FieldName = "JTPUBQRDEF_BH";
            this.colbh.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.colbh.Name = "colbh";
            this.colbh.Visible = true;
            this.colbh.VisibleIndex = 0;
            // 
            // colmc
            // 
            this.colmc.Caption = "名称";
            this.colmc.FieldName = "JTPUBQRDEF_MC";
            this.colmc.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.colmc.Name = "colmc";
            this.colmc.Visible = true;
            this.colmc.VisibleIndex = 1;
            // 
            // colTItle
            // 
            this.colTItle.Caption = "标题";
            this.colTItle.FieldName = "JTPUBQRDEF_TITLE";
            this.colTItle.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.colTItle.Name = "colTItle";
            this.colTItle.Visible = true;
            this.colTItle.VisibleIndex = 2;
            // 
            // colSubTil
            // 
            this.colSubTil.Caption = "副标题";
            this.colSubTil.FieldName = "JTPUBQRDEF_SUBTIL";
            this.colSubTil.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.colSubTil.Name = "colSubTil";
            this.colSubTil.Visible = true;
            this.colSubTil.VisibleIndex = 3;
            // 
            // colDWField
            // 
            this.colDWField.Caption = "字段(参数)";
            this.colDWField.FieldName = "JTPUBQRDEF_DWFIELD";
            this.colDWField.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.colDWField.Name = "colDWField";
            this.colDWField.Visible = true;
            this.colDWField.VisibleIndex = 4;
            // 
            // frmDefList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1176, 749);
            this.Controls.Add(this.panelControl3);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.Name = "frmDefList";
            this.Text = "查询定义列表";
            this.Load += new System.EventHandler(this.frmDevBBShow_Load);
            this.Shown += new System.EventHandler(this.frmDevBBShow_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbMC.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBH.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion


        private DevExpress.XtraBars.Docking.ControlContainer controlContainer1;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnQry;
        private DevExpress.XtraEditors.TextEdit tbBH;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton btnadd;
        private DevExpress.XtraEditors.SimpleButton btnEdit;
        private DevExpress.XtraEditors.SimpleButton btnTit;
        private DevExpress.XtraEditors.SimpleButton btnDel;
        private DevExpress.XtraEditors.TextEdit tbMC;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnOut;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colbh;
        private DevExpress.XtraGrid.Columns.GridColumn colmc;
        private DevExpress.XtraGrid.Columns.GridColumn colTItle;
        private DevExpress.XtraGrid.Columns.GridColumn colSubTil;
        private DevExpress.XtraGrid.Columns.GridColumn colDWField;
        protected DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
        private DevExpress.XtraEditors.SimpleButton btnmenu;
 
    }
}