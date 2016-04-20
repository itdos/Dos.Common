using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dos.Common
{
    /// <summary>
    /// 通用扩展
    /// </summary>
    public static class CommonExpand
    {
        /// <summary>
        /// 是否是Guid
        /// </summary>
        /// <Param name="key"></Param>
        /// <returns></returns>
        public static bool IsGuid(this string key)
        {
            Guid g;
            return Guid.TryParse(key, out g);
        }
    }
}
