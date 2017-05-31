using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingWithMessages.Config
{
    public class Settings
    {
        //ToDo: Enter a valid Serivce Bus connection string
        public static string ConnectionString = "Endpoint=sb://messagedynamiccrm.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=39PaYBEJitBikBLHZ6bJHQ9pYU2rUTabXOu/pJ3nUms=";
        public static string QueueName = "workingwithmessages";
        public static string ForwardedQueueName = "forwardedmessages";



        public static string TextString = "The quick brown fox jumps over the lazy dog";
    }
}
