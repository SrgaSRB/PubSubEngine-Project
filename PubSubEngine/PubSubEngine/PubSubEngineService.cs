using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using SecurityManager;

namespace PubSubEngine
{
    //Implement IPublisherService and ISubscriberService
    //Authenticates and validates clients
    //Distributes messages to Subscribers based on their subscriptions
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PubSubEngineService : IPublisherService, ISubscriberService, ITopicService
    {
        private static readonly Dictionary<string, List<(ISubscriberCallback callback, int minRisk, int maxRisk)>> topicSubscribers = new();
        private static readonly List<string> topics = new();
        private static readonly Dictionary<string, List<Alarm>> alarms = new();

        public bool Publish(string topicEncrypt, Alarm alarmEncript)
        {
            try
            {
                string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                string clientSignName = clientName + "_sign";

                Console.WriteLine("Publish -> From client: {0}", clientName);

                X509Certificate2 certificate = CertificateManager.GetCertificateFromStorage(
                    StoreName.TrustedPeople, StoreLocation.LocalMachine, clientSignName);

                bool isValid = false;
                try
                {
                    isValid = DigitalSignature.Verify(topicEncrypt, HashAlgorithm.SHA1, alarmEncript.Signature, certificate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DigitalSignature.Verify failed: " + ex.Message);
                    return false;
                }

                if (!isValid)
                {
                    Console.WriteLine("Publish -> Invalid digital signature from client: " + clientName);
                    return false;
                }

                Console.WriteLine("Publish -> Signature is valid");

                if (!alarms.ContainsKey(topicEncrypt))
                {
                    alarms[topicEncrypt] = new List<Alarm>();
                }

                alarms[topicEncrypt].Add(alarmEncript);

                NotifySubscribers(topicEncrypt, alarmEncript);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Publish error");
                Console.WriteLine(e.Message);
                return false;
            }
        }


        public bool Subscribe(string topicEncrypt, string minRiskEncrypt, string maxRiskEncrypt)
        {
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<ISubscriberCallback>();

                int minRisk = int.Parse(AES_Symm_Algorithm.DecryptData<string>(minRiskEncrypt));
                int maxRisk = int.Parse(AES_Symm_Algorithm.DecryptData<string>(maxRiskEncrypt));

                if (!topicSubscribers.ContainsKey(topicEncrypt))
                {
                    topicSubscribers[topicEncrypt] = new List<(ISubscriberCallback, int, int)>();
                }

                if (!topicSubscribers[topicEncrypt].Any(sub => sub.callback == callback))
                {
                    topicSubscribers[topicEncrypt].Add((callback, minRisk, maxRisk));
                }

                Console.WriteLine($"Subscriber subscribed to topic '{AES_Symm_Algorithm.DecryptData<string>(topicEncrypt)}' with risk range {minRisk}-{maxRisk}.");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Subscriber failed to subscribe to topic {0}", AES_Symm_Algorithm.DecryptData<string>(topicEncrypt));
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public void Unsubscribe(string topicEncrypt)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ISubscriberCallback>();

            if (topicSubscribers.ContainsKey(topicEncrypt))
            {
                var subscriberToRemove = topicSubscribers[topicEncrypt]
                    .FirstOrDefault(subscriber => subscriber.callback == callback);

                if (subscriberToRemove != default)
                {
                    topicSubscribers[topicEncrypt].Remove(subscriberToRemove);
                    Console.WriteLine($"Subscriber unsubscribed from topic '{AES_Symm_Algorithm.DecryptData<string>(topicEncrypt)}'.");
                }
                else
                {
                    Console.WriteLine($"Subscriber not found for topic '{AES_Symm_Algorithm.DecryptData<string>(topicEncrypt)}'.");
                }
            }
        }


        public void RegisterPublisher(string topicEncrypt, byte[] sign)
        {
            string clienName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            string clientNameSign = clienName + "_sign";
            Console.WriteLine("RegisterPublisher -> Client Name: {0}", clienName);
            X509Certificate2 certificate = CertificateManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientNameSign);
            try
            {

                if (DigitalSignature.Verify(topicEncrypt, HashAlgorithm.SHA1, sign, certificate))
                {
                    Console.WriteLine("RegisterPublisher -> Sign is valid");

                    if (!topics.Contains(topicEncrypt))
                    {
                        topics.Add(topicEncrypt);
                        NotifySubscribersOfNewPublisher(topicEncrypt, true);
                    }
                }
                else
                {
                    Console.WriteLine("RegisterPublisher -> Sign is invalid");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RegisterPublisher: {ex.Message}");
                throw new FaultException("Internal server error occurred during registration.");
            }
        }

        private void NotifySubscribers(string topicEncrypt, Alarm alarmEncript)
        {
            if (topicSubscribers.ContainsKey(topicEncrypt))
            {
                foreach (var (callback, minRisk, maxRisk) in topicSubscribers[topicEncrypt])
                {
                    try
                    {
                        int alarmRisk = int.Parse(AES_Symm_Algorithm.DecryptData<string>(alarmEncript.RiskLevel));
                        if (alarmRisk >= minRisk && alarmRisk <= maxRisk)
                        {
                            callback.ReceiveAlarm(topicEncrypt, alarmEncript);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error notifying subscriber for topic '{AES_Symm_Algorithm.DecryptData<string>(topicEncrypt)}'.");
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private void NotifySubscribersOfNewPublisher(string topicEncrypt, bool notifyType)
        {
            foreach (var subscriberList in topicSubscribers.Values)
            {
                foreach (var (callback, _, _) in subscriberList)
                {
                    try
                    {
                        if (notifyType)
                        {
                            callback.NewPublisher(topicEncrypt);
                        }
                        else
                        {
                            callback.LogOutPublisher(topicEncrypt);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error notifying subscriber about new publisher for topic '{AES_Symm_Algorithm.DecryptData<string>(topicEncrypt)}'.");
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }


        public List<string> getAllTopics()
        {
            return topics;
        }

        public bool LogOutPublisher(string topicEncrypt, byte[] sign)
        {
            string clienName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            string clientNameSign = clienName + "_sign";
            X509Certificate2 certificate = CertificateManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientNameSign);

            if (!DigitalSignature.Verify(topicEncrypt, HashAlgorithm.SHA1, sign, certificate))
                return false;

            try
            {
                if (topics.Contains(topicEncrypt))
                {
                    topics.Remove(topicEncrypt);
                    NotifySubscribersOfNewPublisher(topicEncrypt, false);
                    Console.WriteLine("The publisher who was registered on the topic[{0}] has successfully logged out.", AES_Symm_Algorithm.DecryptData<string>(topicEncrypt));
                    return true;
                }
                Console.WriteLine("ERROR: Failed to logout publisher from topic [{0}], topic was not found!", AES_Symm_Algorithm.DecryptData<string>(topicEncrypt));
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Failed to logout publisher from topic [{0}]", AES_Symm_Algorithm.DecryptData<string>(topicEncrypt));
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
