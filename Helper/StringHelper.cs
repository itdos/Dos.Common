#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：RegExpHelper
* Copyright(c) www.ITdos.com
* CLR 版本: 4.0.30319.17929
* 创 建 人：ITdos
* 电子邮箱：admin@itdos.com
* 创建日期：2016/05/04 23:11:36
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

namespace Dos.Common.Helper
{
    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// 获取字节数
        /// str：需要获取的字符串
        /// </summary>
        public static int Length(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return 0;
            }
            var j = 0;
            var ce = str.GetEnumerator();
            while (ce.MoveNext())
            {
                j += (ce.Current > 0 && ce.Current < 255) ? 1 : 2;
            }
            return j;
        }
        /// <summary>
        /// 按字节数截取指定字节
        /// </summary>
        /// <Param name="str">需要获取的字符串</Param>
        /// <Param name="length">获取的字节长度</Param>
        /// <returns></returns>
        public static string SubString(string str, int length)
        {
            var result = str;
            int j = 0, k = 0;
            var ce = str.GetEnumerator();
            while (ce.MoveNext())
            {
                j += (ce.Current > 0 && ce.Current < 255) ? 1 : 2;

                if (j <= length)
                {
                    k++;
                }
                else
                {
                    result = str.Substring(0, k);
                    break;
                }
            }
            return result;
        }
        #region 取首字母
        /// <summary>
        /// 获取汉字首字母（可包含多个汉字）
        /// </summary>
        /// <Param name="strText"></Param>
        /// <returns></returns>
        public static string GetChineseSpell(string strText)
        {
            int len = strText.Length;
            string myStr = "";
            for (int i = 0; i < len; i++)
            {
                myStr += GetSpell(strText.Substring(i, 1));
            }
            return myStr;
        }

        public static string GetSpell(string cnChar)
        {
            var arrCn = Encoding.Default.GetBytes(cnChar);
            if (arrCn.Length > 1)
            {
                int area = (short)arrCn[0];
                int pos = (short)arrCn[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
                    }
                }
                return "*";
            }
            return cnChar;
        }

        /// <summary>
        /// 获取第一个汉字的首字母，只能输入汉字
        /// </summary>
        /// <Param name="c"></Param>
        /// <returns></returns>
        public static string GetInitial(string c)
        {
            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "A";
            if (i < 0xB2C1) return "B";
            if (i < 0xB4EE) return "C";
            if (i < 0xB6EA) return "D";
            if (i < 0xB7A2) return "E";
            if (i < 0xB8C1) return "F";
            if (i < 0xB9FE) return "G";
            if (i < 0xBBF7) return "H";
            if (i < 0xBFA6) return "J";
            if (i < 0xC0AC) return "K";
            if (i < 0xC2E8) return "L";
            if (i < 0xC4C3) return "M";
            if (i < 0xC5B6) return "N";
            if (i < 0xC5BE) return "O";
            if (i < 0xC6DA) return "P";
            if (i < 0xC8BB) return "Q";
            if (i < 0xC8F6) return "R";
            if (i < 0xCBFA) return "S";
            if (i < 0xCDDA) return "T";
            if (i < 0xCEF4) return "W";
            if (i < 0xD1B9) return "X";
            if (i < 0xD4D1) return "Y";
            if (i < 0xD7FA) return "Z";
            return "*";
        }
        #endregion
        #region 计算匹配率/相似度
        /// <summary>
        /// 计算相似度。
        /// </summary>
        public static SimilarityResult SimilarityRate(string str1, string str2)
        {
            var result = new SimilarityResult();
            var arrChar1 = str1.ToCharArray();
            var arrChar2 = str2.ToCharArray();
            var computeTimes = 0;
            var row = arrChar1.Length + 1;
            var column = arrChar2.Length + 1;
            var matrix = new int[row, column];
            //开始时间
            var beginTime = DateTime.Now;
            //初始化矩阵的第一行和第一列
            for (var i = 0; i < column; i++)
            {
                matrix[0, i] = i;
            }
            for (var i = 0; i < row; i++)
            {
                matrix[i, 0] = i;
            }
            for (var i = 1; i < row; i++)
            {
                for (var j = 1; j < column; j++)
                {
                    var intCost = 0;
                    intCost = arrChar1[i - 1] == arrChar2[j - 1] ? 0 : 1;
                    //关键步骤，计算当前位置值为左边+1、上面+1、左上角+intCost中的最小值 
                    //循环遍历到最后_Matrix[_Row - 1, _Column - 1]即为两个字符串的距离
                    matrix[i, j] = Minimum(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1, matrix[i - 1, j - 1] + intCost);
                    computeTimes++;
                }
            }
            //结束时间
            var endTime = DateTime.Now;
            //相似率 移动次数小于最长的字符串长度的20%算同一题
            var intLength = row > column ? row : column;
            //_Result.Rate = (1 - (double)_Matrix[_Row - 1, _Column - 1] / intLength).ToString().Substring(0, 6);
            result.Rate = (1 - (double)matrix[row - 1, column - 1] / (intLength - 1));
            result.ExeTime = (endTime - beginTime).TotalMilliseconds;
            result.ComputeTimes = computeTimes.ToString() + " 距离为：" + matrix[row - 1, column - 1].ToString();
            return result;
        }
        /// <summary>
        /// 取三个数中的最小值
        /// </summary>
        public static int Minimum(int first, int second, int third)
        {
            var intMin = first;
            if (second < intMin)
            {
                intMin = second;
            }
            if (third < intMin)
            {
                intMin = third;
            }
            return intMin;
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        public struct SimilarityResult
        {
            /// <summary>
            /// 相似度，0.54即54%。
            /// </summary>
            public double Rate;
            /// <summary>
            /// 对比次数
            /// </summary>
            public string ComputeTimes;
            /// <summary>
            /// 执行时间，毫秒
            /// </summary>
            public double ExeTime;
        }
        #endregion
    }
}
