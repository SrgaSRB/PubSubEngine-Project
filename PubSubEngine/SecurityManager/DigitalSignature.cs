using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    //Creation and verification of digital signatures (RSA)
    public class DigitalSignature
    {
        public static string Sign(string message, string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                var messageBytes = Encoding.UTF8.GetBytes(message);
                var signedBytes = rsa.SignData(messageBytes, CryptoConfig.MapNameToOID("SHA256"));
                return Convert.ToBase64String(signedBytes);
            }
        }

    }
}
