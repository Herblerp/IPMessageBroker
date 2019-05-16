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
            log.SetLogFileTitle("Sender");
            log.ShowDebugMessages(true);
            log.Welcome();
            Connection conn = Connection.Instance;
            Publisher publisher = Publisher.Instance;

            

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
