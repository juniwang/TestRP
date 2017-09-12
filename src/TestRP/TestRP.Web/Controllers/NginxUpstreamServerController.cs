using AzNginx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TestRP.Common;
using TestRP.Provision.Core;
using TestRP.Web.Filters;

namespace TestRP.Web.Controllers
{
    [ApiVersion(ApiVersions.April2014Preview, ApiVersions.April2014Alpha, ApiVersions.April2014)]
    [RoutePrefix("subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/{resourceProvider}/UpstreamServers/{resourceName}/Servers")]
    public class NginxUpstreamServerController : BaseApiController
    {
        public Provisioner Provisioner { get; set; }

        [AcceptVerbs("GET")]
        [Route]
        public virtual async Task<IHttpActionResult> Get(
           [FromUri]ResourceSpec spec)
        {
            spec.Verify();

            try
            {
                var nginx = await Provisioner.GetResource(spec);
                return Ok(new NginxUpstreamServerListResponse());
            }
            catch
            {
                return Ok(new NginxUpstreamServerListResponse());
            }
        }
    }
}
