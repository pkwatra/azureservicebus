using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.Receiver
{
    class ReceiverConsole
    {
        //ToDo: Enter a valid Serivce Bus connection string
        static string ConnectionString = "Endpoint=sb://dynamiccrmdemo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=B2yyvM265OqwnyJFpcrsmw3V6/eXMFJA6ud2iPHOI98=";
        static string QueuePath = "myqueue";


        static void Main(string[] args)
        {
            // Create a queue client
            var queueClient = QueueClient.CreateFromConnectionString(ConnectionString, QueuePath);

            // Create a message pump to receive messages
            queueClient.OnMessage(msg => ProcessMessage(msg));

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
            queueClient.Close();
        }

        static void ProcessMessage(BrokeredMessage message)
        {
            
            string text = message.GetBody<string>();

            Console.WriteLine("Received: " + text);
        }
    }
}
