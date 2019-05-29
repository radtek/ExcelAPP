using System;
using System.Collections.Generic;
using System.Web;

namespace ExcelAPPWeb
{
    public class JTGLPubQryFavMgr
    {
        public JTGLPubQryFavMgr()
        {
        }
        /*SELECT QRYFAV_QRYID,QRYFAV_HASH,QRYFAV_NAME,QRYFAV_LOCATION,QRYFAV_CREATER,QRYFAV_ROW,QRYFAV_COL,QRYFAV_DATA,QRYFAV_FILTER,QRYFAV_SEL,QRYFAV_DEFAULT
 FROM JTPUBQRYFAV*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="QryID">查询id</param>
        /// <param name="strFields">查询结果集所有字段用','连接的字符串</param>
        /// <param name="strName">收藏名称，对应一个QryID和strFields，唯一，不能重复</param>
        /// <param name="rowFields">行区字段，用','连接</param>
        /// <param name="colFields">列区字段，用','连接</param>
        /// <param name="dataFields">数据区字段，用','连接</param>
        /// <param name="filterFields">过滤区字段，用','连接</param>
        /// <param name="selectFields">待选区字段，用','连接</param>
        /// <param name="IsDefault">是否是默认</param>
        public static void savePubQryFav(string QryID,string strFields,string strName,string rowFields,string colFields,string dataFields,string filterFields,string selectFields,bool IsDefault)
        {
            string sql = "begin  ";
            string vsHash=ComputeHash.getHashString(strFields.Trim(','));

            sql = string.Format(" delete  from JTPUBQRYFAV where QRYFAV_QRYID='{0}' and QRYFAV_HASH='{1}' and QRYFAV_NAME='{2}';  ",
                QryID,vsHash,strName);
            sql += string.Format(@"insert into JTPUBQRYFAV (QRYFAV_QRYID,QRYFAV_HASH,QRYFAV_NAME,QRYFAV_LOCATION,
QRYFAV_CREATER,QRYFAV_ROW,QRYFAV_COL,QRYFAV_DATA,QRYFAV_FILTER,QRYFAV_SEL,QRYFAV_DEFAULT) values
('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');   ",
 QryID,vsHash,strName,"公有","",rowFields,colFields,dataFields,filterFields,selectFields,IsDefault?"1":"0");
            sql += "   end; ";
          //  DBFunction.RunSql(sql);
        }
    }
}
