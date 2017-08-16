using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TestRP.Common;

namespace TestRP.Web.Handlers
{
    public class LogHttpHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
              HttpRequestMessage request,
              CancellationToken token)
        {
            var response = await base.SendAsync(request, token);

            AzureLog.Info(request.ToString());

            return response;
        }
    }
}
