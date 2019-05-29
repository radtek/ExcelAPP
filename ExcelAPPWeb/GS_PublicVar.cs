using System;
using System.Collections;
using System.Web;
using System.Web.SessionState;

namespace ExcelAPPWeb
{
	public class GS_PublicVar
	{
		//StaticHolderTypesShouldNotHaveConstructors.
		private GS_PublicVar(){}

		#region 保存在Process.ini文件中的GS公共变量
		public const string ProcessID		= "Genersoft.BS.Public.ProcessID";
		public const string AppInstanceID	= "Genersoft.BS.Public.AppInstanceID";
		public const string User			= "Genersoft.BS.Public.User";
		public const string UserName		= "Genersoft.BS.Public.UserName";
		public const string Respid			= "Genersoft.BS.Public.Respid";
		public const string RespName		= "Genersoft.BS.Public.RespName";
		public const string ModuleID		= "Genersoft.BS.Public.ModuleID";
		public const string CompanyID		= "Genersoft.BS.Public.CompanyID";
		public const string CompanyName		= "Genersoft.BS.Public.CompanyName";
		public const string FiscalYear		= "Genersoft.BS.Public.FiscalYear";
		public const string StationMsg		= "Genersoft.BS.Public.StationMsg";
		public const string AccountLimit	= "Genersoft.BS.Public.AccountLimit";
		public const string AccountSet		= "Genersoft.BS.Public.AccountSet";
		public const string FiscalCalendar	= "Genersoft.BS.Public.FiscalCalendar";
		public const string PeriodUnit		= "Genersoft.BS.Public.PeriodUnit";
		public const string LocalCurrency	= "Genersoft.BS.Public.LocalCurrency";
		public const string SubjectSeries	= "Genersoft.BS.Public.SubjectSeries";
		public const string SubjectStru		= "Genersoft.BS.Public.SubjectStru";
		public const string SCSeries		= "Genersoft.BS.Public.SCSeries";
		public const string ProductSeries	= "Genersoft.BS.Public.ProductSeries";
		public const string BalancePeriod	= "Genersoft.BS.Public.BalancePeriod";
		public const string BeginYearMonth	= "Genersoft.BS.Public.BeginYearMonth";
		public const string EndYear			= "Genersoft.BS.Public.EndYear";
		public const string Quantitydec		= "Genersoft.BS.Public.Quantitydec";
		public const string Pricedec		= "Genersoft.BS.Public.Pricedec";
		public const string Moneydec		= "Genersoft.BS.Public.Moneydec";
		public const string ExchangeRatedec	= "Genersoft.BS.Public.ExchangeRatedec";
		public const string Department		= "Genersoft.BS.Public.Department";
		public const string CurDate			= "Genersoft.BS.Public.CurDate";
		public const string FiscalPeriod	= "Genersoft.BS.Public.FiscalPeriod";
		public const string AreaID			= "Genersoft.BS.Public.AreaID";
		public const string Menu			= "Genersoft.BS.Public.Menu";
		public const string Login_Source	= "Genersoft.BS.Public.Login.Source";
		public const string Login_Catalog	= "Genersoft.BS.Public.Login.Catalog";
		public const string Login_UserID	= "Genersoft.BS.Public.Login.UserID";
		public const string Login_Provider	= "Genersoft.BS.Public.Login.Provider";
		public const string Login_Check		= "Genersoft.BS.Public.Login.Check";
		public const string DbType			= "Genersoft.BS.Public.DbType";
		public const string Oleconn			= "Genersoft.BS.Public.OleDbConnectionString";
		public const string Sqlconn			= "Genersoft.BS.Public.SqlClientConnectionString";
		public const string Oraconn			= "Genersoft.BS.Public.OracleClientConnectionString";
		public const string Db2conn			= "Genersoft.BS.Public.IBMDB2ClientConnectionString";
		#endregion

		/// <summary>
		/// 取公司参数
		/// </summary>
		/// <param name="ProcessID">进程号</param>
		/// <param name="psMKID">模块</param>
		/// <param name="psDWBH">单位</param>
		/// <param name="psKEY">键值</param>
		/// <returns>参数值</returns>
		public static string GetLSGSCS(string psMKID,string psDWBH,string psKEY)
		{
			try
			{
				string vsSql = string.Empty;
				if(psDWBH==null||psDWBH.Length==0)	psDWBH=" ";

                //vsSql = string.Format(System.Globalization.CultureInfo.CurrentCulture,"SELECT LSGSCS_VALUE FROM LSGSCS{0} WHERE LSGSCS_MKID='{1}' AND LSGSCS_HSDW='{2}' AND LSGSCS_KEY='{3}'",
                //                 GSIDP.GSYS.Session.GSIDPSession.Session["GENERSOFT.BS.PUBLIC.FISCALYEAR"].ToString(), psMKID, psDWBH, psKEY);
                //            return GSIDP.GSYS.Data.DbFunction.ExecuteScalar(vsSql).ToString();		
                return "";
			}
			catch
			{
				throw;
			}
		}
		
		 
        //public static string GenerateId()
        //{
        //    long i = 1;
        //    foreach (byte b in Guid.NewGuid().ToByteArray())
        //    {
        //        i *= ((int)b + 1);
        //    }
        //    return string.Format("{0:x}", i - DateTime.Now.Ticks);
        //}

        //public static long GenerateNumId()
        //{
        //    byte[] buffer = Guid.NewGuid().ToByteArray();
        //    return BitConverter.ToInt64(buffer, 0);
        //}
	}
}

