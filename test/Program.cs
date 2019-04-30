using MessageBroker;
using MessageBroker.Messages;
using System;
using System.Threading;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Log log = Log.Instance;

            log.Welcome();
            log.ShowDebugMessages(true);

            Connection conn = Connection.Instance;
            Publisher publisher = Publisher.Instance;
            IMessageHandler messageHandler = new test();

            conn.OpenConnection("amqPlanning", "amqPlanning", "10.3.56.10", messageHandler);

            AankoopMessage aankoopMessage = new AankoopMessage
            {
                header = new AankoopMessageHeader { },
                body = new AankoopMessageBody
                {
                    aankoopUUID = Guid.NewGuid()
                }
            };

            BadgeMessage badgeMessage = new BadgeMessage
            {
                header = new BadgeMessageHeader { },
                body = new BadgeMessageBody
                {
                    badgeUUID = Guid.NewGuid()
                }
            };

            BezoekerMessage bezoekerMessage = new BezoekerMessage
            {
                header = new BezoekerMessageHeader { },
                body = new BezoekerMessageBody
                {
                    bezoekerUUID = Guid.NewGuid()
                }
            };

            PingMessage pingMessage = new PingMessage
            {
                header = new PingMessageHeader
                {
                    timestamp = DateTime.Now,
                    versie = "1"
                },
                body = new PingMessageBody
                {
                    //To create a new GUID use this ONE LINE of code
                    pingUUID = Guid.NewGuid()
                }
            };

            EventMessage eventMessage = new EventMessage
            {
                header = new EventMessageHeader { },
                body = new EventMessageBody
                {
                    eventUUID = Guid.NewGuid()
                }
            };

            Thread.Sleep(1000);
                publisher.NewMessage(aankoopMessage);
                publisher.NewMessage(badgeMessage);
                publisher.NewMessage(bezoekerMessage);
                publisher.NewMessage(eventMessage);
                publisher.NewMessage(pingMessage);
            Console.ReadLine();
        }
    }
}
