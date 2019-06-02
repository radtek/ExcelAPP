using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ExcelDataBase
{
    /// <summary>
    /// 定义sql或存储过程参数
    /// </summary>  
  public class DbParam
    {
        //
        //string paramname, DataType datatype, ParameterDirection paramdirct, int size, object paramvalue
        //

        /// <summary>
        /// 参数名
        /// </summary>
        public string paramName { set; get; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType dataType { set; get; }

        /// <summary>
        /// 参数方向
        /// </summary>
        public ParameterDirection paramDirct { set; get; }


        public SqlType sqlType { set; get; }
        /// <summary>
        /// 参数长度
        /// </summary>
        public int Size { set; get; }

        /// <summary>
        /// 参数值
        /// </summary>
        public object paramValue { set; get; } 

        public DbParam() {             
            
        }
        public DbParam(string paramname, DataType datatype, ParameterDirection paramdirct,SqlType sqltype, int size, object paramvalue)
        {
            this.paramName = paramname;
            this.dataType = datatype;
            this.paramDirct = paramdirct;
            this.sqlType = sqltype;
            this.Size = size;
            this.paramValue = paramvalue;
        }
    }

    /// <summary>
    /// sql 或存储过程定义
    /// </summary>
    public class DbText
    {
        private CommandType _type = CommandType.Text;
        private DbParam[] _param = null;

        /// <summary>
        /// sql 或 存储过程名
        /// </summary>
        public string Text { set; get; }

        /// <summary>
        /// 执行类型
        /// </summary>
        public CommandType Type 
        { 
            set{this._type=value;}
            get { return this._type; }
        }

        /// <summary>
        /// 执行参数
        /// </summary>
        public DbParam[] param
        {
            set { this._param = value; }
            get { return this._param; }
        }

        public DbText()
        {
        }

        public DbText(string text,CommandType type,DbParam[] psParam)
        {
            this.Text = text;
            this.Type = type;
            this.param = psParam;
        }
    }


}