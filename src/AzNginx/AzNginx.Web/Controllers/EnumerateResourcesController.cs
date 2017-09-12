using AzNginx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AzNginx.Common;
using AzNginx.Provision.Core;
using AzNginx.Web.Filters;

namespace AzNginx.Web.Controllers
{
    [ApiVersion(ApiVersions.April2014Preview, ApiVersions.April2014Alpha, ApiVersions.April2014)]
    public class EnumerateResourcesController : BaseApiController
    {
        public DeploymentStore Store { get; set; }
        public NginxResponseBuilder ResponseBuilder { get; set; }

        /// <summary>
        /// Handles requests to list all resources in a subscription.
        /// </summary>
        [AcceptVerbs("GET")]
        public virtual async Task<NginxResourceListResponse> GetAllResources([FromUri] ResourceSpec spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException("invalid request");
            }
            if (string.IsNullOrWhiteSpace(spec.subscriptionId))
            {
                throw new ArgumentNullException("SubscriptionId is null");
            }

            var nginxResources = await Store.GetAllNginxResources(spec);
            return ResponseBuilder.MakeListResponse(nginxResources, ApiVersion);
        }
    }
}