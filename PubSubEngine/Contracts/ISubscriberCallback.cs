using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISubscriberCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveAlarm(string topic, Alarm alarm);

        [OperationContract(IsOneWay = true)]
        void NewPublisher(string topic);

        [OperationContract(IsOneWay = true)]

        void LogOutPublisher(string topic);
    }
}
