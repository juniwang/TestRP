using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using TestRP.Common;
using TestRP.Common.Helpers;
using TestRP.Common.Models;
using TestRP.Web.Filters;

namespace TestRP.Web.Controllers
{
    [ApiVersion(ApiVersions.April2014Preview, ApiVersions.April2014Alpha, ApiVersions.April2014)]
    [RoutePrefix("subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/{resourceProvider}/Nginx/{resourceName?}")]
    public class NginxResourceController : BaseApiController
    {
        [HttpPut]
        [Route]
        public virtual async Task<IHttpActionResult> Put(
            [FromUri]ResourceSpec spec,
            [FromBody]CreateNResourceRequest request)
        {
            if (request == null || spec == null)
                throw new ArgumentNullException("invalid request");
            spec.Verify();

            spec.resourceName = spec.resourceName ?? "Nginx-" + Guid.NewGuid().ToString().Substring(0, 0);
            var dep = new NDeployment
            {
                CreateTime = DateTime.Now,
                DeploymentId = Guid.NewGuid().ToString(),
                DeploymentLable = "Deployment" + DateTime.Now.ToString(),
                SubscriptionId = spec.subscriptionId
            };

            NData data = NData.TryLoad();
            var res = data.SearchBySpec(spec);
            if (res == null)
            {
                res = new NResource
                {
                    DeploymentId = dep.DeploymentId,
                    Enabled = true,
                    Location = request.location ?? "East US",
                    Name = spec.resourceName,
                    ResourceGroup = spec.resourceGroupName,
                    Tags = request.tags,
                    Type = "Microsoft.Nginx/Nginx",
                    SubscriptionId = spec.subscriptionId,
                    properties = new NResourceProperties
                    {
                        NginxVersion = request.properties.nginxVersion
                    },
                };
                res.CreateRandomServersForTesting();
                data.Resources.Add(res);
            }
            else
            {
                res.DeploymentId = dep.DeploymentId;
                res.Tags = request.tags;
                res.properties.NginxVersion = request.properties.nginxVersion;
            }

            data.Deployments.Add(dep);
            data.SaveChanges();

            return Ok(res.ToResponse());
        }

        [AcceptVerbs("GET")]
        [Route]
        public virtual async Task<IHttpActionResult> Get(
            [FromUri]ResourceSpec spec)
        {
            if (spec == null)
                throw new ArgumentNullException("invalid request");
            spec.Verify();

            var data = NData.TryLoad();
            if (string.IsNullOrEmpty(spec.resourceName))
            {
                return Ok(new NginxResourceListResponse
                {
                    values = data.Resources.Where(
                        p => p.SubscriptionId.Equals(spec.subscriptionId, StringComparison.OrdinalIgnoreCase)
                        && p.ResourceGroup.Equals(spec.resourceGroupName, StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.ToResponse())
                    .ToArray()
                });
            }

            var res = data.SearchBySpec(spec);
            if (res != null)
            {
                return Ok(res.ToResponse());
            }
            else
            {
                return Ok(new NginxResourceListResponse());
            }
        }

        [AcceptVerbs("GET")]
        [Route("servers")]
        public virtual async Task<IHttpActionResult> Servers(
            [FromUri]ResourceSpec spec)
        {
            if (spec == null)
                throw new ArgumentNullException("invalid request");
            spec.Verify();

            var data = NData.TryLoad();
            var res = data.SearchBySpec(spec);
            if (res != null)
            {
                return Ok(res.ToResponse());
            }
            else
            {
                return Ok(new NginxResourceListResponse());
            }
        }

        [AcceptVerbs("DELETE")]
        [Route]
        public virtual async Task<IHttpActionResult> Delete([FromUri]ResourceSpec spec)
        {
            if (spec == null)
                throw new ArgumentNullException("invalid request");
            spec.Verify();

            var data = NData.TryLoad();
            var res = data.SearchBySpec(spec);
            if (res == null)
                return new HttpStatusResult(HttpStatusCode.NoContent);
            else
            {
                data.Resources.Remove(res);
                data.SaveChanges();
            }

            return Ok();
        }

        [AcceptVerbs("OPTIONS")]
        [Route]
        public virtual IHttpActionResult Options([FromUri]ResourceSpec spec)
        {
            var result = new HttpStatusResult(HttpStatusCode.OK, JsonHelpers.ConvertToHttpContent(string.Empty));
            result.AddHeader("Allow", "GET, PUT, DELETE");
            // TODO: return "Link" header pointing at resource documentation
            // result.AddHeader("Link" "<help>; rel=\""help\""");
            return result;
        }
    }
}

