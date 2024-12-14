using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public delegate void MessageReceivedHandler(string topic, string message);

    public interface IEventNotification
    {
        event MessageReceivedHandler OnMessageReceived;
    }
}
