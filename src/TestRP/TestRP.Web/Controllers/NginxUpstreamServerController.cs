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
    [RoutePrefix("subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/{resourceProvider}/UpstreamServers/{resourceName}/Servers")]
    public class NginxUpstreamServerController : BaseApiController
    {
        [AcceptVerbs("GET")]
        [Route]
        public virtual async Task<IHttpActionResult> Get(
           [FromUri]ResourceSpec spec)
        {
            spec.Verify();

            var data = NData.TryLoad();
            NResource resource = data.SearchBySpec(spec);
            if (resource == null)
            {
                return Ok(new NginxUpstreamServerListResponse());
            }
            else
            {
                return Ok(new NginxUpstreamServerListResponse
                {
                    Servers = resource.UpstreamServers.Select(p => p.ToResponse()).ToArray()
                });
            }
        }
    }
}
