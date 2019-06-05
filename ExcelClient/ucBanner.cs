using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
 

namespace FEAP.Controls
{
    public partial class ucBanner : DevExpress.XtraEditors.XtraUserControl
    {
        Dictionary<string, string> _skins = new Dictionary<string, string>
        {
            {"DevExpress Style","默认主题"},
            {"DevExpress Dark Style","默认(黑)"},
            {"Sharp","尖锐"},
            {"Sharp Plus","尖锐（增强）"},
            {"Caramel","焦糖"},
            {"Lilian","莉莲"},
            {"Money Twins","财富"}
        };
        public ucBanner()
        {
            InitializeComponent();
            InitSkins();
        }

        #region Skins
        public void InitSkins()
        {
            barManager1.ForceInitialize();
            foreach (KeyValuePair<string, string> item in _skins)
            {
                BarCheckItem bi = new BarCheckItem(barManager1, false);
                bi.Caption = item.Value;
                bi.Hint = item.Key;
                bi.GroupIndex = 1;
                if (item.Key.Equals(UserLookAndFeel.Default.SkinName))
                    bi.Checked = true;
                pmSkin.AddItem(bi);
                bi.ItemClick += OnSkinClick;
            }
        }

        void OnSkinClick(object sender, ItemClickEventArgs e)
        {
            UserLookAndFeel.Default.SetSkinStyle(e.Item.Hint);
            barManager1.GetController().PaintStyleName = "Skin";
        }
        #endregion

        private void hl4_Click(object sender, EventArgs e)
        {
            Point pos = new Point(hl4.Left, hl4.Top + hl4.Height);
            pmSkin.ShowPopup(PointToScreen(pos));
        }

        private void hl5_Click(object sender, EventArgs e)
        {

        }

        private void hl6_Click(object sender, EventArgs e)
        {

        }
    }
}
