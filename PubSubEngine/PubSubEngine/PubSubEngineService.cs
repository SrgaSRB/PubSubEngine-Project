using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace PubSubEngine
{
    //Implement IPublisherService and ISubscriberService
    //Authenticates and validates clients
    //Distributes messages to Subscribers based on their subscriptions
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PubSubEngineService : IPublisherService, ISubscriberService
    {
        private static readonly Dictionary<string, List<ISubscriberCallback>> topicSubscribers = new();
        private static readonly List<string> topics = new();
        private static readonly Dictionary<string, List<Alarm>> alarms = new();

        public void Publish(string topic, Alarm alarm)
        {
            // Dodavanje alarma za određeni topik
            if (!alarms.ContainsKey(topic))
            {
                alarms[topic] = new List<Alarm>();
            }
            alarms[topic].Add(alarm);

            // Obaveštavanje pretplaćenih subskrajbera
            NotifySubscribers(topic, alarm);
        }

        public void Subscribe(string topic, int minRisk, int maxRisk)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ISubscriberCallback>();

            if (!topicSubscribers.ContainsKey(topic))
            {
                topicSubscribers[topic] = new List<ISubscriberCallback>();
            }

            if (!topicSubscribers[topic].Contains(callback))
            {
                topicSubscribers[topic].Add(callback);
            }

            Console.WriteLine($"Subscriber subscribed to topic '{topic}' with risk range {minRisk}-{maxRisk}.");
        }

        public void Unsubscribe(string topic)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ISubscriberCallback>();

            if (topicSubscribers.ContainsKey(topic))
            {
                topicSubscribers[topic].Remove(callback);

                Console.WriteLine($"Subscriber unsubscribed from topic '{topic}'.");
            }
        }

        public void RegisterPublisher(string topic)
        {
            if (!topics.Contains(topic))
            {
                topics.Add(topic);
                NotifySubscribersOfNewPublisher(topic);
            }
        }

        private void NotifySubscribers(string topic, Alarm alarm)
        {
            if (topicSubscribers.ContainsKey(topic))
            {
                foreach (var subscriber in topicSubscribers[topic])
                {
                    try
                    {
                        subscriber.ReceiveAlarm(topic, alarm);
                    }
                    catch
                    {
                        Console.WriteLine($"Error notifying subscriber for topic '{topic}'.");
                    }
                }
            }
        }

        private void NotifySubscribersOfNewPublisher(string topic)
        {
            foreach (var subscriberList in topicSubscribers.Values)
            {
                foreach (var subscriber in subscriberList)
                {
                    try
                    {
                        subscriber.NewPublisher(topic);
                    }
                    catch
                    {
                        Console.WriteLine($"Error notifying subscriber about new publisher for topic '{topic}'.");
                    }
                }
            }
        }

    }
}
