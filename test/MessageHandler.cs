using MessageBroker;

namespace Example
{
    public class MessageHandler : IMessageHandler
    {
        private Log log = Log.Instance;

        public void HandleMessage(string xmlMessage)
        {
            //log.LogMessage("Received message: " + xmlMessage, "info");

            //Serializer serializer = new Serializer();
            //object message = serializer.DeserializeMessage(xmlMessage);

            //log.LogMessage("Great success!", "info");
        }
    }
}
