using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using AzNginx.Common;

namespace AzNginx.Web.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        protected string ApiVersion { get; set; }

        public string BaseAddress
        {
            get
            {
                var builder = new UriBuilder(ControllerContext.Request.RequestUri.AbsoluteUri)
                {
                    Path = ControllerContext.RequestContext.Url.Route("BaseAddress", new object()),
                    Query = null,
                };
                return builder.Uri.AbsoluteUri;
            }
        }

        public override async Task<HttpResponseMessage> ExecuteAsync(
            HttpControllerContext controllerContext,
            CancellationToken cancellationToken)
        {
            string value = GetQueryParameter(controllerContext.Request, "api-version");
            if (value != null)
            {
                ApiVersion = value.ToLowerInvariant();
            }

            HttpResponseMessage responseResult = await base.ExecuteAsync(controllerContext, cancellationToken);

            // Add x-ms-request-id header to response
            responseResult.Headers.Add("x-ms-request-id",
                controllerContext.Request.GetRequestId());

            // Add Date header to response (note Date should be serialized using RFC 1123 format)
            responseResult.Headers.Date = DateTimeOffset.UtcNow;

            return responseResult;
        }

        protected string GetQueryParameter(HttpRequestMessage request, string parameterName)
        {
            foreach (var pair in request.GetQueryNameValuePairs())
            {
                if (parameterName.Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return pair.Value;
                }
            }

            return null;
        }
    }
}