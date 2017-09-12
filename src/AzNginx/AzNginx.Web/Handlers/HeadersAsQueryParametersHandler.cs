using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzNginx.Web.Handlers
{
    /// <summary>
    /// Handler that understands headers as query parameter to support 
    /// clients (e.g. AJAX) implementing workarounds. [ARG 2.1: 6.4.4]
    /// </summary>
    public class HeadersAsQueryParametersHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // TODO: implement
            return base.SendAsync(request, cancellationToken);
        }
    }
}
