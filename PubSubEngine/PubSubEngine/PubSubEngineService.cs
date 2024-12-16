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
    public class PubSubEngineService : IPublisherService, ISubscriberService, ITopicService
    {
        private static readonly Dictionary<string, List<ISubscriberCallback>> topicSubscribers = new();
        private static readonly List<string> topics = new List<string>();
        private static readonly Dictionary<string, List<Alarm>> alarms = new();

        public bool Publish(string topic, Alarm alarm)
        {
            try
            {
                if (!alarms.ContainsKey(topic))
                {
                    alarms[topic] = new List<Alarm>();
                }
                alarms[topic].Add(alarm);

                NotifySubscribers(topic, alarm);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Publish error");
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool Subscribe(string topic, int minRisk, int maxRisk)
        {
            try
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
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Subscriber failed to subscribe to topic {0}", topic);
                Console.WriteLine(e.Message);
                return false;
            }
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
                NotifySubscribersOfNewPublisher(topic, true);
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
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error notifying subscriber for topic '{topic}'.");
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private void NotifySubscribersOfNewPublisher(string topic, bool notifyType)
        {
            foreach (var subscriberList in topicSubscribers.Values)
            {
                foreach (var subscriber in subscriberList)
                {
                    try
                    {
                        if (notifyType)
                        {
                            subscriber.NewPublisher(topic);
                        }
                        else
                        {
                            subscriber.LogOutPublisher(topic);
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Error notifying subscriber about new publisher for topic '{topic}'.");
                    }
                }
            }
        }

        public List<string> getAllTopics()
        {
            return topics;
        }

        public bool LogOutPublisher(string topic)
        {
            try
            {
                if (topics.Contains(topic))
                {
                    topics.Remove(topic);
                    NotifySubscribersOfNewPublisher(topic, false);
                    Console.WriteLine("The publisher who was registered on the [{0}] has successfully logged out.", topic);
                    return true;
                }
                Console.WriteLine("ERROR: Failed to logout publisher from topic [{0}], topic was not found!", topic);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Failed to logout publisher from topic [{0}]", topic);
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
