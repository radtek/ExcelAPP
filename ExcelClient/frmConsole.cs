using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraBars;
using DevExpress.XtraTabbedMdi;
using DevExpress.XtraEditors;

namespace ExcelClient
{
    public partial class frmConsole : DevExpress.XtraEditors.XtraForm
    {
        private DevExpress.XtraNavBar.NavBarGroup nbgBase1;

        public frmConsole()
        {
            InitializeComponent();


            pnlTop.SuspendLayout();
            /* --最后Add的优先级最高-- */
            // pnlTop.Controls.Add(banner);
            //pnlTop.Controls.Add(pnlLogo);
            /* --此部分需注意，以免乱-- */
            pnlLogo.Dock = DockStyle.Left;
            // banner.Dock = DockStyle.Fill;
            pnlTop.ResumeLayout(false);

            UserLookAndFeel.Default.StyleChanged += Default_StyleChanged;
            pnlLogo.DoubleClick += pnlLogo_DoubleClick;
            this.nbgBase1 = new DevExpress.XtraNavBar.NavBarGroup();
            this.nbgBase1.Caption = "基础资料1";
            this.nbgBase1.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.LargeIconsText;
            this.nbgBase1.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiGoods),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiCustomer),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiSupplier),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbiEmployee)});
            this.nbgBase1.Name = "nbgBase1";
            this.nbgBase1.TopVisibleLinkIndex = 1;
            this.nbMain.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.nbgBase1 }
            );
        }

        void pnlLogo_DoubleClick(object sender, EventArgs e)
        {

        }

        void Default_StyleChanged(object sender, EventArgs e)
        {
            string skinName = UserLookAndFeel.Default.SkinName;
            bool border = skinName.StartsWith("Sharp", StringComparison.OrdinalIgnoreCase);
            if (border)
            {
                pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            }
            else
            {
                pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            }
             
            pnlLogo.BackColor = pnlTop.BackColor;
            pnlTop.Refresh();
        }

        private void frmConsole_Load(object sender, EventArgs e)
        {
            ShowOrActiveForm(typeof(Form1));
            SplashScreenManager.CloseForm(false);
            this.WindowState = FormWindowState.Maximized;
        }

        private void nbiGoods_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            #region WaitForm Demo...
            //SplashScreenManager.ShowForm(this, typeof(frmWait), true, true, false);
            //// 1.do something
            ////      ...
            //// 2.传参控制显示效果...
            //for (int i = 1; i <= 100; i++)
            //{
            //    //SplashScreenManager.Default.SetWaitFormCaption("请稍候");
            //    SplashScreenManager.Default.SetWaitFormDescription(i.ToString() + "%");
            //    System.Threading.Thread.Sleep(50);
            //}
            //SplashScreenManager.CloseForm(false);
            #endregion

            ShowOrActiveForm(typeof(Form1));
        }

        private void nbiCustomer_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowOrActiveForm(typeof(Form1));
        }

        private void nbiSupplier_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {

        }

        private void nbMain_Resize(object sender, EventArgs e)
        {
            pnlMain.Width = ((Control)sender).Width + pnlMain.Padding.Left + 6;
        }

        #region Skins
        private void InitSkins()
        {
            barManager1.ForceInitialize();
            foreach (DevExpress.Skins.SkinContainer cnt in DevExpress.Skins.SkinManager.Default.Skins)
            {
                /*BarButtonItem无法设置Checked属性,弃之...
                BarButtonItem item = new BarButtonItem(barManager1, cnt.SkinName);
                item.ButtonStyle = BarButtonStyle.Check;
                */
                BarCheckItem item = new BarCheckItem(barManager1, false);
                item.Caption = cnt.SkinName;
                item.GroupIndex = 1;    //默认为0=允许多选,非0=互斥单选
                if (cnt.SkinName.Equals(DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName))
                    item.Checked = true;
                bsiSkin.AddItem(item);
                item.ItemClick += new ItemClickEventHandler(OnSkinClick);
            }
        }
        private void OnSkinClick(object sender, ItemClickEventArgs e)
        {
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(e.Item.Caption);
            barManager1.GetController().PaintStyleName = "Skin";
            bsiSkin.Caption = bsiSkin.Hint = e.Item.Caption;
            bsiSkin.Hint = bsiSkin.Caption;
            bsiSkin.ImageIndex = -1;
        }
        #endregion

        #region Merge/UnMerge子窗体工具栏
        private void barManager1_Merge(object sender, DevExpress.XtraBars.BarManagerMergeEventArgs e)
        {
            var parent = barManager1.Bars["Tools"];
            var child = e.ChildManager.Bars["Tools"];
            if (child != null)
                parent.Merge(child);
        }

        private void barManager1_UnMerge(object sender, DevExpress.XtraBars.BarManagerMergeEventArgs e)
        {
            barManager1.Bars["Tools"].UnMerge();
        }
        #endregion

        #region 显示OR激话Mdi子窗体（单例）
        /// <summary>
        /// 显示OR激话Mdi子窗体（单例）
        /// </summary>
        /// <param name="type">子窗体的类型</param>
        private void ShowOrActiveForm(Type type)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == type)
                {
                    f.Activate();
                    return;
                }
            }
            Form form = type.Assembly.CreateInstance(type.ToString()) as Form;
            form.MdiParent = this;
            form.Show();
        }
        #endregion

        #region Mdi Page 处理
        private void mdiManager1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                XtraMdiTabPage page = mdiManager1.CalcHitInfo(new Point(e.X, e.Y)).Page as XtraMdiTabPage;
                if (page != null)
                {
                    if (mdiManager1.SelectedPage != page)
                        mdiManager1.SelectedPage = page;
                    pmTabbed.ShowPopup(Control.MousePosition);
                }
            }
        }

        private void biClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraMdiTabPage page = mdiManager1.SelectedPage;
            if (page != null)
                page.MdiChild.Close();
        }

        private void biCloseAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                f.Close();
            }
        }

        private void biCloseAllExcept_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraMdiTabPage page = mdiManager1.SelectedPage;
            foreach (Form f in this.MdiChildren)
            {
                if (f != page.MdiChild)
                {
                    f.Close();
                    f.Dispose();
                }
            }
        }

        #region 示例代码：如何使某子窗体不能被关闭（子窗体代码）
        /*
            private void frmChild_FormClosing(object sender, FormClosingEventArgs e)
            {
                if (e.CloseReason != CloseReason.MdiFormClosing)
                {
                    e.Cancel = true;
                }
            }
        */
        #endregion
        #endregion
    }
}