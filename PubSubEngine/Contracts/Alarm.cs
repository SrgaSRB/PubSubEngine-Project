using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

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
        public int RiskLevel { get; set; }

        public Alarm(DateTime createdAt, string topic, int riskLevel)
        {
            CreatedAt = createdAt;
            Message = GetMessageFromTopic(topic);
            RiskLevel = riskLevel;
        }

        private string GetMessageFromTopic(string topic)
        {
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
            return $"[{CreatedAt}] {Message} (Risk: {RiskLevel})";
        }

    }
}
