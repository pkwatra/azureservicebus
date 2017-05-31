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
        static string ConnectionString = "Endpoint=sb://dynamiccrmdemo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=B2yyvM265OqwnyJFpcrsmw3V6/eXMFJA6ud2iPHOI98=";
        static string QueuePath = "myqueue";


        static void Main(string[] args)
        {

            // Create a queue client
            var queueClient = QueueClient.CreateFromConnectionString(ConnectionString, QueuePath);

            // Send some messages
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
