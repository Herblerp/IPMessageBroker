using MessageBroker;
using Messages;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Example
{
    public class Serializer
    {
        Log log = Log.Instance;

        public string SerializeMessage(Object message)
        {
            XmlSerializer mySerializer = new XmlSerializer(message.GetType());
            StringWriter writer = new StringWriter();
            string typeoiu = message.GetType().ToString();
            mySerializer.Serialize(writer, message);
            string xmlMessage = writer.ToString();

            return xmlMessage;
        }

        public Object DeserializeMessage(string xmlMessage)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlMessage);
                string messageTypeString = doc.DocumentElement.Name;

                doc.Schemas.Add(null, "xsd/" + messageTypeString + ".xsd");
                doc.Validate(null);

                Type messageType = Type.GetType("Messages." + messageTypeString);

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
