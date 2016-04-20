#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) www.ITdos.com
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
    /// <summary>
    /// 队列操作类
    /// </summary>
    public class QueueHelper
    {
        #region 队列参数
        /// <summary>
        /// 队列参数
        /// </summary>
        public class QueueParam
        {
            /// <summary>
            /// 队列限制数量
            /// </summary>
            public int? QueueCount { get; set; }
            /// <summary>
            /// 队列池
            /// </summary>
            public ConcurrentDictionary<string, object> Pool { get; set; }
        }
        #endregion

        #region 动态队列。适用于FIFO先进先出（如抢购活动），初始化需要设置队列最大数量。
        /// <summary>
        /// 队列参数
        /// </summary>
        public static QueueParam Param;
        /// <summary>
        /// 初始化队列
        /// </summary>
        public QueueHelper()
        {
            Param.Pool = new ConcurrentDictionary<string, object>();
            Param.QueueCount = 5000;
        }
        /// <summary>
        /// 初始化队列。可动态传入QueueCount
        /// </summary>
        public QueueHelper(QueueParam qr)
        {
            Param.Pool = qr.Pool ?? new ConcurrentDictionary<string, object>();
            Param.QueueCount = qr.QueueCount ?? 5000;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object In(string key)
        {
            return In(key, "");
        }
        /// <summary>
        /// 加入队列。成功加入队列后必须在外部手动执行退出队列：QueueHelper对象.Out(string key)。
        /// </summary>
        /// <Param name="key">资源Key</Param>
        /// <Param name="obj"></Param>
        /// <returns></returns>
        public static object In(string key, object obj)
        {
            var failCount = -1;
        Start:
            if (TryGet(key, obj))
                return obj;
            failCount++;
            if (failCount > 5000)
            {
                return null;
            }
            goto Start;
        }
        /// <summary>
        /// 抢资源
        /// </summary>
        /// <Param name="key"></Param>
        /// <Param name="obj"></Param>
        /// <returns></returns>
        private static bool TryGet(string key, object obj)
        {
            if (!Param.Pool.Keys.Contains(key)) 
                return Param.Pool.TryAdd(key, obj);
            Thread.Sleep(SleepNumber);
            return false;
        }

        /// <summary>
        /// 退出队列/释放资源
        /// </summary>
        /// <Param name="key"></Param>
        public static bool Out(string key)
        {
            object s;
            return !string.IsNullOrWhiteSpace(key) && Param.Pool.TryRemove(key, out s);
        }
        /// <summary>
        /// 退出所有队列/释放所有资源
        /// </summary>
        public static void OutAll()
        {
            Param.Pool.Clear();
        }
        #endregion

        #region 静态队列。适用于锁定资源（如每天电影院不同影院固定座位限制），随时都能加入队列抢资源。
        /// <summary>
        /// 尝试入队次数
        /// </summary>
        private static int RetryCount = 5000;
        /// <summary>
        /// 静态队列池
        /// </summary>
        private static ConcurrentDictionary<string, object> staticPool = new ConcurrentDictionary<string, object>();
        /// <summary>
        /// 睡眠时间（毫秒）
        /// </summary>
        private const int SleepNumber = 1;

        /// <summary>
        /// 加入队列
        /// </summary>
        /// <Param name="key"></Param>
        /// <returns></returns>
        public static object StaticIn(string key)
        {
            return StaticIn(key, "");
        }
        /// <summary>
        /// 加入队列。成功加入队列后必须在外部手动执行退出队列：QueueHelper.Out(string key)。
        /// </summary>
        /// <Param name="key">资源Key</Param>
        /// <Param name="obj"></Param>
        /// <returns></returns>
        public static object StaticIn(string key, object obj)
        {
            var failCount = -1;
        Start:
            if (StaticTryGet(key, obj)) 
                return obj;
            failCount++;
            if (failCount > RetryCount)
            {
                return null;
            }
            goto Start;
        }
        /// <summary>
        /// 抢资源
        /// </summary>
        /// <Param name="key"></Param>
        /// <Param name="obj"></Param>
        /// <returns></returns>
        private static bool StaticTryGet(string key, object obj)
        {
            if (!staticPool.Keys.Contains(key)) 
                return staticPool.TryAdd(key, obj);
            Thread.Sleep(SleepNumber);
            return false;
        }

        /// <summary>
        /// 退出队列/释放资源
        /// </summary>
        /// <Param name="key"></Param>
        public static bool StaticOut(string key)
        {
            object s;
            return !string.IsNullOrWhiteSpace(key) && staticPool.TryRemove(key, out s);
        }
        /// <summary>
        /// 退出所有队列/释放所有资源
        /// </summary>
        public static void StaticOutAll()
        {
            staticPool.Clear();
        }
        #endregion
    }
}