using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;

using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Linq;

namespace ExcelClient.JSBridge
{
    public class NPOIData
    {


        #region 通用读取excel
        private DataTable getNPIOExcelData(string sheet, ref IWorkbook ebook, Stream file)
        {
            DataTable dt = new DataTable();
            if (ebook == null)
            {
                ebook = new HSSFWorkbook(file);
            }

            bool is2007 = false;
            if (ebook is NPOI.XSSF.UserModel.XSSFWorkbook) is2007 = true;

            string[] sheetarr = sheet.Split('^');
            ISheet esheet = ebook.GetSheet(sheetarr[0]);
            string rowcolinfo = sheetarr[1];
            if (rowcolinfo.IndexOf(":") != -1) //块读取
            {
                getDtByRange(dt, esheet, rowcolinfo, is2007);

            }
            else if (rowcolinfo.IndexOf("/") != -1) //挑选列读取
            {

                getDtBySelCol(dt, esheet, rowcolinfo, is2007);
            }
            else//按列读取
            {
                getDtByFields(dt, esheet, rowcolinfo, is2007);
            }
            return dt;
        }
        private void getDtByFields(DataTable dt, ISheet esheet, string rowcolinfo, bool is2007)
        {

            string[] colArr = rowcolinfo.Split('~');
            int scol = getIndexByColChar(colArr[0]);
            int ecol = getIndexByColChar(colArr[1]);
            int srow = Convert.ToInt32(colArr[2]) - 1;

            System.Collections.IEnumerator rows = esheet.GetRowEnumerator();

            for (int j = scol; j <= ecol; j++)
            {
                dt.Columns.Add(getColStrByIndex(j));
            }

            int rowindex = -1;
            while (rows.MoveNext())
            {
                rowindex += 1;
                if (rowindex < srow) continue;
                IRow row;
                if (is2007)
                {
                    row = (XSSFRow)rows.Current;
                }
                else
                {
                    row = (HSSFRow)rows.Current;
                }
                DataRow dr = dt.NewRow();
                string vsval = "";
                for (int i = scol; i <= ecol; i++)
                {
                    int drindex = i - scol;
                    ICell cell = row.GetCell(i);
                    if (cell == null)
                    {
                        dr[drindex] = null;
                    }
                    else
                    {
                        dr[drindex] = getCellValue(cell);
                        vsval += dr[drindex];
                    }
                }
                if (string.IsNullOrEmpty(vsval.Trim())) break;
                dt.Rows.Add(dr);

            }

        }
        private void getDtBySelCol(DataTable dt, ISheet esheet, string rowcolinfo, bool is2007)
        {

            string[] colArr = rowcolinfo.Split('~');
            string[] cols = colArr[0].Split('/');
            string[] rownum = colArr[1].Split('#');
            int[] colindex = new int[cols.Length];
            for (int i = 0; i < cols.Length; i++)
            {
                colindex[i] = getIndexByColChar(cols[i]);
            }


            int srow = Convert.ToInt32(rownum[0]) - 1;
            int erow = rownum.Length == 2 ? Convert.ToInt32(rownum[1]) - 1 : 0;

            System.Collections.IEnumerator rows = esheet.GetRowEnumerator();
            foreach (string str in cols)
            {
                dt.Columns.Add(str);
            }
            int rowindex = -1;
            while (rows.MoveNext())
            {
                rowindex += 1;
                if (rowindex < srow) continue;
                if (erow != 0 && rowindex > erow) break;
                IRow row;
                if (is2007)
                {
                    row = (XSSFRow)rows.Current;
                }
                else
                {
                    row = (HSSFRow)rows.Current;
                }
                DataRow dr = dt.NewRow();

                string vsval = "";
                for (int i = 0; i < colindex.Length; i++)
                {
                    ICell cell = row.GetCell(colindex[i]);
                    if (cell == null)
                    {
                        dr[i] = null;
                    }
                    else
                    {
                        dr[i] = getCellValue(cell);
                        vsval += dr[i];
                    }
                }
                if (erow == 0 && string.IsNullOrEmpty(vsval.Trim())) break;
                dt.Rows.Add(dr);

            }

        }
        private string getCellValue(ICell cell)
        {
            string vsre = "";
            try
            {

                if (cell.CellType.ToString() == "FORMULA")
                {

                    vsre = cell.NumericCellValue + "";
                }
                else if (CellType.Formula == cell.CellType)
                {
                    vsre = cell.NumericCellValue + "";

                }
                else
                {
                    vsre = cell.ToString();

                }
                if (string.IsNullOrEmpty(vsre))
                {

                    vsre = cell.StringCellValue;
                }
            }
            catch (Exception ex)
            {

            }
            return vsre;

        }
        private int getIndexByColChar(string colstr)
        {
            int len = 0;
            for (int i = colstr.Length - 1; i >= 0; i--)
            {
                int pow = Convert.ToInt32((Math.Pow((double)26, (double)(colstr.Length - i - 1))));
                len += (((int)colstr[i]) - ((int)'A') + 1) * pow;
            }
            return len - 1;
        }

