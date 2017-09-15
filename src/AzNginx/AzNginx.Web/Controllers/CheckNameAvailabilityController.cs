using AzNginx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AzNginx.Common;
using AzNginx.Provision.Core;

namespace AzNginx.Web.Controllers
{
    public class CheckNameAvailabilityController : BaseApiController
    {
        public NginxResourcesStore Store { get; set; }

        [HttpPost]
        public CheckNameAvailabilityResponse CheckNameAvailability(
            [FromUri]string subscriptionId,
            [FromBody]NameAvailabilityRequest request)
        {
            if (string.IsNullOrEmpty(subscriptionId))
            {
                throw new ArgumentException("subscriptionId");
            }

            if (!NginxResourceTypes.NginxWithRP.Equals(request.type, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentOutOfRangeException("Can only handle Nginx resources.");
            }

            // basic validation in this RP. It should be globally unique since it's part of the dns name.
            // TODO Should call `/providers/Microsoft.Resources/checkresourcename` to in formal implementation.
            // and also it's better to call ARM api

            if (!ResourceNamePolicy.IsResourceNameValid(request.name))
            {
                return CheckNameAvailabilityResponse.Invalid($"Service name {request.name} is invalid.");
            }

            if (!Store.CheckNameAvailability(request.name))
            {
                return CheckNameAvailabilityResponse.AlreadyExists($"Service name {request.name} already existed. Pleaes try another name.");
            }
            return CheckNameAvailabilityResponse.OK();
        }
    }
}