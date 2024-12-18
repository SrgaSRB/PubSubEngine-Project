using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
using SecurityManager;

namespace Contracts
{
    //Definition of alarm models (timestamp, risk, message)
    [DataContract]
    public class Alarm
    {
        [DataMember]
        public DateTime CreatedAt { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string RiskLevel { get; set; }
        [DataMember]
        public string Topic { get; set; }


        public Alarm(DateTime createdAt, string topicEncrypt, string riskLevelEncrypt)
        {
            CreatedAt = createdAt;
            Message = GetMessageFromTopic(topicEncrypt);
            RiskLevel = riskLevelEncrypt;
            Topic = topicEncrypt;
        }

        private string GetMessageFromTopic(string topicEncrypt)
        {
            string topic = AES_Symm_Algorithm.DecryptData<string>(topicEncrypt);
            return topic switch
            {
                "Fire" => AlarmMessages.GetMessage("AlarmFire"),
                "Flood" => AlarmMessages.GetMessage("AlarmFlood"),
                "Intruder" => AlarmMessages.GetMessage("AlarmIntruder"),
                _ => "Unknown alarm", // Default message
            };
        }
        public override string ToString()
        {
            return $"[{CreatedAt}] {Message} (Risk: {AES_Symm_Algorithm.DecryptData<string>(RiskLevel)})";
        }

    }
}
