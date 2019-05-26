using MessageBroker;
using System;
using System.Threading;


namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //Logs zijn niet langer in kleur door bugged afou

            //Get instance
            Log log = Log.Instance;
            //###########BELANGRIJK########## zet log titel! eerste da ge moet doen!
            log.SetLogFileTitle("Sender");
            //Wel of niet tonen van debug msgs
            log.ShowDebugMessages(true);
            //Awesome banner
            log.Welcome();
            Publisher publisher = Publisher.Instance;
            Connection conn = Connection.Instance;

            MessageHandler hndl = new MessageHandler();

            //Openconnection heeft nu een overload voor de sender!
            //Open connectie zonder reciever.
            //conn.OpenConnection("amqPlanning", "amqPlanning", "10.3.56.10");
            //Open connectie met receiver.
            conn.OpenConnection("amqPlanning", "amqPlanning", "10.3.56.10", "Planning", hndl);
            //Zet keepalive aan (kan op zich op eender welk moment worden aangezet GEBRUIK KEEPALIVE NAMESPACE!)
            Thread.Sleep(2000);
            publisher.NewMessage("Hello world!", "amq.fanout");

            //conn.EnableKeepAlive(KeepAlive.SysteemNaam.Planning , "BrokerTest :) ");

            log.LogMessage("Press any key to stop the program.", "info");
            Console.ReadLine();

            conn.CloseConnection();
        }
    }
}
