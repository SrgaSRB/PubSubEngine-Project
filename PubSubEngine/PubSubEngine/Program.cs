using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.ServiceModel.Security;

namespace PubSubEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
                Console.WriteLine("Welcome {0}", srvCertCN);

                NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                var publisherBaseAddress = new Uri("net.tcp://localhost:8080/Publisher");
                ServiceHost publisherHost = new ServiceHost(typeof(PubSubEngineService));
                publisherHost.AddServiceEndpoint(typeof(IPublisherService), binding, publisherBaseAddress);

                publisherHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
                publisherHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();
                publisherHost.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
                publisherHost.Credentials.ServiceCertificate.Certificate = CertificateManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

                publisherHost.Open();
                Console.WriteLine("Publisher service is running on net.tcp://localhost:8080/Publisher");


                NetTcpBinding subscriberBinding = new NetTcpBinding(SecurityMode.Transport);
                subscriberBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                var subscriberService = new PubSubEngineService();
                var subscriberBaseAddress = new Uri("net.tcp://localhost:8081/Subscriber");
                var subscriberHost = new ServiceHost(subscriberService, subscriberBaseAddress);
                subscriberHost.AddServiceEndpoint(typeof(ISubscriberService), subscriberBinding, "");

                subscriberHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
                subscriberHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();
                subscriberHost.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
                subscriberHost.Credentials.ServiceCertificate.Certificate = CertificateManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

                subscriberHost.Open();
                Console.WriteLine("Subscriber service is running on net.tcp://localhost:8081/Subscriber");



                Console.WriteLine("Press Enter to stop the services...");
                Console.ReadLine();

                publisherHost.Close();
                subscriberHost.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}