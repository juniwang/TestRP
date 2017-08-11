using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestRP.Handlers
{
    class DoubleSlashSubscriptionsHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var originalUriString = request.RequestUri.OriginalString;
            int n = originalUriString.IndexOf("//subscriptions", StringComparison.OrdinalIgnoreCase);
            if (n >= 0)
            {
                request.RequestUri = new Uri(
                    originalUriString.Substring(0, n) + // first 'n' characters, before double slash
                    originalUriString.Substring(n + 1), // skip a slash, and take rest
                request.RequestUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
