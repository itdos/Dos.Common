#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：Cookie
* Copyright(c) www.ITdos.com
* CLR 版本: 4.0.30319.239
* 创 建 人：周浩
* 电子邮箱：admin@itdos.com
* 创建日期：2012/1/30 星期一 16:15:22
* 文件描述：
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Dos.Common
{
    /// <summary>
    /// Cookie操作类
    /// </summary>
    public class CookieHelper
    {
        private const string defaultCookieName = "itdos";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookieName"></param>
        public static void Remove(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.AppendCookie(cookie);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void RemoveDefault()
        {
            Remove("itdos");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static string Get(string cookieName)
        {
            return Get(null, cookieName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fatherCookieName"></param>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static string Get(string fatherCookieName, string cookieName)
        {
            HttpCookie cookie;
            string cookieValue = string.Empty;
            if (string.IsNullOrEmpty(fatherCookieName))
            {
                cookie = HttpContext.Current.Request.Cookies[cookieName];
                if (cookie != null)
                {
                    cookieValue = HttpUtility.UrlDecode(cookie.Value);
                }
                return cookieValue;
            }
            cookie = HttpContext.Current.Request.Cookies[fatherCookieName];
            if (cookie != null)
            {
                cookieValue = HttpUtility.UrlDecode(cookie.Values[cookieName]);
            }
            return cookieValue;
        }

        public static string GetDefaul(string cookieName)
        {
            return Get("itdos", cookieName);
        }

        public static void Set(string cookieName, string cookieValue)
        {
            Set(cookieName, cookieValue, (int?)null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        /// <param name="expires">设置cookie过期时间，单位：秒。</param>
        public static void Set(string cookieName, string cookieValue, int? expires)
        {
            Set(cookieName, cookieValue, expires, null, null, null, null);
        }

        public static void Set(string fatherCookieName, string cookieName, string cookieValue)
        {
            Set(fatherCookieName, cookieName, cookieValue, null);
        }

        public static void Set(string fatherCookieName, string cookieName, string cookieValue, int? expires)
        {
            Set(fatherCookieName, cookieName, cookieValue, expires, null, null, null, null);
        }

        public static void Set(string cookieName, string cookieValue, int? expires, string domain, bool? httpOnly, string path, bool? secure)
        {
            Set(null, cookieName, cookieValue, expires, domain, httpOnly, path, secure);
        }

        public static void Set(string fatherCookieName, string cookieName, string cookieValue, int? expires, string domain, bool? httpOnly, string path, bool? secure)
        {
            HttpCookie cookie;
            if (string.IsNullOrEmpty(fatherCookieName))
            {
                cookie = HttpContext.Current.Request.Cookies[cookieName];
                if (cookie == null)
                {
                    cookie = new HttpCookie(cookieName);
                }
                cookie.Value = HttpUtility.UrlEncode(cookieValue);
            }
            else
            {
                cookie = HttpContext.Current.Request.Cookies[fatherCookieName];
                if (cookie == null)
                {
                    cookie = new HttpCookie(fatherCookieName);
                }
                cookie.Values[cookieName] = HttpUtility.UrlEncode(cookieValue);
            }
            if (expires.HasValue)
            {
                cookie.Expires = DateTime.Now.AddSeconds((double)expires.Value);
            }
            if (!string.IsNullOrEmpty(domain))
            {
                cookie.Domain = domain;
            }
            if (httpOnly.HasValue)
            {
                cookie.HttpOnly = httpOnly.Value;
            }
            if (!string.IsNullOrEmpty(path))
            {
                cookie.Path = path;
            }
            if (secure.HasValue)
            {
                cookie.Secure = secure.Value;
            }
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static void SetDefault(string cookieName, string cookieValue)
        {
            SetDefault(cookieName, cookieValue, null);
        }

        public static void SetDefault(string cookieName, string cookieValue, int? expires)
        {
            SetDefault(cookieName, cookieValue, expires, null, null, null, null);
        }

        public static void SetDefault(string cookieName, string cookieValue, int? expires, string domain, bool? httpOnly, string path, bool? secure)
        {
            Set("itdos", cookieName, cookieValue, expires, domain, httpOnly, path, secure);
        }
    }
}
