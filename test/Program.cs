using MessageBroker;
using Messages;
using System;
using System.Threading;


namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Log log = Log.Instance;
            Connection conn = Connection.Instance;
            Publisher publisher = Publisher.Instance;

            log.ShowDebugMessages(true);
            log.Welcome();

            IMessageHandler messageHandler = new MessageHandler();
            conn.OpenConnection("amqPlanning", "amqPlanning", "10.3.56.10", "Planning", null);

            Thread.Sleep(1000);

            TestUtilities util = new TestUtilities();

            util.SendAllMessagesOnce();

            log.LogMessage("Press any key to stop the program.", "info");
            Console.ReadLine();

            conn.CloseConnection();
        }
    }
}
