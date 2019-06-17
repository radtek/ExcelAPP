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
using RestSharp;
using System.Configuration;
using ExcelClient.RestService;

namespace ExcelClient
{
    public partial class FormConsole : DevExpress.XtraEditors.XtraForm
    {
        private DevExpress.XtraNavBar.NavBarGroup nbgBase1;
        int x, y;
        public FormConsole()
        {
            InitializeComponent();

            InitTree();
            InitDW();
            pnlTop.SuspendLayout();
            /* --最后Add的优先级最高-- */
            // pnlTop.Controls.Add(banner);
            //pnlTop.Controls.Add(pnlLogo);
            /* --此部分需注意，以免乱-- */
            //pnlLogo.Dock = DockStyle.Left;
            // banner.Dock = DockStyle.Fill;
            pnlTop.ResumeLayout(false);


            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.Skins.SkinManager.EnableMdiFormSkins();

            //UserLookAndFeel.Default.StyleChanged += Default_StyleChanged;

            DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = new Font("微软雅黑", 10);
            //DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font("Tahoma", 12);
            //UserLookAndFeel.Default.SetSkinStyle("Office 2013 White");

            UserLookAndFeel.Default.SetSkinStyle("Office 2013");
            pnlMain.Width = 290;

            barStaticItem1.Caption = UserInfo.UserName;

            //this.FormBorderStyle = FormBorderStyle.None;
            //this.ShowInTaskbar = false;

            //this.pnlTop.MouseDown += FrmLogin_MouseDown;
            //this.pnlTop.MouseMove += FrmLogin_MouseMove;

            //this.WindowState = FormWindowState.Normal;

        }
 
        public void InitDW()
        {


            var model = DWService.GetDWData(" and lsbzdw_mx='1'");
            if (model.data.Rows.Rows.Count > 0)
            {
                DataRow row = model.data.Rows.Rows[0];

                buttonEditDW.Text = row["LSBZDW_DWMC"].ToString();
                CurDWBH = row["LSBZDW_DWBH"].ToString();
            }

        }

        //public class FuncModel
        //{

        //    public string res { get; set; }
        //    public DataTable data { get; set; }
        //}
        public void InitTree()
        {
            FuncService svr = new FuncService();


            var model = svr.GetMenuList();

            treeList1.ParentFieldName = "Pid";
            treeList1.CustomDrawNodeImages += treeList_CustomDrawNodeImages;
            treeList1.DataSource = model.data;

            treeList1.ExpandAll();
        }
        public static void treeList_CustomDrawNodeImages(object sender, DevExpress.XtraTreeList.CustomDrawNodeImagesEventArgs e)
        {
            if (e.Node.Nodes.Count > 0)
            {
                e.SelectImageIndex = 0;
            }
            else
            {
                e.SelectImageIndex = 1;
            }
        }
        void pnlLogo_DoubleClick(object sender, EventArgs e)
        {

        }

        //void Default_StyleChanged(object sender, EventArgs e)
        //{
        //    string skinName = UserLookAndFeel.Default.SkinName;
        //    bool border = skinName.StartsWith("Sharp", StringComparison.OrdinalIgnoreCase);
        //    if (border)
        //    {
        //        pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
        //    }
        //    else
        //    {
        //        pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        //    }

        //    // pnlLogo.BackColor = pnlTop.BackColor;
        //    pnlTop.Refresh();
        //}

        private void frmConsole_Load(object sender, EventArgs e)
        {

            // 打开首页
            var host = ConfigurationManager.AppSettings["Host"].ToString();
            var url = host + "/drp/NoticeIndex.html?p_USERID=" + UserInfo.UserCode + "&p_USERNAME=" + UserInfo.UserName;

            FormCef form = new FormCef();
            form.SetURL(url);
            form.MdiParent = this;
            form.Text = "首页";
            form.Tag = "main";
            form.Show();
            FormList["main"] = form;

            SplashScreenManager.CloseForm(false);
            this.WindowState = FormWindowState.Maximized;


            timer1.Interval = 1000;
            timer1.Start();
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

            ShowOrActiveForm(typeof(FormDW));
        }

