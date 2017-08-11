using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TestRP.Common;
using TestRP.Common.Models;
using TestRP.Web.Filters;

namespace TestRP.Web.Controllers
{
    [ApiVersion(ApiVersions.April2014Preview, ApiVersions.April2014Alpha, ApiVersions.April2014)]
    public class EnumerateResourcesController : BaseApiController
    {
        /// <summary>
        /// Handles requests to list all resources in a subscription.
        /// </summary>
        [AcceptVerbs("GET")]
        public virtual async Task<NginxResourceListResponse> GetAllResources(
            [FromUri] string locationName,
            [FromUri] string subscriptionId,
            [FromUri] string resourceType,
            [FromUri] bool whereHasNotifications = false)
        {
            if (string.IsNullOrEmpty(subscriptionId))
            {
                throw new ArgumentException("subscriptionId");
            }

            if (string.IsNullOrWhiteSpace(locationName))
            {
                throw new ArgumentException("locationName");
            }

            var dummy1 = new NginxResourceResponse { id = "dummy1", location = locationName, name = "dummy1", type = resourceType };
            var dummy2 = new NginxResourceResponse { id = "dummy1", location = locationName, name = "dummy1", type = resourceType };
            var dummy3 = new NginxResourceResponse { id = "dummy1", location = locationName, name = "dummy1", type = resourceType };
            return new NginxResourceListResponse
            {
                values = new NginxResourceResponse[] { dummy1, dummy2, dummy3 }
            };
        }
    }
}