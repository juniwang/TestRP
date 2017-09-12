using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AzNginx.Web.Handlers
{
    public class RequiredHeadersHandler : DelegatingHandler
    {
        // This header will not change so 1Year is fine
        public static readonly KeyValuePair<string, string> HstsHeader = new KeyValuePair<string, string>("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

        // Required per http://vstfrd:8080/Azure/RD/_workitems#_a=edit&id=8948532&triage=true
        public static readonly KeyValuePair<string, string> ContentTypeOptions = new KeyValuePair<string, string>("x-content-type-options", "nosniff");

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken token)
        {
            var response = await base.SendAsync(request, token);

            AddHeaderIfNotPresent(response, HstsHeader);
            AddHeaderIfNotPresent(response, ContentTypeOptions);

            return response;
        }

        void AddHeaderIfNotPresent(HttpResponseMessage response, KeyValuePair<string, string> header)
        {
            if (!response.Headers.Contains(header.Key))
                response.Headers.Add(header.Key, header.Value);
        }

    }
}
