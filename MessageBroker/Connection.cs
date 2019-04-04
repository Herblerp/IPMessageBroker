using MessageBroker.Messages;
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

                    EnableConsumer();

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

        private void EnableConsumer()
        {
            try
            {
                var consumer = new EventingBasicConsumer(_consumerChannel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    HandleMessage(message);

                    _consumerChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                _consumerChannel.BasicConsume(queue: _queueName,
                                     autoAck: false,
                                     consumer: consumer);
            }catch(OperationInterruptedException e)
            {
                log.LogMessage("Failed to enable consumer: " + e.Message + ".", "error");
            }
        }

        private void HandleMessage(string xmlMessage)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlMessage);

                //Get the type of the message
                string messageType = doc.DocumentElement.Name;

                log.LogMessageType("Received message of type: ",messageType);

                if (messageType == "AankoopMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(AankoopMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    AankoopMessage message = (AankoopMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleAankoopMessage(message);
                }

                else if (messageType == "BadgeMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(BadgeMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    BadgeMessage message = (BadgeMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleBadgeMessage(message);
                }

                else if (messageType == "BezoekerMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(BezoekerMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    BezoekerMessage message = (BezoekerMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleBezoekerMessage(message);
                }

                else if (messageType == "CreditnotaMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(CreditnotaMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    CreditnotaMessage message = (CreditnotaMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleCreditnotaMessage(message);

                }

                else if (messageType == "ErrorMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(ErrorMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    ErrorMessage pingMessage = (ErrorMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleErrorMessage(pingMessage);
                }

                else if (messageType == "FactuurMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(FactuurMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    FactuurMessage pingMessage = (FactuurMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleFactuurMessage(pingMessage);
                }

                else if (messageType == "EventMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(EventMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    EventMessage message = (EventMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleEventMessage(message);
                }

                else if (messageType == "GroepRegistratieMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(GroepRegistratieMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    GroepRegistratieMessage message = (GroepRegistratieMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleGroepRegistratieMessage(message);
                }

                else if (messageType == "InschrijvingsMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(InschrijvingsMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    InschrijvingsMessage message = (InschrijvingsMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleInschrijvingsMessage(message);
                }

                else if (messageType == "KalenderMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(KalenderMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    KalenderMessage message = (KalenderMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleKalenderMessage(message);
                }

                else if (messageType == "KeepAliveMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(KeepAliveMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    KeepAliveMessage message = (KeepAliveMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleKeepAliveMessage(message);
                }

                else if (messageType == "LocatieMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(LocatieMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    LocatieMessage message = (LocatieMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleLocatieMessage(message);
                }

                else if (messageType == "OplaadMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(OplaadMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    OplaadMessage message = (OplaadMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleOplaadMessage(message);
                }

                else if (messageType == "OrganisatieMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(OrganisatieMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    OrganisatieMessage message = (OrganisatieMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleOrganisatieMessage(message);
                }

                else if (messageType == "PingMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(PingMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    PingMessage message = (PingMessage)serializer.Deserialize(reader);

                    _messageHandler.HandlePingMessage(message);
                }

                else if (messageType == "RegistratieMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(RegistratieMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    RegistratieMessage message = (RegistratieMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleRegistratieMessage(message);
                }

                else if (messageType == "ReservatieMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(ReservatieMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    ReservatieMessage message = (ReservatieMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleReservatieMessage(message);
                }

                else if (messageType == "SessieMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(SessieMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    SessieMessage message = (SessieMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleSessieMessage(message);
                }

                else if (messageType == "TaakMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(TaakMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    TaakMessage message = (TaakMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleTaakMessage(message);
                }

                else if (messageType == "VerkoopsItemMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(VerkoopsItemMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    VerkoopsItemMessage message = (VerkoopsItemMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleVerkoopsItemMessage(message);
                }

                else if (messageType == "WerkMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(WerkMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    WerkMessage message = (WerkMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleWerkMessage(message);
                }

                else if (messageType == "WerknemerMessage")
                {
                    log.LogMessage("Processing message.", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(WerknemerMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    WerknemerMessage message = (WerknemerMessage)serializer.Deserialize(reader);

                    _messageHandler.HandleWerknemerMessage(message);
                }

                else
                {
                    log.LogMessage("Discarding message", "warning");
                }
            }
            catch (XmlException e)
            {
                log.LogMessage("Could not process message: " + e.GetType(), "error");
            }
        }

        private Connection()
        {

        }
    }
}


