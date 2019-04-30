using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBroker
{
    public interface IMessageHandler
    {
        void HandleMessage(string msg);
    }
}
