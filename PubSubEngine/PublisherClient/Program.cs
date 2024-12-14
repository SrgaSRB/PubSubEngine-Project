using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Contracts;
using PublisherClient;
using PubSubEngine;

namespace PublisherClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var binding = new NetTcpBinding();
            var address = new EndpointAddress("net.tcp://localhost:8080/Publisher");

            var factory = new ChannelFactory<IPublisherService>(binding, address);
            var proxy = factory.CreateChannel();

            proxy.RegisterPublisher("Weather");
            proxy.Publish("Weather", new Alarm(DateTime.Now, "It's sunny today!", 5));
            proxy.Publish("Weather", new Alarm(DateTime.Now, "Storm is coming!", 80));

            while (true)
            {
                Console.WriteLine("1 -> Send message to subscribrs");
                Console.WriteLine("0 -> Exit");

                string command = Console.ReadLine();
                if (command == "0")
                {
                    break;
                }
                else if (command == "1")
                {
                    Console.Write("Topic: ");
                    string topic = Console.ReadLine();
                    Console.Write("Message: ");
                    string msg = Console.ReadLine();
                    Console.WriteLine("Risk level: ");
                    string riskLevel = Console.ReadLine();
                    proxy.Publish(topic, new Alarm(DateTime.Now, msg, int.Parse(riskLevel)));
                }


            }

            Console.ReadLine();
        }
    }
}
