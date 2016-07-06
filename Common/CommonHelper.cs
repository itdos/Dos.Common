#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) www.ITdos.com
* CLR 版本: 4.0.30319.17929
* 创 建 人：ITdos
* 电子邮箱：admin@itdos.com
* 创建日期：2015/09/10 14:08:52
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
using System.Web;
using Dos.Common.Helper;

namespace Dos.Common
{
    /// <summary>
    /// 通用Helper
    /// </summary>
    public class CommonHelper
    {
        /// <summary>
        /// 获取当前项目完整路径，如D:\Web\
        /// </summary>
        /// <returns></returns>
        public static string GetFullPath()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath("~/");
            }
            if (!AppDomain.CurrentDomain.BaseDirectory.DosIsNullOrWhiteSpace())
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
            return "";
        }
        #region 取首字母
        /// <summary>
        /// 获取汉字首字母（可包含多个汉字）
        /// </summary>
        /// <Param name="strText"></Param>
        /// <returns></returns>
        public static string GetChineseSpell(string strText)
        {
            return StringHelper.GetChineseSpell(strText);
        }

        public static string GetSpell(string cnChar)
        {
            return StringHelper.GetSpell(cnChar);
        }

        /// <summary>
        /// 获取第一个汉字的首字母，只能输入汉字
        /// </summary>
        /// <Param name="c"></Param>
        /// <returns></returns>
        public static string GetInitial(string c)
        {
            return StringHelper.GetInitial(c);
        }
        #endregion
        #region 计算匹配率/相似度
        /// <summary>
        /// 计算相似度。
        /// </summary>
        public static StringHelper.SimilarityResult SimilarityRate(string str1, string str2)
        {
            return StringHelper.SimilarityRate(str1, str2);
        }
        /// <summary>
        /// 取三个数中的最小值
        /// </summary>
        public static int Minimum(int first, int second, int third)
        {
            return StringHelper.Minimum(first, second,third);
        }
        #endregion
    }
}
