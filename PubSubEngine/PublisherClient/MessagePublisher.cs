using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurityManager;

namespace PublisherClient
{
    //Generating messages with timestamp, risk and messages from a resource file
    //Message encryption (AES) and digital signing (RSA)
    public class MessagePublisher
    {
        private readonly Action<string, string> _publishMessage;

        public MessagePublisher(Action<string, string> publishMessage)
        {
            _publishMessage = publishMessage;
        }

        public void SendMessage(string topic, string message)
        {
            Console.WriteLine($"Publisher sending message to topic '{topic}': {message}");
            _publishMessage(topic, message);
        }
    }

}


