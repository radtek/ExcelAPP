﻿using System;
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

namespace ExcelClient
{
    public partial class FormConsole : DevExpress.XtraEditors.XtraForm
    {
        private DevExpress.XtraNavBar.NavBarGroup nbgBase1;

        public FormConsole()
        {
            InitializeComponent();

            InitTree();
            pnlTop.SuspendLayout();
            /* --最后Add的优先级最高-- */
            // pnlTop.Controls.Add(banner);
            //pnlTop.Controls.Add(pnlLogo);
            /* --此部分需注意，以免乱-- */
            //pnlLogo.Dock = DockStyle.Left;
            // banner.Dock = DockStyle.Fill;
            pnlTop.ResumeLayout(false);

            UserLookAndFeel.Default.StyleChanged += Default_StyleChanged;
            //pnlLogo.DoubleClick += pnlLogo_DoubleClick;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConsole));

            var nbiGoods = new DevExpress.XtraNavBar.NavBarItem();
            nbiGoods.Caption = "商品资料11";
            nbiGoods.LargeImage = ((System.Drawing.Image)(resources.GetObject("nbiGoods.LargeImage")));
            nbiGoods.Name = "nbiGoods11";
            nbiGoods.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nbiGoods_LinkClicked);
            nbiGoods.Tag = "11";

            this.nbgBase1 = new DevExpress.XtraNavBar.NavBarGroup();
            this.nbgBase1.Caption = "测试22";
            this.nbgBase1.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.LargeIconsText;
            this.nbgBase1.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(nbiGoods) });
            this.nbgBase1.Name = "nbgBase1";
            this.nbgBase1.TopVisibleLinkIndex = 1;
            this.nbMain.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.nbgBase1 }
            );
        }


        public class FuncModel
        {

            public string res { get; set; }
            public DataTable data { get; set; }
        }
        public void InitTree()
        {
            var client = new RestClient("http://localhost/excel/api/help.ashx");
            var request = new RestRequest(Method.POST);
            //request.AddHeader("Postman-Token", "ef2f2b5e-1172-4cee-8edf-ba1a35fc1971");
            //request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //request.AddHeader("SessionId", "1ef7d3b2-6ef1-4e39-8dc6-50eb6976c76");

            request.AddParameter("op", "GetFunc");

            //request.AddParameter("application/x-www-form-urlencoded", "file=444&key=6QGVVkEJkH1d8oiMtvboPc&secret=36a837aacbd14026861c22ac9ae07de6&pid=2", ParameterType.RequestBody);
            //IRestResponse<FuncModel> response = client.Execute<FuncModel>(request);
            IRestResponse response = client.Execute(request);
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<FuncModel>(response.Content);

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

            // pnlLogo.BackColor = pnlTop.BackColor;
            pnlTop.Refresh();
        }

        private void frmConsole_Load(object sender, EventArgs e)
        {

            var url = "http://10.8.2.107:9001/drp/NoticeIndex.html?p_USERID=" + UserInfo.UserCode + "&p_USERNAME=" + UserInfo.UserName;

            FormCef form = new FormCef();
            form.SetURL(url);
            form.MdiParent = this;
            form.Text = "首页";
            form.Tag = "main";
            form.Show();
            FormList["main"] = form;

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
                page.MdiChild.Close();
                FormList.Remove(page.MdiChild.Tag.ToString());
            }

        }

        private void biCloseAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
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
            var dg = frm.ShowDialog();
            if (dg.Equals(DialogResult.OK))
            {
                MessageBox.Show(frm.strName);
                buttonEditDW.Text = frm.strName;
                CurDWBH = frm.strKey;
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
            form.Tag = ruleID;
            form.MdiParent = this;
            form.Show();
            FormList[ruleID] = form;
        }
    }
}