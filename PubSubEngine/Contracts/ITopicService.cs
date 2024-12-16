using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    //For now, this interface does nothing,
    //the written function has been moved to isubscribeservice,
    //delete it when the program ends if nothing is implemented
    public interface ITopicService
    {
        [OperationContract]
        List<string> getAllTopics();

    }
}
