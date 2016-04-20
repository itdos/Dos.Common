using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Collections;

namespace Dos.Common
{
    /// <summary>
    /// 缓存处理类
    /// </summary>
    public class CacheHelper
    {
        /// <summary>
        /// cache
        /// </summary>
        private static volatile System.Web.Caching.Cache cache = HttpRuntime.Cache;

        /// <summary>
        /// timeout 600秒
        /// </summary>
        private static int _timeOut = 600 * 2;

        ///// <summary>
        ///// lock object
        ///// </summary>
        //private static object lockobj = new object();

        /// <summary>
        /// 添加缓存 (绝对有效期)
        /// </summary>
        /// <Param name="cacheKey">缓存键值</Param>
        /// <Param name="cacheValue">缓存内容</Param>
        public static void Set(string cacheKey, object cacheValue)
        {
            Set(cacheKey, cacheValue, _timeOut);
        }

        /// <summary>
        /// 添加缓存 (绝对有效期)
        /// </summary>
        /// <Param name="cacheKey">缓存键值</Param>
        /// <Param name="cacheValue">缓存内容</Param>
        /// <Param name="timeout">绝对有效期（单位: 秒）</Param>
        public static void Set(string cacheKey, object cacheValue, int timeout)
        {

            if (string.IsNullOrEmpty(cacheKey))
            {
                return;
            }

            if (null == cacheValue)
            {
                Remove(cacheKey);
                return;
            }

            CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(onRemove);

            if (timeout <= 0)
            {
                cache.Insert(cacheKey, cacheValue, null, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.High, callBack);
            }
            else
            {
                cache.Insert(cacheKey, cacheValue, null, DateTime.Now.AddSeconds(timeout), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, callBack);
            }
        }

        /// <summary>
        /// 添加缓存 (相对有效期)
        /// </summary>
        /// <Param name="cacheKey">缓存键值</Param>
        /// <Param name="cacheValue">缓存内容</Param>
        public static void AddCacheSlidingExpiration(string cacheKey, object cacheValue)
        {
            AddCacheSlidingExpiration(cacheKey, cacheValue, _timeOut);
        }

        /// <summary>
        /// 添加缓存 (相对有效期)
        /// </summary>
        /// <Param name="cacheKey">缓存键值</Param>
        /// <Param name="cacheValue">缓存内容</Param>
        /// <Param name="timeout">相对过期时间 (单位: 秒)</Param>
        public static void AddCacheSlidingExpiration(string cacheKey, object cacheValue, int timeout)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                return;
            }

            if (null == cacheValue)
            {
                Remove(cacheKey);
                return;
            }

            CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(onRemove);

            if (timeout <= 0)
            {
                cache.Insert(cacheKey, cacheValue, null, DateTime.MaxValue, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, callBack);
            }
            else
            {
                cache.Insert(cacheKey, cacheValue, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(timeout), System.Web.Caching.CacheItemPriority.High, callBack);
            }
        }


        /// <summary>
        /// 添加缓存 (文件依赖)
        /// </summary>
        /// <Param name="cacheKey">缓存键值</Param>
        /// <Param name="cacheValue">缓存内容</Param>
        /// <Param name="filenames">缓存依赖的文件或目录</Param>
        public static void AddCacheFilesDependency(string cacheKey, object cacheValue, params string[] filenames)
        {
            CacheDependency dep = new CacheDependency(filenames, DateTime.Now);

            AddCacheDependency(cacheKey, cacheValue, _timeOut, dep);
        }

        /// <summary>
        /// 添加缓存 (文件依赖)
        /// </summary>
        /// <Param name="cacheKey">缓存键值</Param>
        /// <Param name="cacheValue">缓存内容</Param>
        /// <Param name="timeout">绝对过期时间 （单位：秒）</Param>
        /// <Param name="dep">缓存依赖</Param>
        public static void AddCacheDependency(string cacheKey, object cacheValue, int timeout, CacheDependency dep)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                return;
            }

            if (null == cacheValue)
            {
                Remove(cacheKey);
                return;
            }

            CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(onRemove);

            if (timeout <= 0)
            {
                cache.Insert(cacheKey, cacheValue, dep, DateTime.MaxValue, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, callBack);
            }
            else
            {
                cache.Insert(cacheKey, cacheValue, dep, System.DateTime.Now.AddSeconds(timeout), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, callBack);
            }
        }


        /// <summary>
        /// 添加缓存 (一组键值依赖)
        /// </summary>
        /// <Param name="cacheKey">缓存键值</Param>
        /// <Param name="cacheValue">缓存内容</Param>
        /// <Param name="cachekeys">一组缓存键，此改变缓存也失效</Param>
        public static void AddCacheKeysDependency(string cacheKey, object cacheValue, string[] cachekeys)
        {

            CacheDependency dep = new CacheDependency(null, cachekeys, DateTime.Now);

            AddCacheDependency(cacheKey, cacheValue, _timeOut, dep);
        }



        /// <summary>
        /// 缓存删除的委托实例
        /// </summary>
        /// <Param name="key"></Param>
        /// <Param name="val"></Param>
        /// <Param name="reason"></Param>
        private static void onRemove(string key, object val, CacheItemRemovedReason reason)
        {
            //switch (reason)
            //{
            //    case CacheItemRemovedReason.DependencyChanged:
            //        break;
            //    case CacheItemRemovedReason.Expired:
            //        break;
            //    case CacheItemRemovedReason.Removed:
            //        break;
            //    case CacheItemRemovedReason.Underused:
            //        break;
            //    default: break;
            //}

            //do something: write log  ext.

        }


        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <Param name="cacheKey">缓存键值</Param>
        public static void Remove(string cacheKey)
        {
            if (!string.IsNullOrEmpty(cacheKey))
                cache.Remove(cacheKey);
        }


        /// <summary>
        /// 获取缓存。若没有设置过传入的cacheKey，则返回null
        /// </summary>
        /// <Param name="cacheKey">对象的关键字</Param>
        /// <returns></returns>
        public static object Get(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                return null;
            }
            return cache.Get(cacheKey);
        }


        /// <summary>
        /// 获取缓存数量
        /// </summary>
        /// <returns></returns>
        public static int GetCount()
        {
            return cache.Count;
        }


        /// <summary>
        /// 返回缓存键值列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetKeys()
        {
            List<string> cacheKeys = new List<string>();
            IDictionaryEnumerator cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                cacheKeys.Add(cacheEnum.Key.ToString());
            }
            return cacheKeys;
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public static void Clear()
        {
            List<string> cacheKeys = GetKeys();
            foreach (string cacheKey in cacheKeys)
            {
                Remove(cacheKey);
            }
        }

    }
}