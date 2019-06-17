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
    public partial class frmDefEdit : DevExpress.XtraEditors.XtraForm
    {
        public frmDefEdit()
        {
            InitializeComponent();
        }

        ExcelClient.WsGetDataClient.WSGetData mgr  ;
        public string ProcessID = DevQryPubFun.GSYDBSrc;
        private DataTable DtWhere = new DataTable();
        public string ID = "";
        JTPUBQRDEFMgr defMgr;
        JTPUBQRDEFEty ety=new JTPUBQRDEFEty();

        public WsGetDataClient.WSGetData WSMgr
        {
            set { this.mgr = value; }
        } 

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!getCtrlValue())
                return  ;
            defMgr.Save(ety, DtWhere);

            MessageBox.Show("保存成功");
            DtWhere = defMgr.getWhereDt(ety.JTPUBQRDEF_ID);
            gridControl1.DataSource = DtWhere.DefaultView;
            btnParamEnabled();
            // string sql = insertDefineParam();
            // if (!string.IsNullOrEmpty(sql))
            // {
            //    WebSvrGetData.execsql(this.ProcessID, sql, mgr);
            //}
        }

        private void btnColse_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmDefEdit_Load(object sender, EventArgs e)
        {
            DataTable dtsrc = mgr.getDataSet(DevQryPubFun.GSYDBSrc, "select * from GSYDTSRC").Tables[0];
            foreach (DataRow row in dtsrc.Rows)
            {
                JTPUBQRDEF_DBSRC.Properties.Items.Add(row["GSYDTSRC_ID"].ToString());

            }  
            JTPUBQRDEF_DBSRC.Properties.Items.Add(DevQryPubFun.GSYDBSrc);
            defMgr = new JTPUBQRDEFMgr(mgr, this.ProcessID);
            if (!string.IsNullOrEmpty(ID))
            {
                ety = defMgr.getEty(ID);
                setCtrlValue();
            } 
            DtWhere =defMgr.getWhereDt(ID);
            gridControl1.DataSource = DtWhere.DefaultView;
            btnParamEnabled();
        }
        private void setCtrlValue()
        { 
            JTPUBQRDEF_BH.Text = ety.JTPUBQRDEF_BH;
            JTPUBQRDEF_MC.Text = ety.JTPUBQRDEF_MC;
            JTPUBQRDEF_TITLE.Text = ety.JTPUBQRDEF_TITLE;
            JTPUBQRDEF_SUBTIL.Text = ety.JTPUBQRDEF_SUBTIL;
            JTPUBQRDEF_TYPE.Text = ety.JTPUBQRDEF_TYPE;
            JTPUBQRDEF_DBSRC.Text = ety.JTPUBQRDEF_DBSRC;
            JTPUBQRDEF_SQL.Text = ety.JTPUBQRDEF_SQL;
            JTPUBQRDEF_ORA.Text = ety.JTPUBQRDEF_ORA;  
            JTPUBQRDEF_WHERE.Text = ety.JTPUBQRDEF_WHERE;
        }

        private bool getCtrlValue()
        {
            bool bl = true;
            if (JTPUBQRDEF_BH.Text == "")
            {
                bl = false;
                MessageBox.Show("编号不能为空！");
            }
            if (JTPUBQRDEF_MC.Text == "")
            {
                bl = false;
                MessageBox.Show("名称不能为空！");
            }
            if (JTPUBQRDEF_TITLE.Text == "")
            {
                bl = false;
                MessageBox.Show("标题不能为空！");
            }
            if (JTPUBQRDEF_SUBTIL.Text == "")
            {
                bl = false;
                MessageBox.Show("副标题不能为空！");
            }
            if (JTPUBQRDEF_DBSRC.Text == "")
            {
                bl = false;
                MessageBox.Show("数据源不能为空！");
            } 
            if (JTPUBQRDEF_SQL.Text == "")
            {
                bl = false;
                MessageBox.Show("SQL不能为空！");
            }
            if (JTPUBQRDEF_ORA.Text == "")
            {
                bl = false;
                MessageBox.Show("Oracle sql不能为空！");
            } 
            ety.JTPUBQRDEF_BH = JTPUBQRDEF_BH.Text;
            ety.JTPUBQRDEF_MC = JTPUBQRDEF_MC.Text;
            ety.JTPUBQRDEF_TITLE = JTPUBQRDEF_TITLE.Text;
            ety.JTPUBQRDEF_SUBTIL = JTPUBQRDEF_SUBTIL.Text;
            ety.JTPUBQRDEF_TYPE = JTPUBQRDEF_TYPE.Text;
            ety.JTPUBQRDEF_DBSRC = JTPUBQRDEF_DBSRC.Text;
            ety.JTPUBQRDEF_SQL = JTPUBQRDEF_SQL.Text;
            ety.JTPUBQRDEF_ORA = JTPUBQRDEF_ORA.Text; 
            ety.JTPUBQRDEF_WHERE = JTPUBQRDEF_WHERE.Text;

           // gridControl1.EndUpdate();

            gridView1.UpdateCurrentRow();
             
            return bl;
 
        }

        private void btnParamEnabled()
        {
            if (string.IsNullOrEmpty(ety.JTPUBQRDEF_ID))
            {
                btnParam.Enabled = false;
            }
            else
            {
                btnParam.Enabled = true;
            }
        }

        private void JTPUBQRDEF_TYPE_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (JTPUBQRDEF_TYPE.SelectedIndex == 0)
            {
                lblcs.Text = "字段：";            
            }
            else
            {
                lblcs.Text = "参数：";            
            }
        }

        private void btnaddField_Click(object sender, EventArgs e)
        {
            DataRow row = DtWhere.NewRow();
            row["PARAMDEF_ORD"] = (DtWhere.Rows.Count + 1 + "").PadLeft(3, '0');
            DtWhere.Rows.Add(row);
        }

        private void btndelfield_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle >= 0)
            {

              //  if (MessageBox.Show("确实要删除吗？", "删除确认", MessageBoxButtons.YesNo) == DialogResult.No) return;
               // DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                
                DtWhere.Rows.RemoveAt(gridView1.FocusedRowHandle);
            }
        }

        private void btnParam_Click(object sender, EventArgs e)
        {
            string sql = insertDefineParam();
            if (!string.IsNullOrEmpty(sql))
            {
                WebSvrGetData.execsql(this.ProcessID, sql, mgr);
                frmDefIndex frmind = new frmDefIndex();
                frmind.ID = ety.JTPUBQRDEF_ID;
                frmind.WSMgr = mgr;
                frmind.ShowDialog();
            }
            else
            {
                MessageBox.Show("没有定义参数！");
            }

        }


        private string insertDefineParam()
        {
            StringBuilder sb = new StringBuilder();
          

            sb.AppendFormat("delete from EACUSTOMFIELDS where CLASSSETCODE='{0}' and FIELDNAME not in (select PARAMDEF_NAME from JTPUBQRPARAMDEF where PARAMDEF_QRYID='{0}');", ety.JTPUBQRDEF_ID);
            foreach (DataRow row in DtWhere.Rows)
            {  
                int i = Convert.ToInt32(row["PARAMDEF_ORD"]);
                sb.Append(insertCUSTOMFIELDSSQL(ety.JTPUBQRDEF_ID, row["PARAMDEF_NAME"].ToString(), i));
            }

            string sql = sb.ToString();
            if (!string.IsNullOrEmpty(sql))
                sql = "begin  " + sql + " end;";
            return sql;
        }
        private string insertCUSTOMFIELDSSQL(string classsetcode, string fieldName,int ord )
        {
           string sql= string.Format(@"INSERT INTO EACUSTOMFIELDS (CLASSSETCODE,ACTTABLE, FIELDNAME ,DISPLAYNAME,INPUTTYPE,ISDISPLAY,GETINFOFROM,GETINFOWHERE,ISREQUIRED,DEFAULTVALUE,DISPLAYORDER) select
 '{0}','QRYINDEX','{1}',' ','0','1','','','0','',{2} from dual where NOT EXISTS(SELECT 1 FROM EACUSTOMFIELDS WHERE  CLASSSETCODE='{0}' AND  FIELDNAME='{1}') ;", classsetcode, fieldName, ord + "");
           string dbtype = mgr.getDBType(this.ProcessID);
         
           if (dbtype != "ORA")
               sql = sql.Replace("from dual ", " ");
            
           return sql;
        }
      



    }


}
