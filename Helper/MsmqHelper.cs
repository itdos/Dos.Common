#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：EncryptHelper
* Copyright(c) www.ITdos.com
* CLR 版本: 4.0.30319.17929
* 创 建 人：ITdos
* 电子邮箱：admin@itdos.com
* 创建日期：2016/04/20 11:00:49
* 文件描述：
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Messaging;

namespace Dos.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class MsmqHelper
    {
        private string _path;
        private MessageQueue _sendQueue;
        private MessageQueue _receiveQueue;

        #region 私有
        /// <summary>
        /// 初始化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        private void Init<T>(MsmqParam param) where T : class
        {
            Init(param);
            if (_sendQueue == null)
            {
                _sendQueue = NewMessageQueue<T>(_path);
            }
            if (_receiveQueue == null)
            {
                _receiveQueue = NewMessageQueue<T>(_path);
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="param"></param>
        private void Init(MsmqParam param)
        {
            if (string.IsNullOrWhiteSpace(_path))
            {
                _path = param.QueuePath + param.QueueName;
            }
            CreateMessageQueue(_path);
        }
        /// <summary>
        /// 创建MessageQueue
        /// </summary>
        /// <param name="path"></param>
        private static void CreateMessageQueue(string path)
        {
            if (!MessageQueue.Exists(path))
            {
                MessageQueue.Create(path);
            }
        }
        /// <summary>
        /// 实例化MessageQueue
        /// </summary>
        private static MessageQueue NewMessageQueue<T>(string path)
        {
            var formatter = new XmlMessageFormatter(new Type[] { typeof(T) });
            var result = new MessageQueue(path) { Formatter = formatter };
            result.SetPermissions("everyone", MessageQueueAccessRights.FullControl);
            return result;
        }
        #endregion

        #region 动态方法
        /// <summary>
        /// 
        /// </summary>
        public MsmqHelper()
        {
            Init(new MsmqParam());
        }
        /// <summary>
        /// 传入自定义队列名。
        /// </summary>
        public MsmqHelper(string queueName)
        {
            Init(new MsmqParam()
            {
                QueueName = queueName
            });
        }
        /// <summary>
        /// 传入各种参数。
        /// </summary>
        public MsmqHelper(MsmqParam param)
        {
            Init(param);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        public void Send<T>(T message) where T : class
        {
            Init<T>(new MsmqParam<T>());
            var envelope = new Message(message) { Recoverable = false };
            _sendQueue.Send(envelope);
        }
        /// <summary>
        /// 发送消息并异步接收一个消息。
        /// </summary>
        public void SendAndReceiveAsyn<T>(T message, Action<T> callBack, bool loop = false) where T : class
        {
            Init<T>(new MsmqParam<T>());
            var envelope = new Message(message) { Recoverable = false };
            _sendQueue.Send(envelope);
            ReceiveAsyn(callBack, loop);
        }
        /// <summary>
        /// 异步接收消息。
        /// </summary>
        public void ReceiveAsyn<T>(Action<T> callBack, bool loop = false) where T : class
        {
            Init<T>(new MsmqParam<T>());
            _receiveQueue.ReceiveCompleted += (sender, args) =>
            {
                var result = args.Message.Body as T;
                callBack(result);
                if (loop)
                {
                    _receiveQueue.BeginReceive();
                }
            };
            _receiveQueue.BeginReceive();
        }
        /// <summary>
        /// 异步接收消息，常驻内存，一直等待接收新的队列并触发回调函数。
        /// </summary>
        /// <param name="callBack"></param>
        public void ReceiveAsynLoop<T>(Action<T> callBack) where T : class
        {
            ReceiveAsyn(callBack, true);
        }
        /// <summary>
        /// 接收消息。
        /// </summary>
        public T Receive<T>() where T : class
        {
            Init<T>(new MsmqParam<T>());
            var message = _receiveQueue.Receive();
            if (message != null)
            {
                return message.Body as T;
            }
            return null;
        }
        #endregion

        #region 静态方法
        /// <summary>
        /// 发送消息。必传Message。
        /// </summary>
        public static void Send<T>(MsmqParam<T> param) where T : class
        {
            var path = param.QueuePath + param.QueueName;
            CreateMessageQueue(path);
            var envelope = new Message(param.Message) { Recoverable = false };
            NewMessageQueue<T>(path).Send(envelope);
        }
        /// <summary>
        /// 发送消息并异步接收一个消息。必传Message。
        /// </summary>
        public static void SendAndReceiveAsyn<T>(MsmqParam<T> param, Action<T> callBack, bool loop = false) where T : class
        {
            var path = param.QueuePath + param.QueueName;
            CreateMessageQueue(path);
            var envelope = new Message(param.Message) { Recoverable = false };
            NewMessageQueue<T>(path).Send(envelope);
            param.CallBack = callBack;
            ReceiveAsyn(param, loop);
        }
        /// <summary>
        /// 发送消息。必传QueueName、Message。
        /// </summary>
        public static void Send<T>(string queueName, T message) where T : class
        {
            var param = new MsmqParam<T>();
            param.QueueName = queueName;
            param.Message = message;
            Send(param);
        }
        /// <summary>
        /// 发送消息。必传QueueName、Message。
        /// </summary>
        public static void SendAndReceiveAsyn<T>(string queueName, T message, Action<T> callBack, bool loop = false) where T : class
        {
            var param = new MsmqParam<T>();
            param.QueueName = queueName;
            param.Message = message;
            Send(param);
            param.CallBack = callBack;
            ReceiveAsyn(param, loop);
        }
        /// <summary>
        /// 异步接收消息。必传CallBack
        /// </summary>
        public static void ReceiveAsyn<T>(MsmqParam<T> param, bool loop = false) where T : class
        {
            var path = param.QueuePath + param.QueueName;
            CreateMessageQueue(path);
            var receiveQueue = NewMessageQueue<T>(path);
            receiveQueue.ReceiveCompleted += (sender, args) =>
            {
                var result = args.Message.Body as T;
                param.CallBack(result);
                if (loop)
                {
                    receiveQueue.BeginReceive();
                }
            };
            receiveQueue.BeginReceive();
        }
        /// <summary>
        /// 异步接收消息。必传QueueName、CallBack
        /// </summary>
        public static void ReceiveAsyn<T>(string queueName, Action<T> callBack) where T : class
        {
            var param = new MsmqParam<T>();
            param.QueueName = queueName;
            param.CallBack = callBack;
            ReceiveAsyn(param);
        }
        /// <summary>
        /// 异步接收消息。必传QueueName、CallBack。常驻内存，一直等待接收新的队列并触发回调函数。
        /// </summary>
        public static void ReceiveAsynLoop<T>(string queueName, Action<T> callBack) where T : class
        {
            var param = new MsmqParam<T>();
            param.QueueName = queueName;
            param.CallBack = callBack;
            ReceiveAsyn(param, true);
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        public static T Receive<T>(MsmqParam<T> param) where T : class
        {
            var path = param.QueuePath + param.QueueName;
            CreateMessageQueue(path);
            var receiveQueue = NewMessageQueue<T>(path);
            var message = receiveQueue.Receive();
            if (message != null)
            {
                return message.Body as T;
            }
            return null;
        }
        /// <summary>
        /// 接收消息。必传QueueName
        /// </summary>
        public static T Receive<T>(string queueName) where T : class
        {
            var param = new MsmqParam<T>();
            param.QueueName = queueName;
            return Receive(param);
        }
        #endregion
    }
    #region 参数类
    /// <summary>
    /// 参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MsmqParam<T> : MsmqParam where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        public T Message { get; set; }
        /// <summary>
        /// callBack
        /// </summary>
        public Action<T> CallBack { get; set; }
    }
    /// <summary>
    /// 参数
    /// </summary>
    public class MsmqParam
    {
        private string _queueName = "ITdosQueue";
        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName
        {
            get { return _queueName; }
            set { _queueName = value; }
        }
        /// <summary>
        /// 队列名称
        /// </summary>
        private string _queuePath = @".\Private$\";
        /// <summary>
        /// 路径
        /// </summary>
        public string QueuePath
        {
            get { return _queuePath; }
            set { _queuePath = value; }
        }
    }
    #endregion
}
