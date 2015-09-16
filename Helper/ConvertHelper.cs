using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dos.Common
{
    /// <summary>
    /// 通用转换类
    /// </summary>
    public class ConvertHelper
    {
        /// <summary>
        /// 将枚举转换为Dictionary<string, string/>
        /// </summary>
        public static Dictionary<string, string> EnumToDictionary(Type enumType)
        {
            if (enumType.IsEnum == false)
            {
                return null;
            }
            var list = new Dictionary<string, string>();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.IsSpecialName) continue;
                strValue = field.GetRawConstantValue().ToString();
                object[] arr = field.GetCustomAttributes(typeDescription, true);
                if (arr.Length > 0)
                {
                    strText = (arr[0] as DescriptionAttribute).Description;
                }
                else
                {
                    strText = field.Name;
                }
                list.Add(strText, strValue);
            }
            return list;
        }
    }
}
