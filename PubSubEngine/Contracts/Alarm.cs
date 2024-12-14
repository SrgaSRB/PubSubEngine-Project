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

        public Alarm(DateTime createdAt, string message, int riskLevel)
        {
            CreatedAt = createdAt;
            Message = message;
            RiskLevel = riskLevel;
        }

        public override string ToString()
        {
            return $"[{CreatedAt}] {Message} (Risk: {RiskLevel})";
        }
    }
}
