using System;
using Microsoft.ServiceBus;
using RfidCheckout.Config;
using Microsoft.ServiceBus.Messaging;
using RfidCheckout.DataContracts;

namespace RfidCheckout.Checkout
{
    class CheckoutConsole
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Checkout Console");



            // Create the NamespaceManager
            NamespaceManager namespaceMgr =
                //new NamespaceManager(serviceBusUri, credentials);
                NamespaceManager.CreateFromConnectionString(AccountDetails.ConnectionString);

            // Create the MessagingFactory
            MessagingFactory factory =
                //MessagingFactory.Create(serviceBusUri, credentials);
                MessagingFactory.CreateFromConnectionString(AccountDetails.ConnectionString);

            // Delete the queue if it exists.
            if (namespaceMgr.QueueExists(AccountDetails.QueueName))
            {
                namespaceMgr.DeleteQueue(AccountDetails.QueueName);
            }



            // Create a description for the queue.
            QueueDescription rfidCheckoutQueueDescription =
                new QueueDescription(AccountDetails.QueueName)
                {
                    // Comment in to require duplicate detection
                    RequiresDuplicateDetection = true,
                    DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10),

                    // Comment in to require sessions
                    RequiresSession = true
                };



            // Create a queue based on the queue description.
            namespaceMgr.CreateQueue(rfidCheckoutQueueDescription);

            // Use the MessagingFactory to create a queue client for the 
            // specified queue.
            QueueClient queueClient = factory.CreateQueueClient(AccountDetails.QueueName);

            Console.WriteLine("Receiving tag read messages...");
            while (true)
            {
                int receivedCount = 0;
                double billTotal = 0.0;

                // Comment in to use a session receiver
                Console.ForegroundColor = ConsoleColor.Cyan;
                var messageSession = queueClient.AcceptMessageSession();
                Console.WriteLine("Accepted session: " + messageSession.SessionId);


                Console.ForegroundColor = ConsoleColor.Yellow;


                while (true)
                {
                    // Receive a tag read message.

                    // Swap comments to use a session receiver
                    //var receivedTagRead = queueClient.Receive(TimeSpan.FromSeconds(5));
                    var receivedTagRead = messageSession.Receive(TimeSpan.FromSeconds(5));

                    if (receivedTagRead != null)
                    {
                        // Process the message.
                        RfidTag tag = receivedTagRead.GetBody<RfidTag>();
                        Console.WriteLine("Bill for {0}", tag.Product);
                        receivedCount++;
                        billTotal += tag.Price;

                        // Mark the message as complete
                        receivedTagRead.Complete();
                    }
                    else
                    {
                        break;
                    }
                }

                if (receivedCount > 0)
                {
                    // Bill the customer.
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine
                        ("Bill customer ${0} for {1} items.", billTotal, receivedCount);
                    Console.WriteLine();
                    Console.ResetColor();
                }
                
            }


        }
    }
}
