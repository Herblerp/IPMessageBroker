using ErrorMessageNS;
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
            Connection conn = Connection.Instance;
            Publisher publisher = Publisher.Instance;

            log.ShowDebugMessages(true);
            log.Welcome();

            IMessageHandler messageHandler = new MessageHandler();
            conn.OpenConnection("amqPlanning", "amqPlanning", "10.3.56.10", "Planning", messageHandler);

            Thread.Sleep(1000);

            ErrorMessage errorMessage = new ErrorMessage()
            {
                header = new ErrorMessageHeader()
                {
                    sender = systeemNaam.Planning,
                    timestamp = DateTime.Now,
                    versie = "1"
                },
                body = new ErrorMessageBody()
                {
                    errorBericht = "Ik ben een errorbericht"
                }
            };

            ErrorMessageHandler handler = new ErrorMessageHandler();

            publisher.NewMessage(handler.ConvertToXml(errorMessage));

            log.LogMessage("Press any key to stop the program.", "info");
            Console.ReadLine();

            conn.CloseConnection();
        }
    }
}
