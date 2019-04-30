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
            Console.WriteLine("AankoopUUID is: " + aankoopMessage.body.aankoopUUID);
        }

        public void HandleBadgeMessage(BadgeMessage badgeMessage)
        {
            Console.WriteLine("BadgeUUID is: " + badgeMessage.body.badgeUUID);
        }

        public void HandleBezoekerMessage(BezoekerMessage bezoekerMessage)
        {
            Console.WriteLine("BezoekerUUID is: " + bezoekerMessage.body.bezoekerUUID);
        }

        public void HandleEventMessage(EventMessage eventMessage)
        {
            Console.WriteLine("EventUUID is: " + eventMessage.body.eventUUID);
        }

        public void HandlePingMessage(PingMessage pingMessage)
        {
            Console.WriteLine(pingMessage.header.timestamp);
            Console.WriteLine(pingMessage.header.versie);
            Console.WriteLine(pingMessage.header.type);
            Console.WriteLine(pingMessage.body.pingUUID);
        }
    }
}
