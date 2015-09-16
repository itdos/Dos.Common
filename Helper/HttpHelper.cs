#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：EncryptHelper
* Copyright(c) 青之软件
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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Dos.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpHelper
    {
        private static readonly string UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Maxthon/4.1.2.4000 Chrome/26.0.1410.43 Safari/537.1";
        private  const string ContentType = "application/x-www-form-urlencoded";
        private const int TimeOut = 5;
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        #region Get请求
        /// <summary>
        /// Get方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="queryString"></param>
        /// <param name="timeOut">单位：秒</param>
        /// <param name="encoding"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static Stream GetStream(string url, string queryString = null, int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            url = string.Format("{0}?{1}", url, queryString);
            var r = CreateRequest(url);
            r.Timeout = timeOut * 1000;
            r.UserAgent = UserAgent;
            r.Referer = refer;
            r.CookieContainer = cc;
            r.Method = "GET";
            return r.GetResponse().GetResponseStream();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="timeOut">单位：秒</param>
        /// <param name="encoding"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static Stream GetStream(string url, HttpParam param = null,
           int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            return GetStream(url, FormatData(param), timeOut, encoding, cc, refer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Get(string url)
        {
            return Get(url, "");
        }
        /// <summary>
        /// 
        /// </summary>
        public static T Get<T>(string url)
        {
            return Get<T>(url, "");
        }
        /// <summary>
        ///  Get方式获取字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="queryString"></param>
        /// <param name="timeOut">单位：秒</param>
        /// <param name="encoding"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string Get(string url, string queryString = null, int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null,
            string refer = null)
        {
            return new StreamReader(GetStream(url, queryString, timeOut, encoding, cc, refer)).ReadToEnd();
        }
        /// <summary>
        /// Get方式获取字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="timeOut">单位：秒</param>
        /// <param name="encoding"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string Get(string url, HttpParam param = null,
           int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            return Get(url, FormatData(param), timeOut, encoding, cc, refer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="timeOut">单位：秒</param>
        /// <param name="encoding"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static T Get<T>(string url, HttpParam param = null,
           int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            var str = Get(url, param, timeOut, encoding, cc, refer);
            return JsonConvert.DeserializeObject<T>(str);
            //return JsonHelper.Deserialize<T>(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="queryString"></param>
        /// <param name="timeOut">单位秒</param>
        /// <param name="encoding"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static T Get<T>(string url, string queryString = null, int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null,
            string refer = null)
        {
            var str = Get(url, queryString, timeOut, encoding, cc, refer);
            return JsonConvert.DeserializeObject<T>(str);
            //return JsonHelper.Deserialize<T>(str);
        }
        #endregion

        #region Post 请求
        /// <summary>
        /// Post方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="timeOut">单位：秒</param>
        /// <param name="encoding"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static Stream PostStream(string url, string param = null, int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            var r = CreateRequest(url);
            r.Timeout = timeOut * 1000;
            r.UserAgent = UserAgent;
            r.Method = "POST";
            r.Referer = refer;
            r.CookieContainer = cc;
            r.ContentType = ContentType;
            if (param != null)
            {
                var bs = (encoding ?? DefaultEncoding).GetBytes(param);
                r.ContentLength = bs.Length;
                var stream = r.GetRequestStream();
                stream.Write(bs, 0, bs.Length);
                stream.Flush();
                stream.Close();
            }
            var rep = r.GetResponse();
            return rep.GetResponseStream();
        }
        /// <summary>
        /// 以post方式提交，将响应编码为字串。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="getParam"></param>
        /// <param name="timeOut">单位：秒</param>
        /// <param name="encoding"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string Post(string url, HttpParam param = null, HttpParam getParam = null, int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            var urlt = string.Format("{0}{1}", url, getParam == null ? "" : string.Format("?{0}", getParam.Format()));
            return Post(urlt, FormatData(param),null, timeOut, encoding, cc, refer);
        }
        public static T Post<T>(string url, HttpParam param = null, HttpParam getParam = null, int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            var str = Post(url, param, getParam, timeOut, encoding, cc, refer);
            return JsonConvert.DeserializeObject<T>(str);
            //return JsonHelper.Deserialize<T>(str);
        }
        /// <summary>
        /// 
        /// </summary>
        public static string Post(string url)
        {
            return Post(url, "");
        }
        /// <summary>
        /// 
        /// </summary>
        public static T Post<T>(string url)
        {
            return Post<T>(url, "");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="getParam"></param>
        /// <param name="timeOut">单位</：秒param>
        /// <param name="encoding"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string Post(string url, string param = null, string getParam = null, int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            var urlt = String.Format("{0}{1}", url, getParam == null ? "" : string.Format("?{0}", getParam));
            var str = new StreamReader(PostStream(urlt, param, timeOut, encoding, cc, refer), (encoding ?? DefaultEncoding)).ReadToEnd();
            return str;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="getParam"></param>
        /// <param name="timeOut">单位：秒</param>
        /// <param name="encoding"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static T Post<T>(string url, string param = null, string getParam = null, int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            var str = Post(url, param, getParam, timeOut, encoding, cc, refer);
            return JsonConvert.DeserializeObject<T>(str);
            //return JsonHelper.Deserialize<T>(str);
        }
        #endregion
        
        #region Common
        /// <summary>
        /// 创建一个请求
        /// </summary>
        public static HttpWebRequest CreateRequest(string url)
        {
            var r = WebRequest.Create(url) as HttpWebRequest;
            return r;
        }
        private static string FormatData(IEnumerable<KeyValuePair<string, object>> data)
        {
            return new HttpParam(data).Format();
        }
        
        #endregion

        /// <summary>
        /// 上传文件。formData参数附加到url
        /// </summary>
        public static string Upload(string url, HttpParam formData, string filePath)
        {
            var urlt = String.Format("{0}?{1}", url, formData == null ? "" : formData.Format());
            var data = new WebClient().UploadFile(urlt, "POST", filePath);
            return Encoding.UTF8.GetString(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="timeOut">单位：秒</param>
        /// <param name="encoding"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static HttpStatusCode HeadHttpCode(string url, string data = null, int timeOut = TimeOut, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            try
            {
                var r = CreateRequest(url);
                r.Timeout = timeOut;
                r.UserAgent = UserAgent;
                r.Method = "HEAD";
                r.Referer = refer;
                r.CookieContainer = cc;
                var httpWebResponse = r.GetResponse() as HttpWebResponse;
                if (httpWebResponse != null) return httpWebResponse.StatusCode;
                return HttpStatusCode.ExpectationFailed;
            }
            catch
            {
                return HttpStatusCode.ExpectationFailed;
            }
        }

        
    }
    /// <summary>
    /// 
    /// </summary>
    public class HttpParam : Dictionary<string, object>
    {
        public HttpParam()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public HttpParam(IEnumerable<KeyValuePair<string, object>> data)
        {
            foreach (var keyValuePair in data)
            {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
        /// <summary>
        /// 转换为http form格式字符串
        /// </summary>
        /// <returns></returns>
        public virtual string Format()
        {
            var lst = this.Select(e => String.Format("{0}={1}", e.Key, Uri.EscapeDataString(Convert.ToString(e.Value))));
            return String.Join("&", lst);
        }
    }
}
