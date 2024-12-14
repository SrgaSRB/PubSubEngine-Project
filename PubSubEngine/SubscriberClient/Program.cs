using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Contracts;
using PubSubEngine;
using System.Runtime.Remoting.Contexts;

namespace SubscriberClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var callback = new SubscriberCallback();
            var context = new InstanceContext(callback);

            var binding = new NetTcpBinding();
            var address = new EndpointAddress("net.tcp://localhost:8081/Subscriber");

            var factory = new DuplexChannelFactory<ISubscriberService>(context, binding, address);
            var proxy = factory.CreateChannel();

            proxy.Subscribe("Weather", 1, 50);

            Console.ReadLine();
        }
    }
}
