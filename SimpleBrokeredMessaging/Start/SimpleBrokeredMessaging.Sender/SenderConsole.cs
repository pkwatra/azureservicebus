using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.Sender
{
    class SenderConsole
    {
        //ToDo: Enter a valid Serivce Bus connection string
        static string ConnectionString = "";
        static string QueuePath = "demoqueue";


        static void Main(string[] args)
        {
            // Create a queue client
            var queueClient = QueueClient.CreateFromConnectionString
                (ConnectionString, QueuePath);

            for (int i = 0; i < 10; i++)
            {
                var message = new BrokeredMessage("Message: " + i);
                queueClient.Send(message);
                Console.WriteLine("Sent: " + i);
            }
            queueClient.Close();




            Console.ReadLine();

        }
    }
}
