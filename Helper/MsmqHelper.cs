using System;
using System.Messaging;

namespace Dos.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class MsmqHelper<T> where T : class
    {
        const string QueueUrl = @".\Private$\";
        const string QueueName = "ITdosQueue";
        private readonly MessageQueue _sendQueue;
        private readonly MessageQueue _receiveQueue;
        /// <summary>
        /// 
        /// </summary>
        private static readonly MessageQueue StaticSendQueue;
        /// <summary>
        /// 
        /// </summary>
        private static readonly MessageQueue StaticReceiveQueue;
        #region 动态方法
        /// <summary>
        /// 
        /// </summary>
        public MsmqHelper()
        {
            var name = QueueUrl + QueueName;
            if (!MessageQueue.Exists(name))
            {
                MessageQueue.Create(name);
            }
            var formatter = new XmlMessageFormatter(new Type[] { typeof(T) });
            _sendQueue = new MessageQueue(name) { Formatter = formatter };
            _receiveQueue = new MessageQueue(name) { Formatter = formatter };
        }
        /// <summary>
        /// 
        /// </summary>
        public MsmqHelper(string queueName)
        {
            var name = QueueUrl + queueName;
            if (!MessageQueue.Exists(name))
            {
                MessageQueue.Create(name);
            }
            var formatter = new XmlMessageFormatter(new Type[] { typeof(T) });
            _sendQueue = new MessageQueue(name) { Formatter = formatter };
            _receiveQueue = new MessageQueue(name) { Formatter = formatter };
        }
        /// <summary>
        /// placeholder：占位符，传入null即可。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="placeholder">占位符，传入null即可。</param>
        public void Send(T message, object placeholder)
        {
            var envelope = new Message(message) { Recoverable = false };
            _sendQueue.Send(envelope);
        }
        /// <summary>
        /// placeholder：占位符，传入null即可。
        /// </summary>
        /// <param name="messageReceiver"></param>
        /// <param name="placeholder">占位符，传入null即可。</param>
        public void ReceiveAsyn(Action<T> messageReceiver, object placeholder)
        {
            _receiveQueue.ReceiveCompleted += (sender, args) =>
            {
                try
                {
                    var result = args.Message.Body as T;
                    messageReceiver(result);
                    _receiveQueue.BeginReceive();
                }
                catch (Exception exception)
                {
                    //Console.WriteLine(exception);
                    throw;
                }
            };
            _receiveQueue.BeginReceive();
        }
        #endregion
        #region 静态方法
        /// <summary>
        /// 
        /// </summary>
        static MsmqHelper()
        {
            var name = QueueUrl + QueueName;
            if (!MessageQueue.Exists(name))
            {
                MessageQueue.Create(name);
            }
            var formatter = new XmlMessageFormatter(new Type[] { typeof(T) });
            StaticSendQueue = new MessageQueue(name) { Formatter = formatter };
            StaticReceiveQueue = new MessageQueue(name) { Formatter = formatter };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Send(T message)
        {
            var envelope = new Message(message) { Recoverable = false };
            StaticSendQueue.Send(envelope);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageReceiver"></param>
        public static void ReceiveAsyn(Action<T> messageReceiver)
        {
            StaticReceiveQueue.ReceiveCompleted += (sender, args) =>
            {
                try
                {
                    var result = args.Message.Body as T;
                    messageReceiver(result);
                    StaticReceiveQueue.BeginReceive();
                }
                catch (Exception exception)
                {
                    //Console.WriteLine(exception);
                    throw;
                }
            };
            StaticReceiveQueue.BeginReceive();
        }
        #endregion
    }
}
