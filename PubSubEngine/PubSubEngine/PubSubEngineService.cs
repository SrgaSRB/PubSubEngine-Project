using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace PubSubEngine
{
    //Implement IPublisherService and ISubscriberService
    //Authenticates and validates clients
    //Distributes messages to Subscribers based on their subscriptions
    public class PubSubEngineService : IPublisherService, ISubscriberService
    {
    }
}
