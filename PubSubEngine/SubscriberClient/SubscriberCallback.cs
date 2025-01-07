using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using SecurityManager;
using Logging;

namespace SubscriberClient
{
    //Communication with the PubSubEngine service
    internal class SubscriberCallback : ISubscriberCallback
    {
        private static readonly List<string> msgLog = new List<string>();

        public void ReceiveAlarm(string topicEncrypt, Alarm alarmEncrypt)
        {

            string clienName = "publisher";
            string clientNameSign = clienName + "_sign";
            X509Certificate2 certificate = CertificateManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientNameSign);

            if (DigitalSignature.Verify(topicEncrypt, HashAlgorithm.SHA1, alarmEncrypt.Signature, certificate))
            {
                Console.WriteLine("\nValid alarm received.");

                string msg = $"Alarm received for topic '{AES_Symm_Algorithm.DecryptData<string>(topicEncrypt)}': {alarmEncrypt}";

                string consoleMsg = "\n--------------------------------------------------Messages--------------------------------------------------\n";
                foreach (string m in msgLog)
                {
                    consoleMsg += "|\t\t" + m + "\n";
                }
                consoleMsg += "|[Latest]\t" + msg + "\n";
                consoleMsg += "------------------------------------------------------------------------------------------------------------";

                Console.WriteLine(consoleMsg);
                msgLog.Add(msg);

                SaveAlarmToDatabase(msg, alarmEncrypt, certificate);

                Program.PrintOptions();
            }
            else
            {
                Console.WriteLine("Invalid alarm received. Ignoring...");
            }

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

        private void SaveAlarmToDatabase(string msg, Alarm alarm, X509Certificate2 certificate)
        {
            File.AppendAllText("alarms.txt", msg + Environment.NewLine);
            Console.WriteLine("\nAlarm saved to database.");

            // Da li treba alarm dekriptovati
            string timestamp = DateTime.Now.ToString();
            string databaseName = "alarms.txt";
            string entityId = alarm.Id.ToString(); 
            string digitalSignature = Convert.ToBase64String(alarm.Signature);
            string publicKey = Convert.ToBase64String(certificate.GetPublicKey());

            Audit.DataInserted(timestamp, databaseName, entityId, digitalSignature, publicKey);
        }

    }
}
