using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    //Defines operations for Subscriber clients
    [ServiceContract(CallbackContract = typeof(ISubscriberCallback))]
    public interface ISubscriberService
    {
        [OperationContract]
        bool Subscribe(string topic, string minRisk, string maxRisk);

        [OperationContract]
        void Unsubscribe(string topic);

        [OperationContract]
        List<string> getAllTopics();


    }
}
