using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace SubscriberClient
{
    //Communication with the PubSubEngine service
    internal class SubscriberCallback : ISubscriberCallback
    {
        public void ReceiveAlarm(string topic, Alarm alarm)
        {
            Console.WriteLine($"Alarm received for topic '{topic}': {alarm}");
        }

        public void NewPublisher(string topic)
        {
            Console.WriteLine($"New publisher registered for topic '{topic}'.");
        }
    }
}
