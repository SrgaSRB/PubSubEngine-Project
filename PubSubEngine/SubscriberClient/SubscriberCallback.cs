using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace SubscriberClient
{
    //Communication with the PubSubEngine service
    internal class SubscriberCallback : ISubscriberCallback
    {
        private static readonly List<string> msgLog = new List<string>();

        public void ReceiveAlarm(string topic, Alarm alarm)
        {
            string msg = $"Alarm received for topic '{topic}': {alarm}";

            string consoleMsg = "\n--------------------------------------------------Messages--------------------------------------------------\n";
            foreach (string m in msgLog)
            {
                consoleMsg += "|\t\t" + m + "\n";
            }
            consoleMsg += "|[Latest]\t" + msg + "\n";
            consoleMsg += "------------------------------------------------------------------------------------------------------------";

            Console.WriteLine(consoleMsg);

            msgLog.Add(msg);
            Program.PrintOptions();
        }

        public void NewPublisher(string topic)
        {
            Console.WriteLine($"New publisher registered for topic '{topic}'.");
            Program.PrintOptions();
        }

        public void LogOutPublisher(string topic)
        {
            Console.WriteLine($"Publisher for topic [{topic}] logout.");
            Program.PrintOptions();
        }
    }
}
