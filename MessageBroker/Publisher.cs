using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace MessageBroker
{
    public class Publisher
    {
        private static Log log = Log.Instance;

        private static Publisher _instance;
        private static readonly object _padlock = new object();

        private List<string> _cachedMessages;
        private Timer _sendCachedMessagesTimer;
        private AutoResetEvent _sendCachedMessagesAutoEvent;

        private const int _sendCachedMessagesInterval = 30000;

        private Publisher()
        {
            _cachedMessages = new List<string>();
            _sendCachedMessagesAutoEvent = new AutoResetEvent(false);
            _sendCachedMessagesTimer = new Timer(PublishCachedMessages, _sendCachedMessagesAutoEvent, _sendCachedMessagesInterval, _sendCachedMessagesInterval);
            log.LogMessage("Publisher initialized, checking cache every " + _sendCachedMessagesInterval, "debug");
        }

        public static Publisher Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        log.LogMessage("Creating publisher instance.", "debug");
                        _instance = new Publisher();
                    }
                    return _instance;
                }
            }
        }

        public void NewMessage(string msg)
        {
            log.LogMessageType("Publishing message of type: ", msg.GetType().Name);

            CacheMessage(msg);
            PublishCachedMessages(msg);
        }

        private bool PublishMessage(string msg)
        {
            try
            {
                if (Connection.Instance.IsConnected())
                {
                    var body = Encoding.UTF8.GetBytes(msg);

                    var properties = Connection.Instance.PublisherChannel.CreateBasicProperties();
                    properties.Persistent = true;

                    Connection.Instance.PublisherChannel.BasicPublish(exchange: "amq.fanout",
                                         routingKey: "",
                                         basicProperties: properties,
                                         body: body);

                    log.LogMessage("Message sent to server.", "info");

                    return true;
                }
                else
                {
                    log.LogMessage("Can not publish message, connection is closed.", "warning");
                    return false;
                }
            }
            catch (Exception e)
            {
                log.LogMessage("Failed to send message. " + e.GetType(), "error");
                return false;
            }
        }

        private void CacheMessage(string msg)
        {
            _cachedMessages.Add(msg);
        }

        private void PublishCachedMessages(Object stateInfo)
        {
            lock (_padlock)
            {
                if (_cachedMessages.Count > 0)
                {
                    log.LogMessage("Trying to publish " + _cachedMessages.Count + " cached messages.", "info");

                    while (_cachedMessages.Count > 0 && PublishMessage(_cachedMessages[0]))
                    {
                        _cachedMessages.RemoveAt(0);
                    }
                }
                else
                {
                    log.LogMessage("No cached messages found. Checking again in " + _sendCachedMessagesInterval / 1000 + "s.", "info");
                }
            }
        }
    }
}
