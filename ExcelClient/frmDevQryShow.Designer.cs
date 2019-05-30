namespace ExcelClient
{
    partial class frmDevQryShow
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
            this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            this.tabctrl = new DevExpress.XtraTab.XtraTabControl();
            this.tabpagegrid = new DevExpress.XtraTab.XtraTabPage();
            this.tabpagePivot = new DevExpress.XtraTab.XtraTabPage();
            ((System.ComponentModel.ISupportInitialize)(this.tabctrl)).BeginInit();
            this.tabctrl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabctrl
            // 
            this.tabctrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabctrl.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Bottom;
            this.tabctrl.Location = new System.Drawing.Point(0, 0);
            this.tabctrl.Name = "tabctrl";
            this.tabctrl.SelectedTabPage = this.tabpagegrid;
            this.tabctrl.Size = new System.Drawing.Size(1370, 706);
            this.tabctrl.TabIndex = 0;
            this.tabctrl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabpagegrid,
            this.tabpagePivot});
            this.tabctrl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabctrl_SelectedPageChanged);
            // 
            // tabpagegrid
            // 
            this.tabpagegrid.Name = "tabpagegrid";
            this.tabpagegrid.Size = new System.Drawing.Size(1364, 679);
            this.tabpagegrid.Text = "表格展示";
            // 
            // tabpagePivot
            // 
            this.tabpagePivot.Name = "tabpagePivot";
            this.tabpagePivot.Size = new System.Drawing.Size(1369, 679);
            this.tabpagePivot.Text = "透视分析";
            // 
            // frmDevQryShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 706);
            this.Controls.Add(this.tabctrl);
            this.Name = "frmDevQryShow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDevQryShow_FormClosing);
            this.Load += new System.EventHandler(this.frmDevQryShow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tabctrl)).EndInit();
            this.tabctrl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public DevExpress.XtraTab.XtraTabControl tabctrl;
        public DevExpress.XtraTab.XtraTabPage tabpagegrid;
        public DevExpress.XtraTab.XtraTabPage tabpagePivot;

        protected DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
    }
}