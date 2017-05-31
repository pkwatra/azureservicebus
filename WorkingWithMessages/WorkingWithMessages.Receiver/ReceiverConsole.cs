using Microsoft.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using WorkingWithMessages.Config;
using WorkingWithMessages.DataContracts;
using System.Threading;

namespace WorkingWithMessages.Receiver
{
    class ReceiverConsole
    {

        private static QueueClient m_QueueClient;

        static void Main(string[] args)
        {
            Console.WriteLine("Receiver Console - Hit enter");
            Console.ReadLine();
            CreateQueue();


            //ToDo: Comment in the appropriate method
            //ReceiveAndProcessCharacters(1);
            //ReceiveAndProcessCharacters(16);

            SimplePizzaReceiveLoop();

            //ReceiveAndProcessPizzaOrdesUsingOnMessage(1);
            //ReceiveAndProcessPizzaOrdesUsingOnMessage(5);
            //ReceiveAndProcessPizzaOrdesUsingOnMessage(100);



            Console.WriteLine("Receiving, hit enter to exit");
            Console.ReadLine();
            StopReceiving();

        }

        static void SimplePizzaReceiveLoop()
        {
            // Create a queue client
            QueueClient client = QueueClient.CreateFromConnectionString
                (Settings.ConnectionString, Settings.QueueName);

            while (true)
            {
                Console.WriteLine("Receiving...");

                // Receive a message
                BrokeredMessage message = client.Receive(TimeSpan.FromSeconds(5));

                if (message != null)
                {
                    try
                    {
                        // Process the message
                        PizzaOrder order = message.GetBody<PizzaOrder>();

                        // Process the message
                        CookPizza(order);

                        // Mark the message as complete
                        message.Complete();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex.Message);

                        // Abandon the message
                        message.Abandon();

                        // Deadletter the message
                        //message.DeadLetter();

                        // Or do nothing
                    }
                }
                else
                {
                    Console.WriteLine("No message prsent on queue.");
                }
            }
        }

        private static void CookPizza(PizzaOrder order)
        {
            Console.WriteLine("Cooking {0} for {1}.", order.Type, order.CustomerName);
            Thread.Sleep(5000);
            Console.WriteLine("    {0} pizza for {1}.", order.Type, order.CustomerName);
        }

   

        static void ReceiveAndProcessPizzaOrdesUsingOnMessage(int threads)
        {
            // Create a new client
            m_QueueClient = QueueClient.CreateFromConnectionString
                (Settings.ConnectionString, Settings.QueueName);

            // Set the options for using OnMessage
            var options = new OnMessageOptions()
            {
                AutoComplete = false,
                MaxConcurrentCalls = threads,
                AutoRenewTimeout = TimeSpan.FromSeconds(30)
            };

            // Create a message pump using OnMessage
            m_QueueClient.OnMessage(message =>
            {              
                // Deserializse the message body
                var order = message.GetBody<PizzaOrder>();

                // Process the message
                CookPizza(order);

                // Complete the message
                message.Complete();

            }, options);


            Console.WriteLine("Receiving, hit enter to exit");
            Console.ReadLine();
            StopReceiving();
        }



        static void ReceiveAndProcessCharacters(int threads)
        {
            // Create a queue client
            m_QueueClient = QueueClient.CreateFromConnectionString(Settings.ConnectionString, Settings.QueueName);

            // Create options for OnMessage
            OnMessageOptions options = new OnMessageOptions()
            {
                AutoComplete = false,
                MaxConcurrentCalls = threads
            };

            // Create a message pump
            m_QueueClient.OnMessage(message =>
            {
                Console.Write(message.Label);
                message.Complete();
            }, options);
        }



        static void StopReceiving()
        {
            // Close the client, which will stop the message pump.
            m_QueueClient.Close();
        }


        static void CreateQueue()
        {
            var manager = NamespaceManager.CreateFromConnectionString(Settings.ConnectionString);
            if (!manager.QueueExists(Settings.QueueName))
            {
                Console.Write("Creating queue: " + Settings.QueueName + "...");
                manager.CreateQueue(Settings.QueueName);
                Console.WriteLine("Done!");
            }
        }
    }
}
