using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelClient
{

    public class BASE64
    {

        public static string EnCode(string SourceString)
        {
            return EnCode(SourceString, "utf-8");
        }
        /// <summary>
        /// 将字符串使用base64算法解密
        /// </summary>
        /// <param name="Base64String">加密的字符串</param>
        /// <param name="Encode">编码格式 gb2312 or utf-8
        /// <returns>解密的字符串</returns>
        public static string DeCode(string Base64String)
        {
            return DeCode(Base64String, "utf-8");
        }
        /// <summary>
        /// 将字符串使用base64算法加密
        /// </summary>
        /// <param name="SourceString">待加密的字符串</param>
        /// <param name="Encode">编码格式 gb2312 or utf-8
        /// <returns>编码后的文本字符串</returns>

        public static string EnCode(string SourceString, string Encode)
        {
            System.Text.Encoding ens = System.Text.Encoding.GetEncoding(Encode);
            return Convert.ToBase64String(ens.GetBytes(SourceString)); ;
        }
        /// <summary>
        /// 将字符串使用base64算法解密
        /// </summary>
        /// <param name="Base64String">加密的字符串</param>
        /// <param name="Encode">编码格式 gb2312 or utf-8
        /// <returns>解密的字符串</returns>
        public static string DeCode(string Base64String, string Encode)
        {
            System.Text.Encoding ens = System.Text.Encoding.GetEncoding(Encode);
            return ens.GetString((Convert.FromBase64String(Base64String)));
        }
    }


}
