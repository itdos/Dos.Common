#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：OperateStatus
* Copyright(c) 青之软件
* CLR 版本: 4.0.30319.17929
* 创 建 人：周浩
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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
namespace Dos.Common
{
    [Serializable]
    [DataContract]
    [CollectionDataContractAttribute]
    public class BaseResult
    {
        public BaseResult()
        {
            this.IsSuccess = true;
            this.Data = null;
            this.Message = null;
        }
        public BaseResult(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
            this.Data = null;
            this.Message = null;
        }
        public BaseResult(bool isSuccess, object data)
        {
            this.IsSuccess = isSuccess;
            this.Data = data;
            this.Message = null;
        }
        public BaseResult(bool isSuccess, object data, string message)
        {
            this.IsSuccess = isSuccess;
            this.Data = data;
            this.Message = message;
        }
        public BaseResult(bool isSuccess, object data, string message, int dataCount)
        {
            this.IsSuccess = isSuccess;
            this.Data = data;
            this.Message = message;
            this.DataCount = dataCount;
        }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public object Data { get; set; }
        [DataMember]
        public int? DataCount { get; set; }
    }
}
