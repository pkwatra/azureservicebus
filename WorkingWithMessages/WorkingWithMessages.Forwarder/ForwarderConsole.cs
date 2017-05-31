using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using WorkingWithMessages.Config;

namespace WorkingWithMessages.Forwarder
{
    class ForwarderConsole
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Forwarder Console");
            Console.WriteLine();
            CreateQueue();
            ForwardMessages();

        }


        static void ForwardMessages()
        {
            // Create queue clients
            var inboundQueueClient = QueueClient.CreateFromConnectionString(Settings.ConnectionString, Settings.QueueName);
            var outboundQueueClient = QueueClient.CreateFromConnectionString(Settings.ConnectionString, Settings.ForwardedQueueName);

            Console.WriteLine("Forwarding messages, hit enter to exit");

            inboundQueueClient.OnMessage(message =>
            {
                // Without message cloning
                outboundQueueClient.Send(message);

                // Clone the message
                var outboundMessage = message.Clone();

                outboundQueueClient.Send(outboundMessage);

                Console.WriteLine("Forwarded message: " + message.Label);
            });

            
            Console.ReadLine();

            // Close the clients to free up connections.
            inboundQueueClient.Close();
            outboundQueueClient.Close();
        }

        static void CreateQueue()
        {
            var manager = NamespaceManager.CreateFromConnectionString(Settings.ConnectionString);
            if (!manager.QueueExists(Settings.ForwardedQueueName))
            {
                Console.Write("Creating queue: " + Settings.ForwardedQueueName + "...");
                manager.CreateQueue(Settings.ForwardedQueueName);
                Console.WriteLine("Done!");
            }
        }
    }
}
