using DevExpress.XtraTreeList;
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
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();
            treeList1.OptionsBehavior.EnableFiltering = true;
            ///treeList1.OptionsView.ShowAutoFilterRow = true;
            treeList1.OptionsFilter.FilterMode = FilterMode.Smart;
            treeList1.OptionsBehavior.Editable = false;
            DataTable dt = new DataTable();

            DataColumn dc1 = new DataColumn();
            dc1.ColumnName = "DWBH";
            dc1.DataType = typeof(string);


            DataColumn dc2 = new DataColumn();
            dc2.ColumnName = "ID";
            dc2.DataType = typeof(string);

            DataColumn dc3 = new DataColumn();
            dc3.ColumnName = "ParentID";
            dc3.DataType = typeof(string);


            DataColumn dc4 = new DataColumn();
            dc4.ColumnName = "DWMC";
            dc4.DataType = typeof(string);

            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);


            DataRow dr1 = dt.NewRow();
            dr1[0] = "BB";
            dr1[1] = 0;
            dr1[2] = "";
            dr1[3] = "32";

            DataRow dr2 = dt.NewRow();
            dr2[0] = "BB1";
            dr2[1] = 1;
            dr2[2] = 0;
            dr2[3] = "123";

            DataRow dr3 = dt.NewRow();
            dr3[0] = "M1";
            dr3[1] = 2;
            dr3[2] = 1;
            dr3[3] = "123";

            DataRow dr4 = dt.NewRow();
            dr4[0] = "M2";
            dr4[1] = 3;
            dr4[2] = 0;
            dr4[3] = "123";

            DataRow dr5 = dt.NewRow();
            dr4[0] = "M2";
            dr4[1] = 3;
            dr4[2] = 0;
            dr4[3] = "123";

            dt.Rows.Add(dr1);
            dt.Rows.Add(dr2);
            dt.Rows.Add(dr3);
            dt.Rows.Add(dr4);
            dt.Rows.Add(dr5);
            treeList1.DataSource = dt;

            //treeList1.KeyFieldName = "DWBH";
            //treeList1.ParentFieldName = "PDWBH";




        }
        private string nodeText = "";


        private void treeList1_FilterNode(object sender, DevExpress.XtraTreeList.FilterNodeEventArgs e)
        {

        }
        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            nodeText = textBox1.Text;
            treeList1.FilterNodes();
        }



        private void treeList1_FilterNode_1(object sender, DevExpress.XtraTreeList.FilterNodeEventArgs e)
        {
            string NodeText = e.Node.GetDisplayText(treeList1.Nodes[0].Id);
            bool IsVisible = NodeText.ToUpper().IndexOf(nodeText.ToUpper()) >= 0;

            if (IsVisible)
            {
                DevExpress.XtraTreeList.Nodes.TreeListNode Node = e.Node.ParentNode;
                while (Node != null)
                {
                    if (!Node.Visible)
                    {
                        Node.Visible = true;
                        Node = Node.ParentNode;
                    }
                    else
                        break;
                }
            }

            e.Node.Visible = IsVisible;
            e.Handled = true;
        }


        public string strName = "";
        private void treeList1_DoubleClick(object sender, EventArgs e)
        {

            string strKey = treeList1.FocusedNode.GetValue("DWBH").ToString();
            strName = treeList1.FocusedNode.GetValue("DWMC").ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();
          
        }
    }
}
