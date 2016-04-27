#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：OperateStatus
* Copyright(c) www.ITdos.com
* CLR 版本: 4.0.30319.17929
* 创 建 人：ITdos
* 电子邮箱：admin@itdos.com
* 创建日期：2014/10/1 11:00:49
* 文件描述：
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ServiceModel;
namespace Dos.Common
{
    /// <summary>
    /// 
    /// </summary>
    //[Serializable]
    [DataContract]
    //[CollectionDataContractAttribute]
    public class BaseResult : DynamicObject
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseResult()
        {
            this.IsSuccess = true;
            this.Data = null;
            this.Message = null;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <Param name="isSuccess"></Param>
        public BaseResult(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
            this.Data = null;
            this.Message = null;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <Param name="isSuccess"></Param>
        /// <Param name="data"></Param>
        public BaseResult(bool isSuccess, object data)
        {
            this.IsSuccess = isSuccess;
            this.Data = data;
            this.Message = null;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <Param name="isSuccess"></Param>
        /// <Param name="data"></Param>
        /// <Param name="message"></Param>
        public BaseResult(bool isSuccess, object data, string message)
        {
            this.IsSuccess = isSuccess;
            this.Data = data;
            this.Message = message;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <Param name="isSuccess"></Param>
        /// <Param name="data"></Param>
        /// <Param name="message"></Param>
        /// <Param name="dataCount"></Param>
        public BaseResult(bool isSuccess, object data, string message, int dataCount)
        {
            this.IsSuccess = isSuccess;
            this.Data = data;
            this.Message = message;
            this.DataCount = dataCount;
        }
        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        [DataMember]
        public object Data { get; set; }
        /// <summary>
        /// 数量数量
        /// </summary>
        [DataMember]
        public int? DataCount { get; set; }

        public Dictionary<string, object> Properties = new Dictionary<string, object>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!Properties.Keys.Contains(binder.Name))
            {
                Properties.Add(binder.Name, value.ToString());
            }
            return true;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return Properties.TryGetValue(binder.Name, out result);
        }
    }
}
