using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    //Managing topics and subscriptions
    //Add/remove Publisher
    public class TopicManager
    {
        private readonly Dictionary<string, EventHandler<string>> _topicEvents = new();

        public void Subscribe(string topic, EventHandler<string> handler)
        {
            if (!_topicEvents.ContainsKey(topic))
            {
                _topicEvents[topic] = null;
            }

            _topicEvents[topic] += handler;
            Console.WriteLine($"Subscriber added to topic: {topic}");
        }

        public void Unsubscribe(string topic, EventHandler<string> handler)
        {
            if (_topicEvents.ContainsKey(topic))
            {
                _topicEvents[topic] -= handler;
                Console.WriteLine($"Subscriber removed from topic: {topic}");
            }
        }

        public void PublishMessage(string topic, string message)
        {
            Console.WriteLine($"Publishing message to topic: {topic}");
            if (_topicEvents.ContainsKey(topic))
            {
                _topicEvents[topic]?.Invoke(this, message);
            }
            else
            {
                Console.WriteLine($"No subscribers for topic: {topic}");
            }
        }
    }

}
