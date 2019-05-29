using DevExpress.XtraTreeList;
using RestSharp;
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
    public partial class FormDW : DevExpress.XtraEditors.XtraForm
    {

        public class RestModel
        {

            public string res { get; set; }
            public RestModelData data { get; set; }
        }
        public class RestModelData
        {

            public DataTable Rows { get; set; }
        }
        public FormDW()
        {
            InitializeComponent();
            treeList1.OptionsBehavior.EnableFiltering = true;
            ///treeList1.OptionsView.ShowAutoFilterRow = true;
            treeList1.OptionsFilter.FilterMode = FilterMode.Smart;
            treeList1.OptionsBehavior.Editable = false;


            var client = new RestClient("http://localhost:1402/api/help.ashx");
            var request = new RestRequest(Method.POST);
            //request.AddHeader("Postman-Token", "ef2f2b5e-1172-4cee-8edf-ba1a35fc1971");
            //request.AddHeader("Cookie", UserInfo.Cookie);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var parm = UserInfo.Cookie.Split(';');
            request.AddParameter(parm[0].Split('=')[0], parm[0].Split('=')[1], ParameterType.Cookie);
            request.AddHeader("Set-Cookie", UserInfo.Cookie);
            request.AddParameter("op", "GetHelpData");
            request.AddParameter("id", "LSBZDW");
            request.AddParameter("filter", "");
            request.AddParameter("order", "");
            request.AddParameter("row", "");
            request.AddParameter("page", 1);
            request.AddParameter("pageSize", 500);

            IRestResponse response = client.Execute(request);
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<RestModel>(response.Content);


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


            foreach (DataRow row in model.data.Rows.Rows)
            {
                DataRow dr1 = dt.NewRow();
                dr1[0] = row["LSBZDW_DWBH"];
                dr1[1] = row["LSBZDW_DWNM"];
                dr1[2] = row["LSBZDW_DWNM"].ToString().Substring(0, (int.Parse(row["LSBZDW_JS"].ToString())-1) * 4);
                dr1[3] = row["LSBZDW_DWMC"];
                dt.Rows.Add(dr1);
            }
 
            //treeList1.KeyFieldName = "DWBH";
            //treeList1.ParentFieldName = "PDWBH";
            treeList1.DataSource =dt;






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
        public string strKey = "";
        private void treeList1_DoubleClick(object sender, EventArgs e)
        {

            strKey = treeList1.FocusedNode.GetValue("DWBH").ToString();
            strName = treeList1.FocusedNode.GetValue("DWMC").ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();

        }
    }
}
