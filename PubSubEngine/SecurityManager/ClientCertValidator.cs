using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class ClientCertValidator : X509CertificateValidator
    {
        /// <summary>
        /// Implementation of a custom certificate validation on the client side.
        /// Client should consider certificate valid if the given certifiate is not self-signed.
        /// If validation fails, throw an exception with an adequate message.
        /// </summary>
        /// <param name="certificate"> certificate to be validate </param>
        public override void Validate(X509Certificate2 certificate)
        {
            if (DateTime.Now < certificate.NotBefore || DateTime.Now > certificate.NotAfter)
                throw new Exception("Certificate is expired or not yet valid.");

            // proveri da li je izdat od naše CA (možeš da uporediš CN Issuera sa CA)
            if (!certificate.Issuer.Contains("CN=PubSubEngineCA"))
                throw new Exception("Certificate issuer is not our trusted CA.");

            if (!certificate.Subject.Contains("CN=pubsubengine"))
                throw new Exception("Invalid certificate subject.");
        }
    }
}
