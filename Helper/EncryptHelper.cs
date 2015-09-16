#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：EncryptHelper
* Copyright(c) 青之软件
* CLR 版本: 4.0.30319.17929
* 创 建 人：周浩
* 电子邮箱：admin@itdos.com
* 创建日期：2014/10/1 11:00:49
* 文件描述：
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace Dos.Common
{
    public class EncryptHelper
    {
        public static string Key = "itdoscom";  //密钥
        public static string Iv = "itdosnet";   //向量

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string DESEncode(string encryptString)
        {
            try
            {
                var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                var des = new DESCryptoServiceProvider();
                des.Key = Encoding.ASCII.GetBytes(Key);
                des.Mode = CipherMode.ECB;
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, des.CreateEncryptor(), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }
        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DESDecode(string decryptString)
        {
            try
            {
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                var des = new DESCryptoServiceProvider();
                des.Key = Encoding.ASCII.GetBytes(Key);
                des.Mode = CipherMode.ECB;
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, des.CreateDecryptor(), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
        /// <summary>
        /// MD5加密，返回MD5 16位或32位加密后的字符串，默认返回16位。code输入16或32
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="code">MD5返回16位还是32位？请输入16或32</param>
        public static string MD5Encrypt(string str, int code)
        {
            if (code == 16) //16位MD5加密（取32位加密的9~25字符）
            {
                return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 16);
            }
            else if (code == 32) //32位加密  
            {
                return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();
            }
            else
            {
                return "0000000000000000";
            }
        }
        /// <summary>
        /// MD5加密，返回MD5 16位或32位加密后的字符串，默认返回16位。
        /// </summary>
        /// <param name="str">原始字符串</param>
        public static string MD5Encrypt(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 16);
        }
    }
}
