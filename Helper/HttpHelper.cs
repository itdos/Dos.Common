#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：EncryptHelper
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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using EmitMapper.AST.Nodes;
using System.Web;

namespace Dos.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Stream GetStream(string url)
        {
            return RequestStream(new HttpParam()
            {
                Url = url,
                Method = "GET"
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public static Stream GetStream(string url, object getParam)
        {
            return RequestStream(new HttpParam()
            {
                Url = url,
                Method = "GET",
                GetParam = getParam
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Stream PostStream(string url)
        {
            return RequestStream(new HttpParam()
            {
                Url = url,
                Method = "POST"
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postParam"></param>
        /// <returns></returns>
        public static Stream PostStream(string url, object postParam)
        {
            return RequestStream(new HttpParam()
            {
                Url = url,
                Method = "POST",
                GetParam = postParam
            });
        }
        /// <summary>
        /// 文件上传至远程服务器。传入：Url、CookieContainer、PostParam、PostedFile
        /// </summary>
        public static string PostFile(HttpParam param)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(param.Url);
            request.CookieContainer = param.CookieContainer;
            request.Method = "POST";
            request.Timeout = 20000;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;
            request.KeepAlive = true;
            var boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            var boundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            var formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
            var buffer = new byte[param.PostedFile.ContentLength];
            param.PostedFile.InputStream.Read(buffer, 0, buffer.Length);
            var strHeader = "Content-Disposition:application/x-www-form-urlencoded; name=\"{0}\";filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n";
            strHeader = string.Format(strHeader,
                                     "filedata",
                                     param.PostedFile.FileName,
                                     param.PostedFile.ContentType);
            var byteHeader = ASCIIEncoding.ASCII.GetBytes(strHeader);
            try
            {
                using (var stream = request.GetRequestStream())
                {
                    if (param.PostParam != null)
                    {
                        var postParamString = "";
                        if (param.PostParam is string)
                        {
                            postParamString = param.PostParam.ToString();
                        }
                        else
                        {
                            postParamString = JSON.ToJSON(param.PostParam);
                        }
                        var bs = param.Encoding.GetBytes(postParamString);
                        stream.Write(bs, 0, bs.Length);
                    }
                    stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                    stream.Write(byteHeader, 0, byteHeader.Length);
                    stream.Write(buffer, 0, buffer.Length);
                    var trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                    stream.Write(trailer, 0, trailer.Length);
                    stream.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                var result = "";
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
                response.Close();
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// 获取响应流
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Stream RequestStream(HttpParam param)
        {
            #region 处理地址栏参数
            var getParamSb = new StringBuilder();
            if (param.GetParam != null)
            {
                if (param.GetParam is string)
                {
                    getParamSb.Append(param.GetParam.ToString());
                }
                else
                {
                    param.GetParam.GetType().GetProperties().ToList().ForEach(d =>
                    {
                        getParamSb.AppendFormat("{0}={1}&", d.Name, d.GetValue(param.GetParam, null));
                    });
                }
            }
            if (!string.IsNullOrWhiteSpace(getParamSb.ToString().TrimEnd('&')))
            {
                param.Url = string.Format("{0}?{1}", param.Url, getParamSb.ToString().TrimEnd('&'));
            }
            #endregion
            var r = WebRequest.Create(param.Url) as HttpWebRequest;
            if (!string.IsNullOrWhiteSpace(param.CertPath) && !string.IsNullOrWhiteSpace(param.CertPwd))
            {
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                var cer = new X509Certificate2(param.CertPath, param.CertPwd, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                r.ClientCertificates.Add(cer);
                #region 暂时不要的
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                //req.ProtocolVersion = HttpVersion.Version11;
                //req.UserAgent = SUserAgent;
                //req.KeepAlive = false;
                //var cookieContainer = new CookieContainer();
                //req.CookieContainer = cookieContainer;
                //req.Timeout = 1000 * 60;
                //req.Headers.Add("x-requested-with", "XMLHttpRequest");
                #endregion
            }
            r.Timeout = param.TimeOut * 1000;
            r.UserAgent = param.UserAgent;
            r.Method = param.Method ?? "POST";
            r.Referer = param.Referer;
            r.CookieContainer = param.CookieContainer;
            r.ContentType = param.ContentType;
            if (param.PostParam != null)
            {
                var postParamString = "";
                if (param.PostParam is string)
                {
                    postParamString = param.PostParam.ToString();
                }
                else if (param.ParamType == EnumHelper.HttpParamType.Form)
                {
                    var dicParam = JSON.ToObject<Dictionary<string,string>>(JSON.ToJSON(param.PostParam));
                    postParamString = dicParam.Aggregate(postParamString, (current, dic) => current + (dic.Key + "=" + dic.Value + "&")).TrimEnd('&');
                }
                else
                {
                    postParamString = JSON.ToJSON(param.PostParam);
                }
                var bs = param.Encoding.GetBytes(postParamString);
                r.ContentLength = bs.Length;
                using (var rs = r.GetRequestStream())
                {
                    rs.Write(bs, 0, bs.Length);
                }
            }
            return r.GetResponse().GetResponseStream();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        public static string RequestString(HttpParam param)
        {
            var result = "";
            using (var reader = new StreamReader(RequestStream(param), param.Encoding))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        #region Get请求
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Get(string url)
        {
            return Get(new HttpParam()
            {
                Url = url,
                Method = "GET"
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public static string Get(string url, object getParam)
        {
            var param = new HttpParam
            {
                Url = url,
                Method = "GET",
                GetParam = getParam
            };
            return Get(param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Get(HttpParam param)
        {
            param.Method = "GET";
            return RequestString(param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T Get<T>(string url)
        {
            var str = Get(new HttpParam()
            {
                Url = url,
                Method = "GET"
            });
            return JSON.ToObject<T>(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public static T Get<T>(string url, object getParam)
        {
            var param = new HttpParam
            {
                Url = url,
                Method = "GET",
                GetParam = getParam
            };
            var str = Get(param);
            return JSON.ToObject<T>(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T Get<T>(HttpParam param)
        {
            var str = Get(param);
            return JSON.ToObject<T>(str);
        }
        #endregion

        #region Post 请求
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Post(string url)
        {
            var str = Post(new HttpParam()
            {
                Url = url,
                Method = "POST"
            });
            return str;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postParam"></param>
        /// <returns></returns>
        public static string Post(string url, object postParam)
        {
            var param = new HttpParam
            {
                Url = url,
                Method = "POST",
                PostParam = postParam
            };
            var str = Post(param);
            return str;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Post(HttpParam param)
        {
            param.Method = "POST";
            var str = RequestString(param);
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T Post<T>(string url)
        {
            var str = Post(new HttpParam()
            {
                Url = url,
                Method = "POST"
            });
            return JSON.ToObject<T>(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postParam"></param>
        /// <returns></returns>
        public static T Post<T>(string url, object postParam)
        {
            var param = new HttpParam
            {
                Url = url,
                Method = "POST",
                PostParam = postParam
            };
            var str = Post(param);
            return JSON.ToObject<T>(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T Post<T>(HttpParam param)
        {
            param.Method = "POST";
            var str = Post(param);
            return JSON.ToObject<T>(str);
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        #endregion
    }
}
