using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Common.Validator
{
    public class ThumbprintBasedCertificateValidator : ICertificateValidator
    {
        readonly string[] trustedCerts = null;

        public ThumbprintBasedCertificateValidator(params string[] trustedCerts)
        {
            this.trustedCerts = trustedCerts;
        }

        public void Validate(X509Certificate2 certificate, Action<string> logAction)
        {
            if (certificate == null)
            {
                throw new ArgumentException("ClientCertificateNotFound", "certificate");
            }

            if (trustedCerts == null || !trustedCerts.Any())
            {
                throw new InvalidOperationException("EmptyListNotSupported");
            }
            string thumbprint = certificate.Thumbprint;

            // Check that the certificate thumbprint matches one of the trusted thumbprints.
            if (string.IsNullOrEmpty(thumbprint) || !trustedCerts.Contains(thumbprint, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "ThumbprintNotValid", thumbprint));
            }

            DateTime certStartDate = certificate.NotBefore;
            DateTime certEndDate = certificate.NotAfter;

            if (DateTime.Now < certStartDate || DateTime.Now > certEndDate)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "InvalidCertificate", certStartDate, certEndDate));
            }
        }
    }
}
