using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TestRP.Kernel;

namespace TestRP.Handlers
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
            var certificate = retriever.GetClientCertificate(request);

            try
            {
                validator.Validate(certificate, s =>
                {
                    //ResourceProviderEventSource.Instance.LogEvent(EventLevel.Warning, request.AsRequestData(), "Client certificate validation failure details: {0}",
                    //    s);
                });
            }
            catch (Exception e)
            {
                //ResourceProviderEventSource.Instance.LogEvent(EventLevel.Verbose, request.AsRequestData(), "Client certificate validation failed: {0}", e.Message);
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
