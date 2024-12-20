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
        bool Publish(string topic, Alarm alarm);

        [OperationContract]
        void RegisterPublisher(string topic, byte[] sign);

        [OperationContract]
        bool LogOutPublisher(string topic, byte[] sign);

    }
}
