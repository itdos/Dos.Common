using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private static Dictionary<MemberInfo, Object> _micache1 = new Dictionary<MemberInfo, Object>();
        private static Dictionary<MemberInfo, Object> _micache2 = new Dictionary<MemberInfo, Object>();
        /// <summary>
        /// 获取自定义特性，带有缓存功能，避免因.Net内部GetCustomAttributes没有缓存而带来的损耗
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute[] GetCustomAttributes<TAttribute>(this MemberInfo member, Boolean inherit)
        {
            if (member == null) return new TAttribute[0];

            // 根据是否可继承，分属两个缓存集合
            var cache = inherit ? _micache1 : _micache2;

            Object obj = null;
            if (cache.TryGetValue(member, out obj)) return (TAttribute[])obj;
            lock (cache)
            {
                if (cache.TryGetValue(member, out obj)) return (TAttribute[])obj;

                var atts = member.GetCustomAttributes(typeof(TAttribute), inherit) as TAttribute[];
                var att = atts == null ? new TAttribute[0] : atts;
                cache[member] = att;
                return att;
            }
        }
        /// <summary>获取自定义属性</summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo member, Boolean inherit)
        {
            var atts = member.GetCustomAttributes<TAttribute>(inherit);
            if (atts == null || atts.Length < 1) return default(TAttribute);
            return atts[0];
        }
    }
}
