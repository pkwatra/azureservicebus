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
        static string ConnectionString = "";
        static string QueuePath = "demoqueue";


        static void Main(string[] args)
        {
            // Create a queue client
            var queueClient = QueueClient.CreateFromConnectionString
                (ConnectionString, QueuePath);

            // Create a message pump to receive and process messages.
            queueClient.OnMessage(msg => ProcessMessage(msg));

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

            queueClient.Close();

        }

        private static void ProcessMessage(BrokeredMessage msg)
        {
            var text = msg.GetBody<string>();
            Console.WriteLine("Received: " + text);

        }


    }
}
