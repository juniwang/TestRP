using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using TestRP.Common;
using TestRP.Common.Validator;

namespace TestRP.Web.Handlers
{
    public class CertificateValidatorHandler : DelegatingHandler
    {
        ICertificateValidator validator;
        ICertificateRetriever retriever;
        public CertificateValidatorHandler(ICertificateValidator validator, ICertificateRetriever retriever)
        {
            this.validator = validator;
            this.retriever = retriever;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken token)
        {
            try
            {
                var certificate = retriever.GetClientCertificate(request);
                AzureLog.Debug("Headers:" + request.Headers.ToString());
                if (certificate == null && request.Headers.Contains("X-ARR-ClientCert"))
                {
                    string certHeader = request.Headers.GetValues("X-ARR-ClientCert").First();
                    AzureLog.Debug("The certHeader is : " + certHeader);

                    if (!string.IsNullOrWhiteSpace(certHeader))
                    {
                        byte[] clientCertBytes = Convert.FromBase64String(certHeader);
                        certificate = new X509Certificate2(clientCertBytes);
                    }
                }

                validator.Validate(certificate, s =>
                {
                    AzureLog.Info("Client certificate validation failure details: {0}", s);
                });
            }
            catch (Exception e)
            {
                AzureLog.Error("Client certificate validation failed", e);
                return CertificateValidationFailedResponse(e.Message);
            }
            return await base.SendAsync(request, token);
        }

        internal static HttpResponseMessage CertificateValidationFailedResponse(string message)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.Content = new StringContent(message);
            return response;
        }
    }
}
