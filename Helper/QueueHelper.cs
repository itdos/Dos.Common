#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：MyConcurrent
* Copyright(c) 青之软件
* CLR 版本: 4.0.30319.17929
* 创 建 人：ITdos
* 电子邮箱：admin@itdos.com
* 创建日期：2014/12/29 13:41:57
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
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Threading;

namespace Dos.Common
{
    public class QueueHelper
    {
        static ConcurrentDictionary<string, object> dicPool = new ConcurrentDictionary<string, object>();
        /// <summary>
        /// 睡眠时间（毫秒）
        /// </summary>
        private const int SleepNumber = 1;

        /// <summary>
        /// 加入队列
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Join(string key)
        {
            return Join(key, "");
        }
        /// <summary>
        /// 加入队列。
        /// </summary>
        /// <param name="key">资源Key</param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object Join(string key, object obj)
        {
            var failCount = -1;
        Start:
            //抢资源
            if (!TryGet(key, obj))
            {
                failCount++;
                //如果重试次数超过了N秒。（1000=1秒）
                if (failCount > 5000)
                {
                    return null;
                }
                goto Start;
            }
            //资源到手，慢慢处理。
            return obj;
            //释放资源
            //Free(key);
        }
        /// <summary>
        /// 抢资源
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool TryGet(string key, object obj)
        {
            if (dicPool.Keys.Contains(key))
            {
                Thread.Sleep(SleepNumber);
                return false;
            }
            if (!dicPool.TryAdd(key, obj))
            {
                //重新开始
                return false;
            }
            return true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="key"></param>
        public static void Free(string key)
        {
            object s;
            if (!string.IsNullOrWhiteSpace(key))
            {
                dicPool.TryRemove(key, out s);
            }
        }
    }
}