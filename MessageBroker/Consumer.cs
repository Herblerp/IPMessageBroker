using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBroker
{
    public class Consumer
    {
        private static Log log = Log.Instance;

        private static Consumer _instance;
        private static readonly object _padlock = new object();

        private IMessageHandler _messageHandler;

        public static Consumer Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        log.LogMessage("Creating publisher instance.", "debug");
                        _instance = new Consumer();
                    }
                    return _instance;
                }
            }
        }

        public void EnableConsumer(string queueName)
        {
            try
            {
                var consumer = new EventingBasicConsumer(Connection.Instance.ConsumerChannel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    HandleMessage(message);

                    Connection.Instance.ConsumerChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                Connection.Instance.ConsumerChannel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
            }
            catch (OperationInterruptedException e)
            {
                log.LogMessage("Failed to enable consumer: " + e.Message + ".", "error");
            }
        }

        private void HandleMessage(string xmlMessage)
        {
            _messageHandler.HandleMessage(xmlMessage);
        }
    }
}