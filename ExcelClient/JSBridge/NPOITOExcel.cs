using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
namespace ExcelClient.JSBridge
{
    public class NPOITOExcel
    {

        public NPOITOExcel()
        {
        }
        IWorkbook ebook;
        int rowi = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsTitle">0 第一列为列名，1第二列为显示名,2第三列宽度,3第四列，格式方式</param>
        /// <param name="dsData"></param>
        /// <returns></returns>
        public byte[] tOExcel(DataTable dsTitle, DataTable dsData)
        {
            try
            {
                MemoryStream ms = new MemoryStream();    //创建内存流用于写入文件
                //System.IO.Stream sr = new System.IO.MemoryStream();
                ebook = new HSSFWorkbook();
                ISheet esheet = ebook.CreateSheet("sheet1");

                //POI操作Excel中，导出的数据不是很大时，则不会有问题，而数据很多或者比较多时， 
                //就会报以下的错误，是由于cell styles太多create造成，故一般可以把cellstyle设置放到循环外面
                //http://blog.csdn.net/johnstrive/article/details/8568113
                //设置单元格格式
                NPOI.HSSF.UserModel.HSSFCellStyle dstyle = (HSSFCellStyle)ebook.CreateCellStyle();
                NPOI.HSSF.UserModel.HSSFDataFormat dformat = (HSSFDataFormat)ebook.CreateDataFormat();
                dstyle.DataFormat = dformat.GetFormat("yyyy-mm-dd");
                //
                NPOI.HSSF.UserModel.HSSFCellStyle nstyle = (HSSFCellStyle)ebook.CreateCellStyle();
                NPOI.HSSF.UserModel.HSSFDataFormat nformat = (HSSFDataFormat)ebook.CreateDataFormat();

                nstyle.DataFormat = nformat.GetFormat("#,##0.00");
                nstyle.Alignment = HorizontalAlignment.Right;
                // ebook.Add(esheet);
                if (dsTitle.Rows.Count > 0)
                {
                    IRow erow = esheet.CreateRow(rowi);

                    int i = 0;
                    foreach (DataRow row in dsTitle.Rows)
                    {
                        ICell ecell = erow.CreateCell(i, CellType.String);
                        esheet.SetColumnWidth(i, 256 * (row[2] == DBNull.Value ? 80 : Convert.ToInt32(row[2])) / 10);
                        ecell.SetCellValue(row[1].ToString());
                        // erow.Cells.Add(ecell);
                        i++;
                    }
                    //加载数据
                    if (dsData.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsData.Rows)
                        {
                            //创建新行
                            rowi++;
                            IRow derow = esheet.CreateRow(rowi);
                            //填数据
                            for (int cj = 0; cj < dsTitle.Rows.Count; cj++)
                            {
                                DataRow trow = dsTitle.Rows[cj];
                                string colname = trow[0].ToString();
                                CellType vtype = getCellType(dsData.Columns[colname], colname);


                                ICell ecell = derow.CreateCell(cj, vtype);
                                switch (vtype)
                                {
                                    case CellType.Blank:
                                        if (row[colname] != DBNull.Value)
                                        {
                                            if (Convert.ToString(row[colname]) != "")
                                            {
                                                ecell.SetCellValue(Convert.ToDateTime(row[colname].ToString()));
                                            }

                                        }

                                        ecell.CellStyle = dstyle;
                                        break;
                                    case CellType.Numeric:
                                        if (row[colname] != DBNull.Value)
                                        {
                                            ecell.SetCellValue(Convert.ToDouble(row[colname]));
                                        }
                                        ecell.CellStyle = nstyle;
                                        break;
                                    case CellType.String:
                                        ecell.SetCellValue(Convert.ToString(row[colname]));
                                        break;


                                }
                                //erow.Cells.Add(ecell);
                            }

                        }
                    }

                }
                ebook.Write(ms);
                long mssize = ms.Length;
                byte[] msBuffer = ms.ToArray();
                ms.Write(msBuffer, 0, (int)mssize);
                ms.Seek(0, SeekOrigin.Begin);
                ms.Close();
                return msBuffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //erow.CreateCell(

            //return sr;
        }

        Hashtable htTitle = new Hashtable();
        private CellType getCellType(DataColumn col, string colname)
        {
            if (htTitle.ContainsKey(colname))
                return (CellType)htTitle[colname];
            CellType vstype = CellType.String;
            switch (col.DataType.ToString())
            {
                case "System.Int32":
                    vstype = CellType.Numeric;
                    break;
                case "System.DateTime"://blank 用作datatime
                    vstype = CellType.Blank;
                    break;
                case "System.Double":
                    vstype = CellType.Numeric;
                    break;
                case "System.Decimal":
                    vstype = CellType.Numeric;
                    break;
                case "System.Char":
                    vstype = CellType.String;
                    break;
                case "System.String":
                    vstype = CellType.String;
                    break;
            }
            htTitle[colname] = vstype;
            return vstype;
        }

    }
}
