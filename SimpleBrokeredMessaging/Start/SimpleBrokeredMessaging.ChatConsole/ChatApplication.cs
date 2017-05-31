using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.ChatConsole
{
    class ChatApplication
    {
        //ToDo: Enter a valid Serivce Bus connection string
        static string ConnectionString = "";
        static string TopicPath = "chattopic";

        static void Main(string[] args)
        {
            Console.WriteLine("Enter name:");
            var userName = Console.ReadLine();

            // Create a namespace manager to manage artifacts
            var manager = NamespaceManager.CreateFromConnectionString(ConnectionString);

            // Create a topic if it does not exist
            if (!manager.TopicExists(TopicPath))
            {
                manager.CreateTopic(TopicPath);
            }

            // Create a subscription for the user
            var description = new SubscriptionDescription(TopicPath, userName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
            };
            manager.CreateSubscription(description);


            // Create clients
            var factory = MessagingFactory.CreateFromConnectionString(ConnectionString);
            var topicClient = factory.CreateTopicClient(TopicPath);
            var subscriptionClient = factory.CreateSubscriptionClient(TopicPath, userName);

            // Create a message pump for receiving messages
            subscriptionClient.OnMessage(msg => ProcessMessage(msg));

            // Send a message to say you are here
            var helloMessage = new BrokeredMessage("Has entered the room...");
            helloMessage.Label = userName;
            topicClient.Send(helloMessage);

            while (true)
            {
                string text = Console.ReadLine();
                if (text.Equals("exit")) break;

                // Send a chat message
                var chatMessage = new BrokeredMessage(text);
                chatMessage.Label = userName;
                topicClient.Send(chatMessage);

            }

            // Send a message to say you are leaving
            var goodbyeMessage = new BrokeredMessage("Has left the building...");
            goodbyeMessage.Label = userName;
            topicClient.Send(goodbyeMessage);

            // Close the factory and the clients it created
            factory.Close();
        }

        static void ProcessMessage(BrokeredMessage message)
        {
            string user = message.Label;
            string text = message.GetBody<string>();

            Console.WriteLine(user + ">" + text);
        }
    }
}
