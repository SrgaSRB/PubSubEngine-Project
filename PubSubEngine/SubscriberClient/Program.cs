using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Contracts;
using PubSubEngine;
using System.Runtime.Remoting.Contexts;
using SecurityManager;


namespace SubscriberClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            var callback = new SubscriberCallback();
            var context = new InstanceContext(callback);

            var binding = new NetTcpBinding();
            var address = new EndpointAddress("net.tcp://localhost:8081/Subscriber");

            var factory = new DuplexChannelFactory<ISubscriberService>(context, binding, address);
            var proxy = factory.CreateChannel();

            try
            {
                Console.WriteLine("Connected to server. Listening for notifications...");

                while (true)
                {

                    PrintOptions();
                    string input = Console.ReadLine();

                    switch (input)
                    {
                        case "1":
                            Console.Write("Enter topic name to subscribe: ");
                            string topicToSubscribe = Console.ReadLine();
                            SubscribeToTopic(topicToSubscribe, proxy);
                            break;

                        case "2":
                            Console.Write("Enter topic name to unsubscribe: ");
                            string topicToUnsubscribe = Console.ReadLine();
                            topicToUnsubscribe = AES_Symm_Algorithm.EncryptData(topicToUnsubscribe);
                            proxy.Unsubscribe(topicToUnsubscribe);
                            Console.WriteLine($"Unsubscribed from topic '{topicToUnsubscribe}'.");
                            break;

                        case "3":
                            PrintAllTopics(proxy.getAllTopics());
                            break;

                        case "4":
                            Console.WriteLine("Exiting program...");
                            return;

                        default:
                            Console.WriteLine("Invalid command. Try again.");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error communicating with the server.");
                Console.WriteLine(e.Message);
            }
        }

        public static void PrintAllTopics(List<string> topics)
        {
            string printVal = "Topics [";

            foreach (string topic in topics)
            {
                printVal += AES_Symm_Algorithm.DecryptData<string>(topic) + ", ";
            }

            if (topics.Count > 0)
                printVal = printVal.Substring(0, printVal.Length - 2);
            printVal += "]";

            Console.WriteLine(printVal);
        }
        public static void SubscribeToTopic(string topicSub, ISubscriberService proxy)
        {
            Console.Write("Minimun risk [for topic {0}]: ", topicSub);
            string minRisk = Console.ReadLine();
            Console.Write("Maximum risk [for topic {0}]: ", topicSub);
            string maxRisk = Console.ReadLine();


            try
            {
                if (!(int.TryParse(minRisk, out int minRiskInt) && int.TryParse(maxRisk, out int maxRiskInt)))
                {
                    Console.WriteLine("ERROR: Minimum and Maximum risk values must be numbers in range 1-100 where is Minimum risk number lower then Maximum risk number!");
                }
                else if (minRiskInt > maxRiskInt)
                {
                    Console.WriteLine("Minimum risk number must be lower then Maximum risk number!");
                }
                else if (minRiskInt < 1 || maxRiskInt > 100)
                {
                    Console.WriteLine("Minimum risk number must be lower then Maximum risk number!");
                }
                else
                {
                    try
                    {
                        minRisk = AES_Symm_Algorithm.EncryptData(minRisk);
                        maxRisk = AES_Symm_Algorithm.EncryptData(maxRisk);
                        topicSub = AES_Symm_Algorithm.EncryptData(topicSub);

                        if (proxy.Subscribe(topicSub, minRisk, maxRisk))
                        {
                            Console.WriteLine("Success subscribe to [{0}]", AES_Symm_Algorithm.DecryptData<string>(topicSub));
                        }
                        else
                        {
                            Console.WriteLine("ERROR: failed to subscribe to {0}", topicSub);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error communicating with the server");
                        Console.WriteLine(e.Message);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void PrintOptions()
        {
            Console.WriteLine("\nEnter a command: ");
            Console.WriteLine("[1] Subscribe to a topic");
            Console.WriteLine("[2] Unsubscribe from a topic");
            Console.WriteLine("[3] View all topics");
            Console.WriteLine("[4] Exit");
            Console.Write("> ");
        }
    }
}
