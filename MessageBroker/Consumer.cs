using RabbitMQ.Client.Events;
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

        private void EnableConsumer(string queueName)
        {
            try
            {
                var consumer = new EventingBasicConsumer(Connection.Instance.);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    HandleMessage(message);

                    consumerChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                consumerChannel.BasicConsume(queue: queueName,
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