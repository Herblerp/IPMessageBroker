using MessageBroker;
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

            conn.OpenConnection("amqPlanning", "amqPlanning", "10.3.56.10", "Planning", messageHandler);

            Thread.Sleep(1000);
            while (true)
            {
                publisher.NewMessage("HALLO KAKAS");
                Thread.Sleep(1000);
            }
        }
    }
}