        private string getColStrByIndex(int colnum)
        {
            string colstr = "";
            int col1 = colnum / 26;
            int col2 = colnum % 26;
            colstr = Convert.ToChar(col2 + ((int)'A')) + colstr;
            while (col1 > 26)
            {
                col1 = col1 / 26;
                col2 = col1 % 26;
                colstr = Convert.ToChar((col2 - 1) + ((int)'A')) + colstr;
            }
            if (col1 > 0) colstr = Convert.ToChar((col1 - 1) + ((int)'A')) + colstr;
            return colstr;
        }

        private void getDtByRange(DataTable dt, ISheet esheet, string rowcolinfo, bool is2007)
        {

            string[] colArr = rowcolinfo.Split(':');
            string scolchar = Regex.Match(colArr[0], "\\D+").Value;
            int scol = getIndexByColChar(scolchar);
            int srow = Convert.ToInt32(colArr[0].Replace(scolchar, "")) - 1;
            string ecolchar = Regex.Match(colArr[1], "\\D+").Value;
            int ecol = getIndexByColChar(ecolchar);
            int erow = Convert.ToInt32(colArr[1].Replace(ecolchar, "")) - 1;

            System.Collections.IEnumerator rows = esheet.GetRowEnumerator();

            for (int j = scol; j <= ecol; j++)
            {
                dt.Columns.Add(getColStrByIndex(j));
            }
            int rowindex = -1;
            while (rows.MoveNext())
            {
                rowindex += 1;
                if (rowindex < srow) continue;
                if (rowindex > erow) break;
                IRow row;
                if (is2007)
                {
                    row = (XSSFRow)rows.Current;
                }
                else
                {
                    row = (HSSFRow)rows.Current;
                }
                DataRow dr = dt.NewRow();
                string vsval = "";
                for (int i = scol; i <= ecol; i++)
                {
                    int drindex = i - scol;
                    ICell cell = row.GetCell(i);
                    if (cell == null)
                    {
                        dr[drindex] = null;
                    }
                    else
                    {

                        dr[drindex] = getCellValue(cell);
                    }
                }
                dt.Rows.Add(dr);

            }

        }
        #endregion

        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// 将 byte[] 转成 Stream

        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
        public List<ExportModel> getSheetInfo(Stream file, bool is2007, string start = "1", string endKeyWord = "")
        {

            List<ExportModel> list = new List<ExportModel>();
            IWorkbook workbook;
            byte[] data = StreamToBytes(file);
            int x = 0;

            if (is2007)
            {
                workbook = new XSSFWorkbook(file);
                x = workbook.NumberOfSheets;

            }
            else
            {
                workbook = new HSSFWorkbook(file);
                x = workbook.NumberOfSheets;
            }

            List<string> sheetNames = new List<string>();
            for (int i = 0; i < x; i++)
            {

                string importFormat = workbook.GetSheetName(i) + "^" + "A" + "~" + "CZ" + "~" + start;

                sheetNames.Add(workbook.GetSheetName(i));
                DataTable dt = this.getDataTable(importFormat, BytesToStream(data), is2007);
                DataTable dtcopy = dt.Copy();


                foreach (DataColumn col in dt.Columns)
                {
                    string colname = col.ColumnName;


                    var query = (from DataRow t in dt.Rows
                                 where t[colname].ToString() != ""
                                 select new { value = t[colname] });

                    if (query.ToList().Count == 0)
                    {
                        dtcopy.Columns.Remove(colname);
                    }
                    //if (colname != "AS" && colname != "OR")
                    //{
                    //    DataRow[] rowcol = dt.Select(" " + colname + " <>'' ");
                    //    if (rowcol.Length == 0)
                    //    {
                    //        dtcopy.Columns.Remove(colname);
                    //    }
                    //}

                }
                list.Add(new ExportModel { sheetName = workbook.GetSheetName(i), dt = dtcopy });


            }

            return list;
        }

        public DataTable getDataTable(string sheet, Stream file, bool is2007)
        {
            StringBuilder sb = new StringBuilder();

            DataTable dt = new DataTable();
            try
            {
                IWorkbook ebook = null;
                if (is2007)
                {
                    ebook = new XSSFWorkbook(file);


                }
                else
                {
                    ebook = new HSSFWorkbook(file);

                }
                dt = getNPIOExcelData(sheet, ref ebook, file);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

    }
}
