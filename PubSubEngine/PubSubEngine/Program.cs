using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Publisher Service na portu 8080
                var publisherService = new PubSubEngineService();
                var publisherBaseAddress = new Uri("net.tcp://localhost:8080/Publisher");
                var publisherHost = new ServiceHost(publisherService, publisherBaseAddress);
                publisherHost.AddServiceEndpoint(typeof(IPublisherService), new NetTcpBinding(), "");
                publisherHost.Open();
                Console.WriteLine("Publisher service is running on net.tcp://localhost:8080/Publisher");

                // Subscriber Service na portu 8081
                var subscriberService = new PubSubEngineService();
                var subscriberBaseAddress = new Uri("net.tcp://localhost:8081/Subscriber");
                var subscriberHost = new ServiceHost(subscriberService, subscriberBaseAddress);
                subscriberHost.AddServiceEndpoint(typeof(ISubscriberService), new NetTcpBinding(), "");
                subscriberHost.Open();
                Console.WriteLine("Subscriber service is running on net.tcp://localhost:8081/Subscriber");

                Console.WriteLine("Press Enter to stop the services...");
                Console.ReadLine();

                publisherHost.Close();
                subscriberHost.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}