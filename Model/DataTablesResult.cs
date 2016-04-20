#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：OperateStatus
* Copyright(c) www.ITdos.com
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
    [DataContract]
    public class DataTablesResult
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public object data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int draw { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int recordsFiltered { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int recordsTotal { get; set; }
    }
}
