using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dos.Common.Helper;

namespace Dos.Common
{
    /// <summary>
    /// 通用扩展
    /// </summary>
    public static class CommonExpand
    {
        #region string
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DosTrim(this string str)
        {
            return str?.Trim() ?? "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="trimChars"></param>
        /// <returns></returns>
        public static string DosTrim(this string str, params char[] trimChars)
        {
            return str?.Trim(trimChars) ?? "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="trimChars"></param>
        /// <returns></returns>
        public static string DosTrimStart(this string str, params char[] trimChars)
        {
            return str?.TrimStart(trimChars) ?? "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="trimChars"></param>
        /// <returns></returns>
        public static string DosTrimEnd(this string str, params char[] trimChars)
        {
            return str?.TrimEnd(trimChars) ?? "";
        }
        /// <summary>
        /// 是否是Guid
        /// </summary>
        /// <Param name="key"></Param>
        /// <returns></returns>
        public static bool DosIsGuid(this string key)
        {
            Guid g;
            return Guid.TryParse(key, out g);
        }
        /// <summary>
        /// 获取字节数
        /// str：需要获取的字符串
        /// </summary>
        public static int DosLength(this string str)
        {
            return StringHelper.Length(str);
        }
        /// <summary>
        /// 按字节数截取指定字节
        /// </summary>
        /// <Param name="str">需要获取的字符串</Param>
        /// <Param name="length">获取的字节长度</Param>
        /// <returns></returns>
        public static string DosSubString(this string str, int length)
        {
            return StringHelper.SubString(str, length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool DosIsMobilePhone(this string str)
        {
            return RegexHelper.IsMobilePhone(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool DosIsEmail(this string email)
        {
            return RegexHelper.IsEmail(email);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public static bool DosIsIdCard(this string idCard)
        {
            return RegexHelper.IsIdCard(idCard);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool DosIsIP(this string ip)
        {
            return RegexHelper.IsIP(ip);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static bool DosIsUrl(this string strUrl)
        {
            return RegexHelper.IsUrl(strUrl);
        }

        ///  <summary>
        ///  去除HTML标记  
        ///  </summary>   
        ///  <param name="htmlString">包括HTML的源码</param>   
        ///  <returns>已经去除后的文字</returns>   
        public static string DosRemoveHtml(this string htmlString)
        {
            return RegexHelper.RemoveHtml(htmlString);
        }

        #endregion

        #region MemberInfo
        private static Dictionary<MemberInfo, Object> _micache1 = new Dictionary<MemberInfo, Object>();
        private static Dictionary<MemberInfo, Object> _micache2 = new Dictionary<MemberInfo, Object>();
        /// <summary>
        /// 获取自定义特性，带有缓存功能，避免因.Net内部GetCustomAttributes没有缓存而带来的损耗
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute[] DosGetCustomAttributes<TAttribute>(this MemberInfo member, Boolean inherit)
        {
            if (member == null) return new TAttribute[0];

            // 根据是否可继承，分属两个缓存集合
            var cache = inherit ? _micache1 : _micache2;

            object obj = null;
            if (cache.TryGetValue(member, out obj)) return (TAttribute[])obj;
            lock (cache)
            {
                if (cache.TryGetValue(member, out obj)) return (TAttribute[])obj;

                var atts = member.GetCustomAttributes(typeof(TAttribute), inherit) as TAttribute[];
                var att = atts ?? new TAttribute[0];
                cache[member] = att;
                return att;
            }
        }
        /// <summary>获取自定义属性</summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute DosGetCustomAttribute<TAttribute>(this MemberInfo member, Boolean inherit)
        {
            var atts = member.DosGetCustomAttributes<TAttribute>(inherit);
            if (atts == null || atts.Length < 1) return default(TAttribute);
            return atts[0];
        }
        #endregion
    }
}