using System;
using System.Diagnostics;
using System.IO;
namespace Dos.Common
{
    /// <summary>
    /// Dos cmd命令执行帮助类
    /// </summary>
    public class CmdHelper
    {
        /// <summary>
        /// 运行dos命令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string Run(string command)
        {
            string str = "";
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd", "/c " + command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    str = reader.ReadToEnd();
                }
                process.WaitForExit();
            }
            return str.Trim();
        }
    }
}

