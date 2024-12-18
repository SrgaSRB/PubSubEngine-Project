using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using SecurityManager;

namespace SubscriberClient
{
    //Communication with the PubSubEngine service
    internal class SubscriberCallback : ISubscriberCallback
    {
        private static readonly List<string> msgLog = new List<string>();

        public void ReceiveAlarm(string topicEncrypt, Alarm alarmEncrypt)
        {
            Alarm alarm = new Alarm(alarmEncrypt.CreatedAt, alarmEncrypt.Topic, alarmEncrypt.RiskLevel);

            string msg = $"Alarm received for topic '{AES_Symm_Algorithm.DecryptData<string>(topicEncrypt)}': {alarm}";

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

        public void NewPublisher(string topicEncrypt)
        {
            Console.WriteLine($"New publisher registered for topic '{AES_Symm_Algorithm.DecryptData<string>(topicEncrypt)}'.");
            Program.PrintOptions();
        }

        public void LogOutPublisher(string topicEncrypt)
        {
            Console.WriteLine($"Publisher for topic [{AES_Symm_Algorithm.DecryptData<string>(topicEncrypt)}] logout.");
            Program.PrintOptions();
        }
    }
}
