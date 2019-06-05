using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace FEAP.Controls
{
    [ToolboxItem(true)]
    public class HotLabel : LabelControl
    {
        string linkText = string.Empty;
        Color originColor;  //扩展-当前skin下的初始ForeColor
        Color linkColor;    //扩展-当前Skin下的linkColor（特定的，默认值）
        public HotLabel()
        {
            LookAndFeel.StyleChanged += new EventHandler(Default_StyleChanged);
            UpdateColors();

            //Init...
            AutoSizeMode = LabelAutoSizeMode.None;
            Appearance.ImageAlign = ContentAlignment.TopCenter;
            Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            Appearance.TextOptions.VAlignment = VertAlignment.Bottom;
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(42, base.DefaultSize.Height);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LookAndFeel.StyleChanged -= new EventHandler(Default_StyleChanged);
            }
            base.Dispose(disposing);
        }
        [DefaultValue("")]
        public string LinkText
        {
            get { return linkText; }
            set
            {
                if (linkText == value) return;
                linkText = value;
            }
        }
        void Default_StyleChanged(object sender, EventArgs e)
        {
            UpdateColors();
        }
        public void UpdateColors()
        {
            if (DesignMode) return;
            //this.Appearance.ForeColor = WinHelper.GetLinkColor(LookAndFeel);
            originColor = Appearance.ForeColor;
            
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (DesignMode) return;
            Appearance.Font = new Font(Appearance.Font, FontStyle.Underline);
            Appearance.ForeColor = linkColor;
            Cursor = Cursors.Hand;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode) return;
            Appearance.Font = new Font(Appearance.Font, FontStyle.Regular);
            Appearance.ForeColor = originColor;
            Cursor = Cursors.Default;
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (DesignMode || string.IsNullOrEmpty(LinkText)) return;
            Cursor = Cursors.WaitCursor;
            //ObjectHelper.ShowWebSite(LinkText);
        }
    }
}
