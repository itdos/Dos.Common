using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Dos.Common
{
    /// <summary>
    /// 日志帮助类。web.config-AppSettings可以配置DosLogHelperDebug=0或DosLogHelperError=0来关闭日志记录。
    /// 如果不传入path参数，默认是在~/Log/下生成日志文件，也可以web.config-AppSettings配置DosLogHelperPath来设置默认日志文件路径，格式：D;\\File\\Log\\。
    /// </summary>
    public class LogHelper
    {
        private enum LogHelperType
        {
            debug, error
        }
        /// <summary>
        /// 记录调试日志
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filePrefixName"></param>
        /// <param name="path">格式：D:\\File\\Logs\\</param>
        public static void Debug(string content, string filePrefixName = null, string path = null)
        {
            Write(LogHelperType.debug, content, filePrefixName, path);
        }
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filePrefixName"></param>
        /// <param name="path">格式：D:\\File\\Logs\\</param>
        public static void Error(string content, string filePrefixName = null, string path = null)
        {
            Write(LogHelperType.error, content, filePrefixName, path);
        }
        /// <summary>
        /// filePrefixName是文件名前缀，最好用中文，方便在程序Logs文件下查看。
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filePrefixName"></param>
        /// <param name="path"></param>
        /// <param name="logtype"></param>
        private static void Write(LogHelperType logtype, string content, string filePrefixName = null, string path = null)
        {
            try
            {
                if (logtype == LogHelperType.debug)
                {
                    var dosDebug = ConfigurationManager.AppSettings["DosLogHelperDebug"];
                    if (dosDebug != null && dosDebug != "1")
                    {
                        return;
                    }
                }
                else
                {
                    var dosError = ConfigurationManager.AppSettings["DosLogHelperError"];
                    if (dosError != null && dosError != "1")
                    {
                        return;
                    }
                }
                #region 日志文件
                var fileName = filePrefixName + DateTime.Now.ToString("yyyyMMdd") + logtype.ToString() + ".txt";
                if (string.IsNullOrWhiteSpace(path))
                {
                    var dosPath = ConfigurationManager.AppSettings["DosLogHelperPath"];
                    if (string.IsNullOrWhiteSpace(dosPath))
                    {
                        path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + fileName;
                    }
                    else
                    {
                        path = dosPath + fileName;
                    }
                }
                else
                {
                    path += fileName;
                }
                var di = new DirectoryInfo(path.Replace(fileName, ""));
                if (!di.Exists)
                {
                    di.Create();
                }
                //判断文件大小，需要新开文件
                using (var fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    var sw = new StreamWriter(fs);
                    sw.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.WriteLine();
                    sw.Write(content);
                    sw.WriteLine();
                    sw.Write("-----------------------------------------------------------------------------");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                }
                #endregion
            }
            catch
            {
            }
        }
    }
}
