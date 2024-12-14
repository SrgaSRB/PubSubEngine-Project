using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Contracts
{
    //Defines operations for Publisher clients
    [ServiceContract]
    public interface IPublisherService
    {
        [OperationContract]
        void Publish(string topic, Alarm alarm);

        [OperationContract]
        void RegisterPublisher(string topic);
    }
}
