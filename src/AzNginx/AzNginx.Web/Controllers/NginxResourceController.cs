using AzNginx.Models;
using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;
using AzNginx.Common;
using AzNginx.Common.Exceptions;
using AzNginx.Common.Helpers;
using AzNginx.Provision.Core;
using AzNginx.Web.Filters;

namespace AzNginx.Web.Controllers
{
    [ApiVersion(ApiVersions.April2014Preview, ApiVersions.April2014Alpha, ApiVersions.April2014)]
    [RoutePrefix("subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/{resourceProvider}/{resourceType}/{resourceName?}")]
    public class NginxResourceController : BaseApiController
    {
        public DeploymentStore Store { get; set; }

        public Provisioner Provisioner { get; set; }

        public NginxResponseBuilder ResponseBuilder { get; set; }

        [HttpPut]
        [Route]
        public virtual async Task<IHttpActionResult> Put(
            [FromUri]ResourceSpec spec,
            [FromBody]CreateNginxResourceRequest body)
        {
            // validate requests
            if (body == null || spec == null)
            {
                throw new ArgumentNullException("invalid request");
            }
            spec.Verify();
            ResourceNamePolicy.ValidateResourceName(spec);
            spec.locationName = body.location;

            var nginx = await Store.TryGetDeployment(spec);
            if (nginx == null)
            {
                // create request
                nginx = await Provisioner.CreateResource(spec, body, OperationId);
                // todo enqueue message to deployment
                // saving to DB
                Store.Context.NginxResources.Add(nginx);
                Store.Context.SaveChanges();
                return Ok(ResponseBuilder.MakeResponse(nginx, ApiVersion));
            }
            else
            {
                // update request
                return Ok(ResponseBuilder.MakeResponse(nginx, ApiVersion));
            }
        }

        [AcceptVerbs("GET")]
        [Route]
        public virtual async Task<IHttpActionResult> Get(
            [FromUri]ResourceSpec spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException("invalid request");
            }
            spec.Verify();

            if (string.IsNullOrEmpty(spec.resourceName))
            {
                // Handles requests to list all resources in a resource group.
                var nginxResources = await Store.GetAllNginxResources(spec);
                return Ok(ResponseBuilder.MakeListResponse(nginxResources, ApiVersion));
            }
            else
            {
                var nginx = await Provisioner.GetResource(spec, Request.Headers.IfModifiedSince);
                return Ok(ResponseBuilder.MakeResponse(nginx, ApiVersion));
            }
        }

        [AcceptVerbs("GET")]
        [Route("servers")]
        public virtual async Task<IHttpActionResult> Servers(
            [FromUri]ResourceSpec spec)
        {
            // should return the Upstream servers. Might be replaced by config API?
            return await Get(spec);
        }

        [AcceptVerbs("DELETE")]
        [Route]
        public virtual async Task<IHttpActionResult> Delete([FromUri]ResourceSpec spec)
        {
            if (spec == null)
                throw new ArgumentNullException("invalid request");
            spec.Verify();

            try
            {
                var nginx = await Provisioner.GetResource(spec);
                // todo should remove acr too
                Store.Context.NginxResources.Remove(nginx);
            }
            catch (NginxResourceDoesntExistException)
            {
                return new HttpStatusResult(HttpStatusCode.NoContent);
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

