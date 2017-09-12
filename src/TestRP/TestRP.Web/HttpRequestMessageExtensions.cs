using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Web
{
    public static class HttpRequestMessageExtensions
    {
        public static string GetRequestId(this HttpRequestMessage request)
        {
            Guid requestId;

            if (request == null)
            {
                requestId = Guid.NewGuid();
            }
            else
            {
                object reqId = null;
                if (request.GetOwinContext().Environment.TryGetValue("x-ms-request-id", out reqId))
                {
                    requestId = (Guid)reqId;
                }
                else
                {
                    requestId = Guid.NewGuid();
                    request.GetOwinContext().Environment.Add("x-ms-request-id", requestId);
                }
            }

            return requestId.ToString();
        }

        public static string GetTenantId(this HttpRequestMessage request)
        {
            return request.GetHeaderValue("x-ms-client-tenant-id");
        }

        public static string GetClientRequestId(this HttpRequestMessage request)
        {
            return request.GetHeaderValue("x-ms-client-request-id");
        }

        public static string GetCsmCorrelationId(this HttpRequestMessage request)
        {
            return request.GetHeaderValue("x-ms-correlation-request-id");
        }

        public static string GetPrincipalId(this HttpRequestMessage request)
        {
            return request.GetHeaderValue("x-ms-client-principal-id");
        }

        public static string GetObjectId(this HttpRequestMessage request)
        {
            return request.GetHeaderValue("x-ms-client-object-id");
        }

        public static string GetHeaderValue(this HttpRequestMessage request, string headerName)
        {
            if (request == null)
            {
                if (headerName.Equals("Host"))
                {
                    return "localhost";
                }
                return null;
            }

            if (request.Headers != null &&
                request.Headers.Contains(headerName))
            {
                IEnumerable<string> values;
                if (request.Headers.TryGetValues(headerName, out values))
                {
                    return values.First();
                }
            }

            return null;
        }
    }
}
