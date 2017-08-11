using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;

namespace TestRP.Handlers
{
    /// <summary>
    /// Handler that understands X-HTTP-METHOD header as a way of overriding HTTP method
    /// for clients implementing workarounds [ARG 2.1: 6.2.3]
    /// </summary>
    public class XHttpMethodHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken token)
        {
            // note: Header names are case insensitive. We don't need code for it, 
            // that's baked into HttpRequestHeaders.Contains().
            if (request.Method == HttpMethod.Post && request.Headers.Contains("X-HTTP-METHOD"))
            {
                var value = request.Headers.GetValues("X-HTTP-METHOD").SingleOrDefault();
                if (String.Equals(value, "PUT", StringComparison.Ordinal))
                {
                    request.Method = HttpMethod.Put;
                    //request.Headers.Remove("X-HTTP-METHOD"); //?
                    return await base.SendAsync(request, token);
                }
                else if (String.Equals(value, "PATCH", StringComparison.Ordinal))
                {
                    request.Method = new HttpMethod("PATCH");
                    return await base.SendAsync(request, token);
                }
                else if (String.Equals(value, "DELETE", StringComparison.Ordinal))
                {
                    request.Method = HttpMethod.Delete;
                    return await base.SendAsync(request, token);
                }

                return request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return await base.SendAsync(request, token);
        }
    }
}
