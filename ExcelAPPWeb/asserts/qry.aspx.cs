using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using ExcelAPPWeb.Query;
using NPoco;
using ExcelAPPWeb;
using System.Web.UI.WebControls;
using ExcelAPPWeb.Service;

namespace ExcelAPPWeb.asserts
{
    public partial class qry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var qryid = Request.QueryString.Get("id");
                var model = new JTPUBQRDEF();

                Database Db = new Database("DataPlatformDB");
                model = Db.Fetch<JTPUBQRDEF>(new Sql("select * from JTPUBQRDEF where JTPUBQRDEF_ID=@0", qryid)).FirstOrDefault();
                model.filter = Db.Fetch<QueryFiter>(new Sql("select a.*,b.PARAMDEF_CMP CMP from EACustomFields a left join JTPUBQRPARAMDEF b on a.ClassSetCode=b.PARAMDEF_QRYID AND a.FieldName=b.PARAMDEF_NAME where a.ClassSetCode=@0 order by DisplayOrder asc", qryid));
                txt_query.Value = Newtonsoft.Json.JsonConvert.SerializeObject(model);

                txtCurDate.Value = DateTime.Now.ToString("yyyy-MM-dd");
                var user = UserService.GetUser();
                txtUserID.Value = user.Id;
                txtUserCode.Value = user.Code;
                txtUserName.Value = user.Name;
                txtDWBH.Value = UserService.GetGsdwh();
                txtDWMC.Value = UserService.GetGSDWMC();

            }
        }
    }
}