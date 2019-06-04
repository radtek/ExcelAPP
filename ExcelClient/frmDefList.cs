using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Configuration;
using DevExpress.LookAndFeel;

namespace ExcelClient
{
    public partial class frmDefList : DevExpress.XtraEditors.XtraForm
    {
        public frmDefList()
        {
            InitializeComponent();
        }
        #region  变量定义
        WsGetDataClient.WSGetData mgr = new WsGetDataClient.WSGetData();
        
        public WsGetDataClient.WSGetData WSMgr
        {
            set { this.mgr = value; }
        } 
        public DataTable  dtData;//数据集 
        private string ProcessID = DevQryPubFun.GSYDBSrc;
        
        JTPUBQRDEFMgr defMgr;
        #endregion


        private void frmDevBBShow_Load(object sender, EventArgs e)
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(DevQryShow.CultureInfo );
            //DevExpress.UserSkins.BonusSkins.Register();
            //DevExpress.UserSkins.OfficeSkins.Register();

            //defaultLookAndFeel1.LookAndFeel.SkinName = "The Asphalt World";
            //defaultLookAndFeel1.LookAndFeel.SkinName = "GS V5";        
            //this.Text = "";             
            //mgr.Url = "http://10.24.11.123/JTGL_PubQry/JTGLPubQry_Svr/WSGetData.asmx";


            //UserLookAndFeel.Default.SetSkinStyle("Office 2013");
            DevQryPubFun.WrapService(mgr); 
        }


        private void frmDevBBShow_Shown(object sender, EventArgs e)
        {
            try
            {
                defMgr = new JTPUBQRDEFMgr( mgr,this.ProcessID);
                getData();
                bindGrid();             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

   
        private void btnQry_Click(object sender, EventArgs e)
        {
            bindGrid();
        }

        private void btnadd_Click(object sender, EventArgs e)
        {

            int focusRow = gridView1.FocusedRowHandle;
            frmDefEdit defedt = new frmDefEdit();
            defedt.WSMgr = mgr;
            defedt.ProcessID = this.ProcessID;
            defedt.ShowDialog();
            getData();
            bindGrid();
            if(focusRow >=0)
            gridView1.FocusedRowHandle = focusRow;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
           
            if (gridView1.FocusedRowHandle >= 0)
            {
                int focusRow = gridView1.FocusedRowHandle;
                frmDefEdit defedt = new frmDefEdit();
                DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string id = row["JTPUBQRDEF_ID"].ToString();
                defedt.ID = id;
                defedt.WSMgr = mgr;
                defedt.ProcessID = this.ProcessID;
                defedt.ShowDialog();
                getData();
                bindGrid();
                gridView1.FocusedRowHandle = focusRow;
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle >= 0)
            {

                if (MessageBox.Show("确实要删除吗？", "删除确认", MessageBoxButtons.YesNo) == DialogResult.No) return;
                DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string id = row["JTPUBQRDEF_ID"].ToString();
                defMgr.delete(id);
                getData();
                bindGrid(); 
            }
        }

        private void btnTit_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle >= 0)
            {
                frmDefListTit titlst = new frmDefListTit();
                DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string id = row["JTPUBQRDEF_ID"].ToString();
                titlst.ID = id;
                titlst.WSMgr = mgr; 
                titlst.ShowDialog();
            }

        }

        private void btnOut_Click(object sender, EventArgs e)
        {
            //if (gridView1.FocusedRowHandle >= 0)
            //{
            //    frmDefOut frmout = new frmDefOut();
            //    DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            //    string id = row["JTPUBQRDEF_ID"].ToString();
            //    frmout.ID = id;
            //    frmout.WSMgr = mgr;
            //    frmout.row = row; 
            //    frmout.ShowDialog();
            //}

        }

   

        #region 获取数据

        private void getData()
        {
            if (dtData != null && dtData.Rows.Count > 0)
                dtData.Rows.Clear();
            dtData = getDataTable("select * from JTPUBQRDEF ");
        }
        private void bindGrid()
        {
            string vswhere = " 1=1 ";
            if (tbBH.Text.Trim() != "")
                vswhere += " and JTPUBQRDEF_BH like '%" + tbBH.Text + "%' ";
            if (tbMC.Text.Trim() != "")
                vswhere += " and JTPUBQRDEF_MC like '%" + tbMC.Text + "%' ";
            if (string.IsNullOrEmpty(vswhere))
                gridControl1.DataSource = dtData.DefaultView;
            else
                gridControl1.DataSource = getDataFormTbByWhere(dtData, vswhere);

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

        private DataTable getDataTableFromDataTableByWhere(DataTable dt, string fieldName, string fieldValue)
        {
            DataTable dtnew = new DataTable();
            dtnew = dt.Clone();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row[fieldName].ToString() == fieldValue)
                    dtnew.Rows.Add(row.ItemArray);
            }
            return dtnew;
        }
        private DataTable getDataFormTbByWhere(DataTable dt, string vswhere)
        {
            DataTable dtnew = new DataTable();
            dtnew = dt.Clone();
            DataRow[] rows = dt.Select(vswhere);
            foreach (DataRow row in rows)
            {
                dtnew.Rows.Add(row.ItemArray);
            }
            return dtnew;
        }

        #endregion 获取数据

        private void btnmenu_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle >= 0)
            {
                //frmDefMenu frmMenu = new frmDefMenu();
                //DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                //string id = row["JTPUBQRDEF_ID"].ToString();
                //frmMenu.ID = id;
                //frmMenu.WSMgr = mgr; 
                //frmMenu.ShowDialog();
            }
        }


 
        


      



    }


}
