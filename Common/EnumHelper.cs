using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dos.Common
{
    public class EnumHelper
    {
        public enum HttpParamType
        {
            /// <summary>
            /// json数据。默认值。
            /// </summary>
            Json,
            /// <summary>
            /// 形如：key=value＆key=value＆key=value
            /// </summary>
            Form
        }
    }
}
