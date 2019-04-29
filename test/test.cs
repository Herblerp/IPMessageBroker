using MessageBroker;
using System;
using System.Collections.Generic;
using System.Text;

namespace test
{
    public class test : IMessageHandler
    {
        public void HandleMessage(string message)
        {
            Log.Instance.LogMessage("Received message", "info");
        }
    }
}
