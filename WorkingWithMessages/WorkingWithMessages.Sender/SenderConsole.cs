using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkingWithMessages.Config;
using WorkingWithMessages.DataContracts;

namespace WorkingWithMessages.Sender
{
    class SenderConsole
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sender Console - Hit enter");
            Console.ReadLine();

            //ToDo: Comment in the appropriate method
            //SendTextString(Settings.TextString, false);
            //SendControlMessage();
            //SendPizzaOrder();
            SendPizzaOrderBatch();

            Console.WriteLine("Sender Console - Complete");
            Console.ReadLine();
        }




        static void SendTextString(string text, bool sendSync)
        {
            // Create a client
            var client = QueueClient.CreateFromConnectionString
                (Settings.ConnectionString, Settings.QueueName);

            Console.Write("Sending: ");

            var taskList = new List<Task>();

            foreach (var letter in text.ToCharArray())
            {
                // Create an empty message and set the lable.
                var message = new BrokeredMessage();
                message.Label = letter.ToString();

                if (sendSync)
                {
                    // Send the message
                    client.Send(message);
                    Console.Write(message.Label);
                }
                else
                {
                    // Create a task to send the message
                    //var sendTask = new Task(() =>
                    //{
                    //    client.Send(message);
                    //    Console.Write(message.Label);
                    //});
                    //sendTask.Start();
                    taskList.Add(client.SendAsync(message).ContinueWith
                        (t => Console.WriteLine("Sent: " + message.Label)));
                    //Console.Write(message.Label);
                }

            }

            if (!sendSync)
            {
                Console.WriteLine("Waiting...");
                Task.WaitAll(taskList.ToArray());
                Console.WriteLine("Complete!");
            }

            Console.ReadLine();
            Console.WriteLine();

            // Always close the client
            client.Close();
        }

        static void SlideCode()
        {
            var client = TopicClient.CreateFromConnectionString("");
            var message = new BrokeredMessage();

            // Veryfy the size of the message body.
            if (message.Size > 250 * 1024)
            {
                throw new ArgumentException("Message is too large");
            }

            // Send synchronously
            client.Send(message);

            // Always close the client
            client.Close();


            // Send assynchronously
            var sendTask = client.SendAsync(message).ContinueWith
                (t => Console.WriteLine("Sent: " + message.Label));

       


            // Always close the client
            client.Close();



        }

        static void SendControlMessage()
        {

            // Create a message with no body.
            var message = new BrokeredMessage()
            {
                Label = "Control"
            };

            // Add some properties to the property collection
            message.Properties.Add("SystemId", 1462);
            message.Properties.Add("Command", "Pending Restart");
            message.Properties.Add("ActionTime", DateTime.UtcNow.AddHours(2));

            // Send the message
            var client = QueueClient.CreateFromConnectionString
                (Settings.ConnectionString, Settings.QueueName);
            Console.Write("Sending control message...");
            client.Send(message);
            Console.WriteLine("Done!");


            Console.WriteLine("Send again?");
            var response = Console.ReadLine();

            if (response.ToLower().StartsWith("y"))
            {
                // Try to send the message a second time...
                Console.Write("Sending control message again...");
                message = message.Clone();
                client.Send(message);
                Console.WriteLine("Done!");
            }

            client.Close();

        }

        static void SendPizzaOrder()
        {
            var order = new PizzaOrder()
            {
                CustomerName = "Alan Smith",
                Type = "Hawaiian",
                Size = "Large"
            };

            // Create a BrokeredMessage
            var message = new BrokeredMessage(order)
            {
                Label = "PizzaOrder"
            };

            // What size is the message?
            Console.WriteLine("Message size: " + message.Size);

            // Send the message...
            var client = QueueClient.CreateFromConnectionString
                (Settings.ConnectionString, Settings.QueueName);
            Console.Write("Sending order...");
            client.Send(message);
            Console.WriteLine("Done!");
            client.Close();

            // What size is the message now?
            Console.WriteLine("Message size: " + message.Size);


        }

        static void SendPizzaOrderBatch()
        {
            // Create some data
            string[] names = { "Pawan", "Ravish", "Vinay" };
            string[] pizzas = { "Hawaiian", "Vegitarian", "Capricciosa", "Napolitana" };

            // Create a queue client
            var client = QueueClient.CreateFromConnectionString
                (Settings.ConnectionString, Settings.QueueName);


            // Send a batch of pizza orders
            var taskList = new List<Task>();
            for (int pizza = 0; pizza < pizzas.Length; pizza++)
            {
                for (int name = 0; name < names.Length; name++)
                {
                    Console.WriteLine("{0} ordered {1}", names[name], pizzas[pizza]);
                    PizzaOrder order = new PizzaOrder()
                    {
                        CustomerName = names[name],
                        Type = pizzas[pizza],
                        Size = "Large"
                    };
                    var message = new BrokeredMessage(order);

                    taskList.Add(client.SendAsync(message));
                }
            }
            //Console.WriteLine("Sending batch...");
            Task.WaitAll(taskList.ToArray());
            Console.WriteLine("Sent!");


        }
    }
}
