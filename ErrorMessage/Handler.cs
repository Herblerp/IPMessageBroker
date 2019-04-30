using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ErrorMessageNS
{
    public class ErrorMessageHandler
    {
        public ErrorMessage ConvertToObject(string xmlMessage)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlMessage);

            XmlSerializer serializer = new XmlSerializer(typeof(ErrorMessage));
            XmlReader reader = new XmlNodeReader(doc);
            ErrorMessage errorMessage = (ErrorMessage)serializer.Deserialize(reader);

            return errorMessage;
        }

        public string ConvertToXml(ErrorMessage errorMessage)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(ErrorMessage));
            StringWriter writer = new StringWriter();
            mySerializer.Serialize(writer, errorMessage);
            string xmlMessage = writer.ToString();

            return xmlMessage;
        }
    }
}
