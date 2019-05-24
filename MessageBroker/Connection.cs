using KeepAlive;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace MessageBroker
{
    public class Connection
    {
        #region Variables

        private static Log log = Log.Instance;

        private static Connection _instance;
        private static readonly object _padlock = new object();

        private IConnection _connection;
        private IModel _consumerChannel;
        private IModel _publisherChannel;
        private IMessageHandler _messageHandler;

        private bool _isConnecting = false;
        private bool _isConnected = false;
        private bool _keepAlive = false;

        private string _userName;
        private string _password;
        private string _hostName;
        private string _queueName;
        private SysteemNaam _system;
        private string _subsystem;

        private Timer _retryConnectionTimer;
        private AutoResetEvent _retryConnectionAutoEvent;

        private Timer _keepAliveTimer;
        private AutoResetEvent _keepAliveAutoEvent;

        private const int _hearbeatInterval = 5;
        private const int _connectionTimeoutInterval = 5000;
        private const int _retryConnectionInterval = 15000;
        private const int _keepAliveInterval = 1000;

        #endregion

        public static Connection Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        log.LogMessage("Creating connection instance.", "debug");
                        _instance = new Connection();
                    }
                    return _instance;
                }
            }
        }
        internal IModel PublisherChannel
        {
            get
            {
                return _publisherChannel;
            }
        }

        public bool IsConnected()
        {
            if (_connection != null)
            {
                return _connection.IsOpen;
            }
            return false;
        }

        public void EnableKeepAlive(SysteemNaam system, string subsystem)
        {
            if (!_keepAlive)
            {
                _system = system;
                _subsystem = subsystem;

                _keepAliveAutoEvent = new AutoResetEvent(false);
                _keepAliveTimer = new Timer(KeepAlive, _keepAliveAutoEvent, 0, _keepAliveInterval);
            }
        }

        public void OpenConnection(string userName, string password, string hostName)
        {
            if (!_isConnecting && !_isConnected)
            {
                log.LogMessage("Creating connection.", "info");
                _isConnecting = true;

                _userName = userName;
                _password = password;
                _hostName = hostName;

                _retryConnectionAutoEvent = new AutoResetEvent(false);
                _retryConnectionTimer = new Timer(Connect, _retryConnectionAutoEvent, 0, _retryConnectionInterval);
            }
            else
            {
                log.LogMessage("Creating connection while already connected or connecting. Please close the connection first.", "error");
            }
        }

        public void OpenConnection(string userName, string password, string hostName, string queueName, IMessageHandler handler)
        {
            if (!_isConnecting && !_isConnected)
            {
                log.LogMessage("Creating connection.", "info");
                _isConnecting = true;

                _userName = userName;
                _password = password;
                _hostName = hostName;
                _queueName = queueName;

                _messageHandler = handler;

                _retryConnectionAutoEvent = new AutoResetEvent(false);
                _retryConnectionTimer = new Timer(Connect, _retryConnectionAutoEvent, 0, _retryConnectionInterval);
            }
            else
            {
                log.LogMessage("Creating connection while already connected or connecting. Please close the connection first.", "error");
            }
        }

        public void CloseConnection()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Close();
                _consumerChannel.Dispose();
                _consumerChannel = null;
            }

            if (_publisherChannel != null)
            {
                _publisherChannel.Close();
                _publisherChannel.Dispose();
                _publisherChannel = null;
            }

            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
            _isConnected = false;

            log.LogMessage("Connection closed.", "info");
        }

        private void Connect(Object stateInfo)
        {
            if (!_isConnected)
            {
                ConnectionFactory factory = new ConnectionFactory()
                {
                    UserName = _userName,
                    Password = _password,
                    HostName = _hostName,
                    RequestedConnectionTimeout = _connectionTimeoutInterval,
                    RequestedHeartbeat = _hearbeatInterval
                };

                try
                {
                    log.LogMessage("Connecting to RabbitMQ server.", "info");
                    _connection = factory.CreateConnection();
                    _consumerChannel = _connection.CreateModel();
                    _publisherChannel = _connection.CreateModel();

                    _retryConnectionTimer.Dispose();
                    _retryConnectionAutoEvent.Dispose();

                    _retryConnectionTimer = null;
                    _retryConnectionAutoEvent = null;

                    _isConnecting = false;
                    _isConnected = true;

                    if (_messageHandler != null)
                    {
                        EnableConsumer();
                    }
                    else
                    {
                        log.LogMessage("Message handler is null. Consumer will not be enabled.", "warning");
                    }
                    log.LogMessage("Successfully connected to RabbitMQ server.", "info");
                }
                catch (BrokerUnreachableException e)
                {
                    log.LogMessage("Connection failed: " + e.Message + ". Retrying in " + _retryConnectionInterval / 1000 + " seconds.", "error");
                }
            }
            else
            {
                log.LogMessage("Called connect while already connected.", "debug");
            }
        }

        private void EnableConsumer()
        {
            try
            {
                var consumer = new EventingBasicConsumer(_consumerChannel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    _messageHandler.HandleMessage(message);

                    _consumerChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                _consumerChannel.BasicConsume(queue: _queueName,
                                     autoAck: false,
                                     consumer: consumer); 
            }
            catch (OperationInterruptedException e)
            {
                log.LogMessage("Failed to enable consumer: " + e.Message + ".", "error");
            }
        }

        private void KeepAlive(Object stateInfo)
        {
            if (_isConnected)
            {
                KeepAliveMessage keepAliveMessage = new KeepAliveMessage
                {
                    header = new KeepAliveMessageHeader
                    {
                        sender = _system,
                        timestamp = DateTime.Now,
                        versie = 0
                    },
                    body = new KeepAliveMessageBody
                    {
                        systeem = _system,
                        subsysteem = _subsystem
                    }
                };
                log.LogMessage("Sending keepalive message.", "info");
                Publisher.Instance.NewMessage(SerializeMessage(keepAliveMessage), "errorEx");
            }
        }

        private string SerializeMessage(Object message)
        {
            XmlSerializer mySerializer = new XmlSerializer(message.GetType());
            StringWriter writer = new StringWriter();
            string typeoiu = message.GetType().ToString();
            mySerializer.Serialize(writer, message);
            string xmlMessage = writer.ToString();

            return xmlMessage;
        }

        private Connection()
        {

        }
    }
}


