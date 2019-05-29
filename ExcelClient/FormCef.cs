using NetDimension.NanUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelClient
{
    public partial class FormCef : Formium
    {
        public FormCef()
        {
            InitializeComponent();
        }

        public void SetURL(string url)
        {

            this.LoadUrl(url);

        }
    }
}
