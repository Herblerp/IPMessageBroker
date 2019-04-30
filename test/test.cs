using MessageBroker;
using System;
using System.Xml;
using ErrorMessageNS;

namespace test
{
    public class test : IMessageHandler
    {
        public void HandleMessage(string msg)
        {
            Log.Instance.LogMessage("Received message", "info");
        }
    }
}
