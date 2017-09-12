using AzNginx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TestRP.Common;

namespace TestRP.Web.Controllers
{
    public class CheckNameAvailabilityController : BaseApiController
    {
        [HttpPost]
        public CheckNameAvailabilityResponse CheckNameAvailability(
            [FromUri]string subscriptionId,
            [FromBody]NameAvailabilityRequest request)
        {
            if (string.IsNullOrEmpty(subscriptionId))
            {
                throw new ArgumentException("subscriptionId");
            }

            if (!"Microsoft.Nginx/Nginx".Equals(request.type, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentOutOfRangeException("Can only handle Nginx resources.");
            }

            // basic validation in this RP. It should be globally unique since it's part of the dns name.
            // Should call `/providers/Microsoft.Resources/checkresourcename` to in formal implementation.

            if (!ResourceNamePolicy.IsResourceNameValid(request.name))
            {
                return CheckNameAvailabilityResponse.Invalid($"Service name {request.name} is invalid.");
            }
            var data = NData.TryLoad();
            if (data.Resources.Count(p => p.Name.Equals(request.name, StringComparison.OrdinalIgnoreCase)) > 0)
            {
                return CheckNameAvailabilityResponse.AlreadyExists($"Service name {request.name} already existed. Pleaes try another name.");
            }
            return CheckNameAvailabilityResponse.OK();
        }
    }
}