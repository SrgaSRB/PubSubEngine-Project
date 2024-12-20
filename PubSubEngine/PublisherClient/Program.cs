using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Contracts;
using PublisherClient;
using PubSubEngine;
using SecurityManager;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Security.Principal;

namespace PublisherClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPublisherService proxy;
            try
            {

                var binding = new NetTcpBinding(SecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                var address = new EndpointAddress(
                    new Uri("net.tcp://localhost:8080/Publisher"),
                    new X509CertificateEndpointIdentity(
                        CertificateManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, "pubsubengine")));

                var factory = new ChannelFactory<IPublisherService>(binding, address);

                string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
                Console.WriteLine("Welcome {0}", cltCertCN);

                factory.Credentials.ClientCertificate.Certificate = CertificateManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
                factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
                factory.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
                factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

                proxy = factory.CreateChannel();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error establishing connection:");
                Console.WriteLine(e.Message);
                Console.WriteLine("Program will now terminate.");
                Console.ReadLine();
                return; // Završava program ako veza ne uspe
            }

            //proxy.RegisterPublisher("Weather");
            //proxy.Publish("Weather", new Alarm(DateTime.Now, "It's sunny today!", 5));
            //proxy.Publish("Weather", new Alarm(DateTime.Now, "Storm is coming!", 80));

            Console.Write("Enter the topic you are posting about: ");
            string topic = Console.ReadLine();

            try
            {
                proxy.RegisterPublisher(AES_Symm_Algorithm.EncryptData(topic));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error communicating with the server");
                Console.WriteLine(e.Message);
            }

            while (true)
            {
                Console.WriteLine("1 -> Sending messages to subscriptions [MOD]");
                Console.WriteLine("0 -> Close publisher");
                Console.WriteLine("Option: ");
                string command = Console.ReadLine();

                if (command == "0")
                {
                    try
                    {
                        if (proxy.LogOutPublisher(AES_Symm_Algorithm.EncryptData(topic)))
                        {
                            Console.WriteLine("Publisher successfuly logout! Press Enter to close program...");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Publisher failed to logout from topic {0}", topic);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error communicating with the server");
                        Console.WriteLine(e.Message);
                    }
                }
                else if (command == "1")
                {
                    while (true)
                    {
                        MessagesMOD();
                        Console.WriteLine("Topic [{0}] ", topic);
                        //Console.Write("Message: ");
                        //string msg = Console.ReadLine();
                        Console.Write("Risk level [1-100]: ");
                        string riskLevel = Console.ReadLine();

                        if (riskLevel == "999")
                            break;
                        else if (!int.TryParse(riskLevel, out int intRiskLEvel))
                        {
                            Console.WriteLine("ERROR: Risk level must be number in range 1-100 ");
                            continue;
                        }
                        else if (intRiskLEvel < 1 || intRiskLEvel > 100)
                        {
                            Console.WriteLine("ERROR: Risk level out of range! Range[1-100] ");
                            continue;
                        }
                        try
                        {
                            if (proxy.Publish(AES_Symm_Algorithm.EncryptData(topic), new Alarm(DateTime.Now, AES_Symm_Algorithm.EncryptData(topic), AES_Symm_Algorithm.EncryptData(riskLevel))))
                            {
                                Console.WriteLine("Message successfuly sent to subscribers!");
                            }
                            else
                            {
                                Console.WriteLine("Error sending message to subscribers".ToUpper());
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error communicating with the server");
                            Console.WriteLine(e.Message);
                        }
                    }
                }

            }


            Console.ReadKey();

        }

        public static void MessagesMOD()
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------------------------------------------------");
            Console.WriteLine("\t\tMessagesMOD [to EXIT tMessagesMOD enter 999 for \"Risk level\"]");
            Console.WriteLine("----------------------------------------------------------------------------------------------------");
        }

    }
}
