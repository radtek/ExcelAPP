using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Configuration;
namespace ExcelClient
{
    public partial class frmDefIndex : DevExpress.XtraEditors.XtraForm
    {
        public frmDefIndex()
        {
            InitializeComponent();
        }
        WsGetDataClient.WSGetData mgr;
        private string ProcessID = DevQryPubFun.GSYDBSrc;
        private DataTable dt = new DataTable();
        private DataTable dtHelp = new DataTable();
        public string ID = "";



        public WsGetDataClient.WSGetData WSMgr
        {
            set { this.mgr = value; }
        }
        private void frmDefIndex_Load(object sender, EventArgs e)
        {
            dtHelp = getDataTable("SELECT ID F_HELP,CODE F_HEBH,NAME F_TITL FROM EAHELP");
            bindGrid();
            setCtrlValue();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row = getCtrlValue();
                string sql = string.Format(@"UPDATE CUSTOMFIELDS  set DISPLAYNAME='{2}',INPUTTYPE='{3}',ISDISPLAY='{4}',
GETINFOFROM='{5}',GETINFOWHERE='{6}',ISREQUIRED='{7}',DEFAULTVALUE='{8}',DISPLAYORDER='{9}'
  where  CLASSSETCODE='{0}' and  FIELDNAME='{1}' ", this.ID, row["FIELDNAME"].ToString(), row["DISPLAYNAME"].ToString(),
                                                  row["INPUTTYPE"].ToString(), row["ISDISPLAY"].ToString(),
                                                   row["GETINFOFROM"].ToString(), row["GETINFOWHERE"].ToString(),
                                                    row["ISREQUIRED"].ToString(), row["DEFAULTVALUE"].ToString(), row["DISPLAYORDER"].ToString());
                WebSvrGetData.execsql(this.ProcessID, sql, mgr);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnColse_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void bindGrid()
        {
            string sql = string.Format(@"select FIELDNAME ,DISPLAYNAME,INPUTTYPE,ISDISPLAY,GETINFOFROM,GETINFOWHERE,ISREQUIRED,DEFAULTVALUE ,DISPLAYORDER
FROM CUSTOMFIELDS WHERE CLASSSETCODE='{0}'", this.ID);
            dt = getDataTable(sql);
            gridControl1.DataSource = dt;

        }


        private void setCtrlValue()
        {
            if (gridView1.FocusedRowHandle >= 0)
            {
                DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                CU_FIELDNAME.Text = row["FIELDNAME"].ToString();
                CU_DISPLAYNAME.Text = row["DISPLAYNAME"].ToString().Trim();
                CU_INPUTTYPE.SelectedIndex = Convert.ToInt32(row["INPUTTYPE"]);
                CU_ISDISPLAY.SelectedIndex = Convert.ToInt32(row["ISDISPLAY"]);
                CU_GETINFOFROM.Text = row["GETINFOFROM"].ToString();
                CU_GETINFOWHERE.Text = row["GETINFOWHERE"].ToString().Replace("'", "''");
                CU_ISREQUIRED.SelectedIndex = Convert.ToInt32(row["ISREQUIRED"]);
                CU_DEFAULTVALUE.Text = row["DEFAULTVALUE"].ToString();
                CU_DISPLAYORDER.Text = row["DISPLAYORDER"].ToString();
                GETINFOFROM.Items.Clear();
                GETINFOFROM.Items.Add(" ");
            }
        }

        private DataRow getCtrlValue()
        {
            DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (string.IsNullOrEmpty(CU_FIELDNAME.Text))
                throw new Exception("字段（参数）不能为空！");
            row["FIELDNAME"] = CU_FIELDNAME.Text;
            row["DISPLAYNAME"] = CU_DISPLAYNAME.Text;
            row["INPUTTYPE"] = CU_INPUTTYPE.SelectedIndex;
            row["ISDISPLAY"] = CU_ISDISPLAY.SelectedIndex;
            row["GETINFOFROM"] = CU_GETINFOFROM.Text;
            row["GETINFOWHERE"] = CU_GETINFOWHERE.Text;
            row["ISREQUIRED"] = CU_ISREQUIRED.SelectedIndex;
            row["DEFAULTVALUE"] = CU_DEFAULTVALUE.Text;
            row["DISPLAYORDER"] = CU_DISPLAYORDER.Text;
            return row;

        }
        private void bindHelpCtrl()
        {
            DataRow[] rows;
            if (string.IsNullOrEmpty(CU_GETINFOFROM.Text))
            {
                rows = dtHelp.Select();
            }
            else
            {
                rows = dtHelp.Select(string.Format("F_HELP like '%{0}%'", CU_GETINFOFROM.Text));
            }
            GETINFOFROM.Items.Clear();
            GETINFOFROM.Items.Add(" ");
            foreach (DataRow row in rows)
            {
                GETINFOFROM.Items.Add(row[0] + "~" + row[1] + "~" + row[2]);
            }

        }

        private DataTable getDataTable(string sql)
        {
            DataTable dt = new DataTable();
            DataSet vsds = StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByte(this.ProcessID, sql));
            if (vsds.Tables.Count > 0)
                return vsds.Tables[0];
            else
                return dt;
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            setCtrlValue();
        }

        private void CU_VALTYPE_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (CU_VALTYPE.SelectedItem.ToString())
            {
                case "当前单位":
                    CU_DEFAULTVALUE.Text = "@DWBH@";
                    break;
                case "当前日期":
                    CU_DEFAULTVALUE.Text = "@DATE@";
                    break;
                case "本位币":
                    CU_DEFAULTVALUE.Text = "@BWB@";
                    break;
                case "GET参数":
                    CU_DEFAULTVALUE.Text = "@GET@";
                    break;
                case "当前年度":
                    CU_DEFAULTVALUE.Text = "@YEAR@";
                    break;
                case "当前月份":
                    CU_DEFAULTVALUE.Text = "@MONTH@";
                    break;
                case "当前岗位":
                    CU_DEFAULTVALUE.Text = "@GW@";
                    break;
            }
        }

        private void GETINFOFROM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GETINFOFROM.SelectedIndex == -1) return;
            string selval = GETINFOFROM.SelectedItem.ToString();
            if (selval != " ")
            {
                CU_GETINFOFROM.Text = selval.Substring(0, selval.LastIndexOf("~"));
            }
        }

        private void CU_GETINFOFROM_KeyPress(object sender, KeyPressEventArgs e)
        {
            bindHelpCtrl();
        }










    }


}