        private void nbiCustomer_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowOrActiveForm(typeof(FormDW));
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
            //foreach (Form f in this.MdiChildren)
            //{
            //    if (f.GetType() == type)
            //    {
            //        f.Activate();
            //        return;
            //    }
            //}
            Form form = type.Assembly.CreateInstance(type.ToString()) as Form;
            form.MdiParent = this;
            form.Show();
            Main f1 = (Main)form;
            f1.SetURL("123", "0101");
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
            {
                if (page.MdiChild.Tag.ToString() == "main") return;
                page.MdiChild.Close();
                FormList.Remove(page.MdiChild.Tag.ToString());
            }

        }

        private void biCloseAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f.Tag.ToString() == "main") continue;

                FormList.Remove(f.Tag.ToString());
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
                    if (f.Tag.ToString() == "main") continue;
                    FormList.Remove(f.Tag.ToString());
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




        /// <summary>
        /// 弹出选择单位帮助处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEditDW_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //MessageBox.Show("1asdf");

            FormDW frm = new FormDW();
            frm.StartPosition = FormStartPosition.CenterScreen;
            var dg = frm.ShowDialog();
            if (dg.Equals(DialogResult.OK))
            {
                //MessageBox.Show(frm.strName);
                buttonEditDW.Text = frm.strName;
                CurDWBH = frm.strKey;

                //切换单位关闭所有的窗体
                foreach (Form f in this.MdiChildren)
                {
                    if (f.Tag.ToString() == "main") continue;

                    FormList.Remove(f.Tag.ToString());
                    f.Close();
                }
            }

        }
        /// <summary>
        /// 打开类型 1 规则导入 2 url 3 winform
        /// </summary>
        private void treeList1_DoubleClick(object sender, EventArgs e)
        {
            string ID = treeList1.FocusedNode.GetValue("ID").ToString();

            string strType = treeList1.FocusedNode.GetValue("REFType").ToString();
            string strRuleID = treeList1.FocusedNode.GetValue("REFID").ToString();
            string name = treeList1.FocusedNode.GetValue("NAME").ToString();


            if (strType == "1")
            {
                openRefInfo(name, strRuleID);
            }
            if (strType == "2")
            {
                openURL(name, ID, treeList1.FocusedNode.GetValue("URLInfo").ToString());
            }

            if (strType == "0")
            {
                openDefInfo(name, ID);
            }
            //URLInfo
            //FormInfo

        }

        public static string CurDWBH = "";

        public static Dictionary<string, Form> FormList = new Dictionary<string, Form>();

        private void openRefInfo(string name, string ruleID)
        {
            if (FormList.ContainsKey(ruleID))
            {
                FormList[ruleID].Activate();
                return;
            }

            Main form = new Main();
            form.SetURL(ruleID, CurDWBH);
            form.Text = name;
            form.Tag = ruleID;
            form.MdiParent = this;
            form.Show();
            FormList[ruleID] = form;
        }

        /// <summary>
        /// 打开多维分析设计页面
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        private void openDefInfo(string name, string id)
        {
            if (FormList.ContainsKey(id))
            {
                FormList[id].Activate();
                return;
            }

            frmDefList form = new frmDefList();
            form.Text = name;
            form.Tag = id;
            form.MdiParent = this;
            form.Show();
            FormList[id] = form;
        }

        public void openDev(string id, string name, Form form)
        {
            if (FormList.ContainsKey(id))
            {
                FormList[id].Activate();
                return;
            }
            form.Text = name;
            form.Tag = id;
            form.MdiParent = this;
            form.Show();
            FormList[id] = form;

        }


        private void openURL(string name, string ruleID, string url)
        {
            if (FormList.ContainsKey(ruleID))
            {
                FormList[ruleID].Activate();
                return;
            }

            FormCef form = new FormCef();
            form.SetURL(url);
            form.Text = name;
            form.parentFF = this;
            form.Tag = ruleID;
            form.MdiParent = this;
            form.Show();
            FormList[ruleID] = form;
        }

        private void FormConsole_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void barStaticItem2_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            barStaticItem4.Caption= DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }

        private void pnlTop_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}