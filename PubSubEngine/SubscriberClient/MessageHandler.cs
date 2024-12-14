using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriberClient
{
    //Verification of digital signature of messages
    //Message decryption (AES)
    //Storing messages in a text database
    //Logging to Windows Event Log
    public class MessageHandler
    {
        public void HandleMessage(string topic, string message)
        {
            Console.WriteLine($"Subscriber received message on topic '{topic}': {message}");
        }
    }

}
