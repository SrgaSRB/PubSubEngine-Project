using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class ServiceCertValidator : X509CertificateValidator
    {
        /// <summary>
        /// Implementation of a custom certificate validation on the service side.
        /// Service should consider certificate valid if its issuer is the same as the issuer of the service.
        /// If validation fails, throw an exception with an adequate message.
        /// </summary>
        /// <param name="certificate"> certificate to be validate </param>
        public override void Validate(X509Certificate2 certificate)
        {
            if (DateTime.Now < certificate.NotBefore || DateTime.Now > certificate.NotAfter)
                throw new Exception("Certificate is expired or not yet valid.");

            X509Certificate2 srvCert =
                CertificateManager.GetCertificateFromStorage(StoreName.My,
                                                             StoreLocation.LocalMachine,
                                                             Formatter.ParseName(WindowsIdentity.GetCurrent().Name));

            if (!certificate.Issuer.Equals(srvCert.Issuer))
                throw new Exception("Certificate issuer is not PubSubEngine.");

            if (!certificate.Subject.Contains("CN=publisher") &&
                !certificate.Subject.Contains("CN=subscriber"))
                throw new Exception("Unexpected certificate CN.");
        }
    }
}
