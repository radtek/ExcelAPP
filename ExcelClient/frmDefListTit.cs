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
    public partial class frmDefListTit : DevExpress.XtraEditors.XtraForm
    {
        public frmDefListTit()
        {
            InitializeComponent();
        }
        #region  变量定义
        WsGetDataClient.WSGetData mgr = new WsGetDataClient.WSGetData();
        private string ProcessID = DevQryPubFun.GSYDBSrc;
        public string ID;

        public DataTable dtData;

        public WsGetDataClient.WSGetData WSMgr
        {
            set { this.mgr = value; }
        }
        #endregion 
        private void frmDefListTit_Load(object sender, EventArgs e)
        {
            reloadData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            gridView1.UpdateCurrentRow();
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                if (dtData.Rows[i]["F_TSTEXT"].ToString() == "")
                    dtData.Rows[i]["F_TSTEXT"] = dtData.Rows[i]["F_TEXT"];
            }
            //
            LSTIGSDEVQRYMgr qrymgr = new LSTIGSDEVQRYMgr(mgr, ProcessID);
            qrymgr.Save(dtData, ID);
            MessageBox.Show("保存成功");
            //reloadData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataRow row = dtData.NewRow();
            row["F_DWBH"] = " ";
            row["F_ID"] = ID;
            row["F_FHBZ"] = "1";
            row["F_GSBH"] = "1";
            row["F_TIBH"] = "";
            row["F_JS"] = "1";
            row["F_FIELD"] = "";
            row["F_TEXT"] = "";
            row["F_TSTEXT"] = "";
            row["F_WIDTH"] = "200";
            row["F_HJBZ"] = "0";
            row["F_YHBZ"] = "0";
            row["F_TYPE"] = "C";
            row["F_ALIGN"] = "L";
            row["F_PREC"] = "";
            row["F_GROUP"] = "0";
            row["F_TSHID"] = "0";
            row["F_ISGD"] = "0";
            row["F_PSUMTYPE"] = "SUM";
            dtData.Rows.Add(row);
        }


        private void btnDel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确实要删除吗？本删除按钮将删除所有行记录，并重新加载默认字段！慎用", "删除确认", MessageBoxButtons.YesNo) == DialogResult.No) return;
            LSTIGSDEVQRYMgr qrymgr = new LSTIGSDEVQRYMgr(mgr, ProcessID);
            qrymgr.delete(ID);
            reloadData();

        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle >= 0)
            {
                dtData.Rows.RemoveAt(gridView1.FocusedRowHandle);
            }
        }




        private void getData()
        {
            if (dtData != null && dtData.Rows.Count > 0)
                dtData.Rows.Clear();
            dtData = getDataTable("select * from LSTIGS where F_ID='" + ID + "' and F_DWBH=' '  ");
        }
        private void bindGrid()
        {

            gridControl1.DataSource = dtData.DefaultView;
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
        /// <summary>
        /// 重载获取数据源函数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbsrc"></param>
        /// <returns></returns>
        private DataTable getDataTable(string sql, string dbsrc)
        {
            DataTable dt = new DataTable();
            DataSet vsds = StringToDataSet.getDataSetFromZipDataFormat(mgr.getZipDataFormatByte(dbsrc, sql));
            if (vsds.Tables.Count > 0)
                return vsds.Tables[0];
            else
                return dt;
        }
        private DataTable getQryData()
        {
            JTPUBQRDEFMgr defmgr = new JTPUBQRDEFMgr(mgr, ProcessID);
            JTPUBQRDEFEty ety = defmgr.getEty(this.ID);
            DataTable dt = null;
            if (ety.JTPUBQRDEF_TYPE.ToUpper() == "SQL")
            {
                string sql = ety.JTPUBQRDEF_SQL + "  " + ety.JTPUBQRDEF_WHERE;
                if (mgr.getDBType(ety.JTPUBQRDEF_DBSRC) == "ORA")
                {
                    sql = ety.JTPUBQRDEF_ORA + "  " + ety.JTPUBQRDEF_WHERE;
                }
                dt = getDataTable(sql, ety.JTPUBQRDEF_DBSRC);
            }
            else
            {
                string[] paramArr = null;
                string[] valueArr = null;
                DataTable dtwhere = defmgr.getWhereDt(this.ID);
                if (dtwhere.Rows.Count > 0)
                {

                    paramArr = new string[dtwhere.Rows.Count];
                    valueArr = new string[dtwhere.Rows.Count];
                    for (int i = 0; i < dtwhere.Rows.Count; i++)
                    {
                        paramArr[i] = dtwhere.Rows[i]["PARAMDEF_NAME"].ToString();
                        valueArr[i] = "";

                    }
                }
                string proc = ety.JTPUBQRDEF_SQL;
                if (mgr.getDBType(ety.JTPUBQRDEF_DBSRC) == "ORA")
                {
                    proc = ety.JTPUBQRDEF_ORA;
                }
                dt = WebSvrGetData.getDataTable(ety.JTPUBQRDEF_DBSRC, ety.JTPUBQRDEF_SQL, paramArr, valueArr, mgr);
            }
            return dt;

        }
        /*F_DWBH,F_ID,F_FHBZ,F_GSBH,F_TIBH,F_JS,F_FIELD,F_TEXT,F_TYPE,F_ALIGN,
F_WIDTH,F_PREC,F_HJBZ,F_YHBZ
' ',' ','1','01','001fjm','1js','field','text','type, c n ','align L R M','width 200','prec S J 2 3','HJBZ 0 1','YHBZ 0 1'*/
        private DataTable genTitleData(DataTable dtTitle)
        {
            if (dtTitle.Rows.Count == 0)
            {
                DataTable dtqry = null;
                try
                {
                    dtqry = getQryData();
                }
                catch (Exception ex)
                {
                }
                if (dtqry != null)
                {
                    int i = 0;
                    foreach (DataColumn col in dtqry.Columns)
                    {
                        i++;
                        DataRow row = dtTitle.NewRow();
                        row["F_DWBH"] = " ";
                        row["F_ID"] = ID;
                        row["F_FHBZ"] = "1";
                        row["F_GSBH"] = "1";
                        row["F_TIBH"] = (i + "").PadLeft(3, '0');
                        row["F_JS"] = "1";

                        row["F_FIELD"] = col.ColumnName.ToUpper();
                        row["F_TEXT"] = col.ColumnName;
                        row["F_TSTEXT"] = "";
                        row["F_WIDTH"] = "200";

                        row["F_HJBZ"] = "0";
                        row["F_YHBZ"] = "0";


                        if (col.DataType == System.Type.GetType("System.String"))
                        {
                            row["F_TYPE"] = "C";
                            row["F_ALIGN"] = "L";
                            row["F_PREC"] = "";
                        }
                        else
                        {
                            row["F_TYPE"] = "N";
                            row["F_ALIGN"] = "R";
                            row["F_PREC"] = "2";
                        }

                        dtTitle.Rows.Add(row);
                    }
                }
            }
            return dtTitle;
        }

        private void reloadData()
        {
            getData();
            dtData = genTitleData(dtData);
            bindGrid();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {

            JTPUBQRDEFMgr defmgr = new JTPUBQRDEFMgr(mgr, ProcessID);
            JTPUBQRDEFEty ety = defmgr.getEty(this.ID);
            DevQryShow frm = new DevQryShow();
            if (ety.JTPUBQRDEF_TYPE.ToUpper() == "SQL")
            {
                string sql = ety.JTPUBQRDEF_SQL;
                if (mgr.getDBType(ety.JTPUBQRDEF_DBSRC) == "ORA")
                {
                    sql = ety.JTPUBQRDEF_ORA;
                }
                frm.showWithParam(ety.JTPUBQRDEF_DBSRC, " ", " ", "", this.ID, "", "2", "2", "", sql, ety.JTPUBQRDEF_SUBTIL, "0", " ");
            }
            else
            {
                string vsParam = "";
                string vsValue = "";
                DataTable dtwhere = defmgr.getWhereDt(this.ID);
                if (dtwhere.Rows.Count > 0)
                {
                    for (int i = 0; i < dtwhere.Rows.Count; i++)
                    {
                        vsParam += "^" + dtwhere.Rows[i]["PARAMDEF_NAME"].ToString();
                        vsValue += "^";

                    }
                    vsParam = vsParam.Substring(1);
                    vsValue = vsValue.Substring(1);
                }
                string proc = ety.JTPUBQRDEF_SQL;
                if (mgr.getDBType(ety.JTPUBQRDEF_DBSRC) == "ORA")
                {
                    proc = ety.JTPUBQRDEF_ORA;
                }
                frm.showProc(ety.JTPUBQRDEF_DBSRC, " ", " ", "", this.ID, "", "2", "2", "", proc, ety.JTPUBQRDEF_SUBTIL, "0", vsParam, vsValue);
            }

        }


    }


}
