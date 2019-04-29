using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;
using System.Threading;
using System.Xml;
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

        private string _userName;
        private string _password;
        private string _hostName;
        private string _queueName;

        private Timer _retryConnectionTimer;
        private AutoResetEvent _retryConnectionAutoEvent;

        private const int _hearbeatInterval = 5;
        private const int _connectionTimeoutInterval = 5000;
        private const int _retryConnectionInterval = 30000;

        #endregion

        public IModel ConsumerChannel
        {
            get
            {
                if (_isConnected)
                {
                    return _consumerChannel;
                }
                return null;
            }
        }

        public IModel PublisherChannel
        {
            get
            {
                if (_isConnected)
                {
                    return _publisherChannel;
                }
                return null;
            }
        }

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

        public bool IsConnected()
        {
            if (_connection != null)
            {
                return _connection.IsOpen;
            }
            return false;
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
            log.LogMessage("Closing connection", "debug");
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

                    log.LogMessage("Successfully connected to RabbitMQ server.", "info");
                }
                catch (BrokerUnreachableException e)
                {
                    log.LogMessage("Failed to connect to RabbitMQ server: " + e.Message + ".", "error");
                    log.LogMessage("Retrying in " + _retryConnectionInterval / 1000 + " seconds.", "info");
                    CloseConnection();
                }
            }
            else
            {
                log.LogMessage("Called connect while already connected.", "debug");
            }
        }
    }
}


