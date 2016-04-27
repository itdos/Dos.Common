#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) www.ITdos.com
* CLR 版本: 4.0.30319.17929
* 创 建 人：ITdos
* 电子邮箱：admin@itdos.com
* 创建日期：2015/09/10 14:08:52
* 文件描述：
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;

namespace Dos.Common
{
    /// <summary>
    /// 
    /// </summary>
    //[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class JsonProp : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonProp"/> class.
        /// </summary>
        public JsonProp()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonProp"/> class with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public JsonProp(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}