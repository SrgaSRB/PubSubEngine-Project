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

namespace PublisherClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var binding = new NetTcpBinding();
            var address = new EndpointAddress("net.tcp://localhost:8080/Publisher");

            var factory = new ChannelFactory<IPublisherService>(binding, address);
            var proxy = factory.CreateChannel();

            Console.WriteLine("Publisher is connected with server");

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
                            if ( proxy.Publish(AES_Symm_Algorithm.EncryptData(topic), new Alarm(DateTime.Now, AES_Symm_Algorithm.EncryptData(topic), AES_Symm_Algorithm.EncryptData(riskLevel))))
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
