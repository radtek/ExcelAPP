 

using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelDataBase
{
  
    
    public class DbProperties
    {
        /// <summary>
        /// δ��ȷ ����
        /// </summary>
        public static readonly DbProperties Unknown = new DbProperties(
            "Unknown", "Unknown","0.0",  "Unknown", "Unknown",
             true, "Unknown", "Unknown", "Unknown", "Unknown", "Unknown", "Unknown", "Unknown", "Unknown", "Unknown" 
           );

        //
        // ����
        //
 

        private string _edition;
        /// <summary>
        /// ���ݿ�汾
        /// </summary>
        public string ServerEdition
        {
            get { return _edition; }
            set{this._edition=value;}
        }

   

        #region public string ProductLevel

        private string _productlevel;

        /// <summary>
        /// �汾���� rtm? beta?
        /// </summary>
        public string ProductLevel
        {
            get { return _productlevel; }
        }

        #endregion

        #region public string ProductName {get;}

        private string _name;

        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public string ProductName
        {
            get { return _name; }
        }

        #endregion

        #region public Version ProductVersion

        private string _vers;

        /// <summary>
        /// �汾��
        /// </summary>
        public string ProductVersion
        {
            get { return _vers; }
        }

        #endregion

    
       

        #region public bool CaseSensitiveNames {get; protected set;}

        private bool _casesensitive;

        /// <summary>
        /// �Ƿ����ִ�Сд
        /// </summary>
        /// <remarks></remarks>
        public bool CaseSensitiveNames
        {
            get { return _casesensitive; }
            protected set { _casesensitive = value; }
        }
        /// <summary>
        /// sql����������
        /// </summary> 
        public string SqlToken
        {
            get;
            set;
        }
         /// <summary>
        /// �ַ����ӷ���
        /// </summary>
        private string _strjoinchar;
        public string StrJoinChar
        {
            get { return this._strjoinchar; }
            set { this._strjoinchar = value; }
        }
        /// <summary>
        /// ���ָ����
        /// </summary>
        private string _pkgSeparator;
        public string PKGSeparator
        {
            get { return this._pkgSeparator; }
            set { this._pkgSeparator = value; }
        }
        /// <summary>
        /// ת����ʱ��ĺ�������Ҫ����������cast('{0}' as datetime)
        /// to_date('{0}','{1}')
        /// </summary>
        public string ToDateFunc
        {
            set;
            get;
        }
        /// <summary>
        /// ��ȡguid�ĺ���
        /// </summary>
        public string GuidFunc
        {
            set;
            get;
        }
        /// <summary>
        /// ��ȡ���ݿ����ڵĺ���
        /// </summary>
        public string SysNowDateFunc
        {
            set;
            get;
        }
        /// <summary>
        /// ��ȡ�Ӵ��ĺ���
        /// </summary>
        public string SubStringFunc
        {
            set;
            get;
        }
        /// <summary>
        /// ��ȡ�Ƿ�Ϊ���жϵĺ���
        /// </summary>
        public string IsnullFunc
        {
            set;
            get;
        }
        /// <summary>
        /// ȡ�ַ������ȵĺ���
        /// </summary>
        public string StringLengthFunc
        {
            set;
            get;
        }

        #endregion

       

        //
        //ʵ��
        //

        #region internal protected DBDatabaseProperties(...)

        /// <summary>
        /// Creates a new instance of the DBDatabaseProperties
        /// </summary>
        
        internal protected DbProperties(string databaseName,
            string productName, 
            string version,  
            string productLevel, 
            string serverEdition,   
            bool caseSensitive, 
            string strJoinChar,
            string sqlToken,
            string PkgSeparator,
            string ToDateFunc,
            string GuidFunc,
            string SysNowDateFunc,
            string SubStringFunc,
            string IsnullFunc,
            string StringLengthFunc
            )
        {
            this._vers = version;
            this._edition = serverEdition;
            this._productlevel = productLevel;
            this._name = productName; 
            this._casesensitive = caseSensitive;            
            this._strjoinchar = strJoinChar;
            this.SqlToken = sqlToken;
            this._pkgSeparator = PkgSeparator;
            this.ToDateFunc = ToDateFunc;
            this.GuidFunc = GuidFunc;
            this.SysNowDateFunc = SysNowDateFunc;
            this.SubStringFunc = SubStringFunc;
            this.IsnullFunc = IsnullFunc;
            this.StringLengthFunc = StringLengthFunc;
        }

        #endregion

        

        #region public override string ToString() + 1 overload

        /// <summary>
        /// Builds a summary string of the properties of this database
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DBDatabaseProperties {");
            this.ToString(sb);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Appends key properties and their values to the StringBuilder. 
        /// Inheritors can override this method to append other properties
        /// </summary>
        /// <param name="sb"></param>
        protected virtual void ToString(StringBuilder sb)
        {
            sb.Append("product:");
            sb.Append(this.ProductName);
            sb.Append(", level:");
            sb.Append(this.ProductLevel);
            sb.Append(", edition:");
            sb.Append(this.ServerEdition);
            sb.Append(", version:");
            sb.Append(this.ProductVersion);
        }

        #endregion

    }
}
