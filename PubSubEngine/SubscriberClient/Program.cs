using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Contracts;
using PubSubEngine;
using System.Runtime.Remoting.Contexts;
using SecurityManager;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Security.Principal;


namespace SubscriberClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            ISubscriberService proxy;

            try
            {
                var callback = new SubscriberCallback();
                var context = new InstanceContext(callback);

                // Binding sa sigurnosnim podešavanjima
                var binding = new NetTcpBinding(SecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                // Endpoint adresa servera
                var address = new EndpointAddress(new Uri("net.tcp://localhost:8081/Subscriber"),
                                                  new X509CertificateEndpointIdentity(CertificateManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, "pubsubengine")));

                // Kreiranje fabrike kanala sa Duplex komunikacijom
                var factory = new DuplexChannelFactory<ISubscriberService>(context, binding, address);

                // Podešavanje sertifikata za klijenta
                string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
                Console.WriteLine("Welcome {0}", cltCertCN);

                factory.Credentials.ClientCertificate.Certificate = CertificateManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
                factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
                factory.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
                factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

                // Kreiranje proxy-a
                proxy = factory.CreateChannel();

            }catch (Exception e)
            {
                Console.WriteLine("Error creating channel to server.");
                Console.WriteLine(e.Message);
                return;
            }

            try
            {
                Console.WriteLine("Connected to server. Listening for notifications...");

                while (true)
                {

                    PrintOptions();
                    string input = Console.ReadLine();

                    switch (input)
                    {
                        case "1":
                            Console.Write("Enter topic name to subscribe: ");
                            string topicToSubscribe = Console.ReadLine();
                            SubscribeToTopic(topicToSubscribe, proxy);
                            break;

                        case "2":
                            Console.Write("Enter topic name to unsubscribe: ");
                            string topicToUnsubscribe = Console.ReadLine();
                            proxy.Unsubscribe(AES_Symm_Algorithm.EncryptData(topicToUnsubscribe));
                            Console.WriteLine($"Unsubscribed from topic '{topicToUnsubscribe}'.");
                            break;

                        case "3":
                            PrintAllTopics(proxy.getAllTopics());
                            break;

                        case "4":
                            Console.WriteLine("Exiting program...");
                            return;

                        default:
                            Console.WriteLine("Invalid command. Try again.");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error communicating with the server.");
                Console.WriteLine(e.Message);
            }
        }

        public static void PrintAllTopics(List<string> topics)
        {
            string printVal = "Topics [";

            foreach (string topic in topics)
            {
                printVal += AES_Symm_Algorithm.DecryptData<string>(topic) + ", ";
            }

            if (topics.Count > 0)
                printVal = printVal.Substring(0, printVal.Length - 2);
            printVal += "]";

            Console.WriteLine(printVal);
        }
        public static void SubscribeToTopic(string topicSub, ISubscriberService proxy)
        {
            Console.Write("Minimun risk [for topic {0}]: ", topicSub);
            string minRisk = Console.ReadLine();
            Console.Write("Maximum risk [for topic {0}]: ", topicSub);
            string maxRisk = Console.ReadLine();


            try
            {
                if (!(int.TryParse(minRisk, out int minRiskInt) && int.TryParse(maxRisk, out int maxRiskInt)))
                {
                    Console.WriteLine("ERROR: Minimum and Maximum risk values must be numbers in range 1-100 where is Minimum risk number lower then Maximum risk number!");
                }
                else if (minRiskInt > maxRiskInt)
                {
                    Console.WriteLine("Minimum risk number must be lower then Maximum risk number!");
                }
                else if (minRiskInt < 1 || maxRiskInt > 100)
                {
                    Console.WriteLine("Minimum risk number must be lower then Maximum risk number!");
                }
                else
                {
                    try
                    {
                        minRisk = AES_Symm_Algorithm.EncryptData(minRisk);
                        maxRisk = AES_Symm_Algorithm.EncryptData(maxRisk);
                        topicSub = AES_Symm_Algorithm.EncryptData(topicSub);

                        if (proxy.Subscribe(topicSub, minRisk, maxRisk))
                        {
                            Console.WriteLine("Success subscribe to [{0}]", AES_Symm_Algorithm.DecryptData<string>(topicSub));
                        }
                        else
                        {
                            Console.WriteLine("ERROR: failed to subscribe to {0}", topicSub);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error communicating with the server");
                        Console.WriteLine(e.Message);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void PrintOptions()
        {
            Console.WriteLine("\nEnter a command: ");
            Console.WriteLine("[1] Subscribe to a topic");
            Console.WriteLine("[2] Unsubscribe from a topic");
            Console.WriteLine("[3] View all topics");
            Console.WriteLine("[4] Exit");
            Console.Write("> ");
        }
    }
}
