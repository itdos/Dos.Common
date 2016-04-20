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
        public static string GetText(string filePath)
        {
            if (File.Exists(filePath))
            {
              return   File.ReadAllText(filePath, Encoding.UTF8);
            }
            return "";
        }
        /// <summary>
        /// 往文件中写内容。（如果文件已存在，则会删除文件再创建。反之直接创建文件）
        /// </summary>
        /// <Param name="filePath">完整路径，如D:\Temp\Temp.json</Param>
        /// <Param name="content">内容。可以\r\n换行。</Param>
        /// <returns></returns>
        public static bool SetText(string filePath, string content)
        {
            File.Delete(filePath);
            using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                var sw = new StreamWriter(fs);
                sw.Write(content);
                sw.Flush();
                sw.Close();
            }
            return true;
        }
        public static void CopyDirectory(string srcDir, string tgtDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);
            CopyDirectory(source, target);
        }
        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!source.Exists)
            {
                return;
            }

            if (!target.Exists)
            {
                target.Create();
            }

            FileInfo[] files = source.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
            }
            DirectoryInfo[] dirs = source.GetDirectories();

            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
            }
        }
        /// <summary>
        /// 根据完整文件路径获取FileStream
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileStream GetFileStream(string fileName)
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
