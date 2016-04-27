using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dos.Common
{
   /// <summary>
   /// 
   /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 从文件中读取所有内容。（如果文件不存在，返回空字符串）
        /// </summary>
        /// <Param name="filePath">完整路径，如D:\Temp\Temp.json</Param>
        /// <returns></returns>
        public static string Read(string filePath, Encoding encoding = null)
        {
            if (File.Exists(filePath))
            {
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }
                return File.ReadAllText(filePath, encoding);
            }
            return "";
        }
        /// <summary>
        /// 往文件中写内容。（如果文件已存在，则会删除文件再创建。反之直接创建文件）
        /// </summary>
        /// <Param name="filePath">完整路径，如D:\Temp\Temp.json</Param>
        /// <Param name="content">内容。可以\r\n换行。</Param>
        /// <returns></returns>
        public static bool Write(string filePath, string content, Encoding encoding = null)
        {
            File.Delete(filePath);
            using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }
                var sw = new StreamWriter(fs, encoding);
                sw.Write(content);
                sw.Flush();
                sw.Close();
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDir"></param>
        /// <param name="toDir"></param>
        public static void Copy(string fromDir, string toDir)
        {
            DirectoryInfo source = new DirectoryInfo(fromDir);
            DirectoryInfo target = new DirectoryInfo(toDir);
            Copy(source, target);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void Copy(DirectoryInfo from, DirectoryInfo to)
        {
            if (to.FullName.StartsWith(from.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!from.Exists)
            {
                return;
            }

            if (!to.Exists)
            {
                to.Create();
            }

            FileInfo[] files = from.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                File.Copy(files[i].FullName, Path.Combine(to.FullName, files[i].Name), true);
            }
            DirectoryInfo[] dirs = from.GetDirectories();

            for (int j = 0; j < dirs.Length; j++)
            {
                Copy(dirs[j].FullName, Path.Combine(to.FullName, dirs[j].Name));
            }
        }
        /// <summary>
        /// 根据完整文件路径获取FileStream
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileStream ReadStream(string fileName)
        {
            FileStream fileStream = null;
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                fileStream = new FileStream(fileName, FileMode.Open);
            }
            return fileStream;
        }
    }
}
