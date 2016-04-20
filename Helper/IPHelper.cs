#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：IPHelper
* Copyright(c) www.ITdos.com
* CLR 版本: 4.0.30319.17929
* 创 建 人：周浩
* 电子邮箱：admin@itdos.com
* 创建日期：2014/10/24 9:46:55
* 文件描述：
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Dos.Common
{
    /// <summary>
    /// IP帮助类
    /// </summary>
    public class IPHelper
    {
        /// <summary>
        /// 获取访问者IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetVisitorIP()
        {
            var result = "";
            try
            {
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                //可能有代理 
                if (!string.IsNullOrWhiteSpace(result))
                {
                    //没有"." 肯定是非IP格式
                    if (result.IndexOf(".", System.StringComparison.Ordinal) == -1)
                        result = "";
                    else
                    {
                        //有","，估计多个代理。取第一个不是内网的IP。
                        if (result.IndexOf(",", System.StringComparison.Ordinal) != -1)
                        {
                            result = result.Replace(" ", string.Empty).Replace("\"", string.Empty);
                            var temparyip = result.Split(",;".ToCharArray());
                            if (temparyip.Length > 0)
                            {
                                foreach (var t in temparyip.Where(t => IsIPAddress(t)
                                        && t.Substring(0, 3) != "10."
                                        && t.Substring(0, 7) != "192.168"
                                        && t.Substring(0, 7) != "172.16."))
                                {
                                    return t == "::1" ? "127.0.0.1" : t;
                                }
                            }
                        }
                        //代理即是IP格式
                        else if (IsIPAddress(result))
                            return result == "::1" ? "127.0.0.1" : result;
                        //代理中的内容非IP
                        else
                            result = "";
                    }
                }
                if (string.IsNullOrWhiteSpace(result))
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if (string.IsNullOrWhiteSpace(result))
                    result = HttpContext.Current.Request.UserHostAddress;
                return result == "::1" ? "127.0.0.1" : result;
            }
            catch (Exception)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(result))
                        result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    if (string.IsNullOrWhiteSpace(result))
                        result = HttpContext.Current.Request.UserHostAddress;
                    return result == "::1" ? "127.0.0.1" : result; 
                }
                catch (Exception ex)
                {
                    return "获取IP失败:" + ex.Message;
                }
            }
        }
        private static bool IsIPAddress(string str)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str) || str.Length < 7 || str.Length > 15)
                    return false;
                const string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}{1}";
                var regex = new Regex(regformat, RegexOptions.IgnoreCase);
                return regex.IsMatch(str);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
