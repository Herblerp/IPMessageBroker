using ErrorMessageNS;
using MessageBroker;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace test
{
    public class MessageHandler : IMessageHandler
    {
        private Log log = Log.Instance;

        public void HandleMessage(string xmlMessage)
        {
            Log.Instance.LogMessage("Received message: " + xmlMessage, "info");

            object message = DeserializeMessage(xmlMessage);

        }


        private Object DeserializeMessage(string xmlMessage)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlMessage);
                string messageTypeString = doc.DocumentElement.Name;

                doc.Schemas.Add(null, "Validation/" + messageTypeString + ".xsd");
                doc.Validate(null);
                
                Type messageType = Type.GetType(messageTypeString + "NS." + messageTypeString);

                XmlSerializer serializer = new XmlSerializer(messageType);
                XmlReader reader = new XmlNodeReader(doc);
                return serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                log.LogMessage("Could not deserialize message. " + e.GetType() + e.Message, "error");
            }
            return null;
        }
    }
}
