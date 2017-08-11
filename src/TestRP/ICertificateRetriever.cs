using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TestRP
{
    public interface ICertificateRetriever
    {
        X509Certificate2 GetClientCertificate(HttpRequestMessage request);
    }

    public class CertificateRetriever : ICertificateRetriever
    {
        public X509Certificate2 GetClientCertificate(HttpRequestMessage request)
        {
            return request.GetClientCertificate();
        }
    }

    public class MockCertificateRetriever : ICertificateRetriever
    {
        private X509Certificate2 cert;

        public MockCertificateRetriever(X509Certificate2 cert)
        {
            this.cert = cert;
        }

        public X509Certificate2 GetClientCertificate(HttpRequestMessage request)
        {
            return cert;
        }
    }
}
