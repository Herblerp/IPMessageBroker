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
            Publisher publisher = Publisher.Instance;

            Connection conn = Connection.Instance;

            conn.OpenConnection("amqPlanning", "amqPlanning", "10.3.56.10", "", null);

            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");

            Thread.Sleep(20000);
            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");
            conn.CloseConnection();
            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");
            publisher.NewMessage("MijnError", "errorEx");

            

            //TestUtilities util = new TestUtilities();

            //util.SendAllMessagesOnce();

            log.LogMessage("Press any key to stop the program.", "info");
            Console.ReadLine();

            conn.CloseConnection();
        }
    }
}
