#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) www.ITdos.com
* CLR 版本: 4.0.30319.17929
* 创 建 人：ITdos
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
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Dos.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpParam
    {
        /// <summary>
        /// GET/POST
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Post参数。
        /// <para>可以传入Json对像：new { Key1 = Value1, Key2 = Value2}</para>
        /// <para>可以传入Json字符串：{"Key1":"Value1","Key2":"Value2"}</para>
        /// <para>可以传入key/value字符串："ke=value＆key=value"</para>
        /// <para>可以传入xml字符串等等</para>
        /// </summary>
        public object PostParam { get; set; }
        /// <summary>
        /// Get参数
        /// <para>可以传入Json对像：new { Key1 = Value1, Key2 = Value2}</para>
        /// <para>可以传入Json字符串：{"Key1":"Value1","Key2":"Value2"}</para>
        /// </summary>
        public object GetParam { get; set; }

        private int _timeOut = 5;
        /// <summary>
        /// 请求超时时间。单位：秒。默认值5秒。
        /// </summary>
        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }
        private Encoding _encoding = Encoding.UTF8;
        /// <summary>
        /// 
        /// </summary>
        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public CookieContainer CookieContainer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CertPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CertPwd { get; set; }
        private string _contentType = "application/x-www-form-urlencoded";
        /// <summary>
        /// 
        /// </summary>
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }
        private string _userAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Maxthon/4.1.2.4000 Chrome/26.0.1410.43 Safari/537.1";
        /// <summary>
        /// 
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public HttpPostedFileBase PostedFile { get; set; }
    }
}
