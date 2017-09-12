using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AzNginx.Web.Handlers
{
    /// <summary>
    /// Handler that understands the return-client-request-id 
    /// AND client-request-id headers, and automatically sets up the correct
    /// response headers.
    /// 
    /// x-ms-client-request-id is an accepted synonym of client-request-id.
    /// x-ms-return-client-request-id is an accepted synconym of return-client-request-id.
    /// See [ARG 2.1: 6.4, 6.4.1], [RPAv2: Client Request Headers, Response Headers]
    /// </summary>
    class XMsClientRequestIdHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken token)
        {
            // Copy x-ms-client-request-id values to the response if they exist and
            // the client has requested it.
            // note: ARG 2.1 and RPAv2 seem possibly inconsistent on whether they expect x-ms prefixes,
            // so being liberal in accepting headers.
            IEnumerable<string> clientRequestIds = null;
            if (request.Headers.Contains("x-ms-client-request-id"))
            {
                clientRequestIds = request.Headers.GetValues("x-ms-client-request-id");
            }
            else if (request.Headers.Contains("client-request-id"))
            {
                clientRequestIds = request.Headers.GetValues("client-request-id");
            }

            HttpResponseMessage response = await base.SendAsync(request, token);
            if (clientRequestIds != null)
            {
                if (request.Headers.Contains("return-client-request-id"))
                {
                    if (request.Headers.GetValues("return-client-request-id").Any(value => String.Equals(value, "True", StringComparison.OrdinalIgnoreCase)))
                    {
                        // note: ARG 2.1 seems to imply ms-x prefix will not be used
                        response.Headers.Add("client-request-id", clientRequestIds);
                    }
                }
                else if (request.Headers.Contains("x-ms-return-client-request-id"))
                {
                    if (request.Headers.GetValues("x-ms-return-client-request-id").Any(value => String.Equals(value, "True", StringComparison.OrdinalIgnoreCase)))
                    {
                        // note: ARG 2.1 seems to imply ms-x prefix will not be used even in this case
                        response.Headers.Add("client-request-id", clientRequestIds);
                    }
                }
            }

            response.Headers.Add("x-rp-server-mvid", GetType().Module.ModuleVersionId.ToString());

            return response;
        }
    }
}
