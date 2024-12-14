using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public static class AlarmMessages
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager("Contracts.AlarmMessagesRES", typeof(AlarmMessages).Assembly);

        public static string GetMessage(string key)
        {
            return ResourceManager.GetString(key);
        }
    }
}
