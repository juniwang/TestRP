using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TestRP.Models;

namespace TestRP.Controllers
{
    public class SubscriptionsController : BaseController
    {
        // This API is called by CSM even though they don't document it.
        [AcceptVerbs("GET")]
        public virtual HttpResponseMessage Get(string locationName, string subscriptionId)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // CSM does notify on proxy-only endpoints as well. CSM doesn't have any hook do exclude those calls.
        // This is a dummy no-op implementation to keep CSM happy. All the real work happen in the
        // per-location PUT calls just below. 
        [AcceptVerbs("PUT")]
        public virtual async Task<IHttpActionResult> Put(
            [FromUri] string subscriptionId,
            [FromBody] UpdateSubscriptionRequest updateRequest)
        {
            await Task.Yield();
            HttpContent responseBody = JsonHelpers.ConvertToHttpContent(updateRequest);
            return new HttpStatusResult(HttpStatusCode.OK, responseBody);
        }

        // FLOW:
        // ARM(Csm) Calls us with a PUT request on a subscription
        // It is either to 'enable' or 'disable' or 'delete' deployments.
        // If it is to disable(enable) and not all deployments of that sub are already disabled(enabled)
        // then we 
        // - enqueue provisioning operations to disable(enable), for deployments which aren't already disabling/disabled
        // - update those deployment states to disabling(enabling)
        // - return 202
        // CSM will continue to call us with the same disable(enable) sub request until we return 200 instead of 202
        // We return 200 only once all the deployments are properly disabled(enabled).
        // For delete requests, its quite similar.
        [AcceptVerbs("PUT")]
        public virtual async Task<IHttpActionResult> Put(
            [FromUri] string locationName,
            [FromUri] string subscriptionId,
            [FromBody] UpdateSubscriptionRequest updateRequest)
        {
            if (String.IsNullOrEmpty(locationName))
            {
                throw new ArgumentException("locationName");
            }

            if (String.IsNullOrEmpty(subscriptionId))
            {
                throw new ArgumentException("subscriptionId");
            }
            if (updateRequest == null)
            {
                throw new ArgumentException("requestBody");
            }
            if (updateRequest.State == null)
            {
                throw new ArgumentException("state");
            }

            HttpContent responseBody = JsonHelpers.ConvertToHttpContent(updateRequest);

            //Logger.Info("UpdateRequest on Subscription {0}. State={1}", subscriptionId, updateRequest.State);
            bool enqueued = false;
            //switch (updateRequest.State)
            //{
            //    case SubscriptionState.Registered:
            //        enqueued = await Provisioner.Value.ReactivateSuspendedDeployments(subscriptionId, locationName);
            //        break;

            //    case SubscriptionState.Suspended:
            //    case SubscriptionState.Warned:
            //        enqueued = await Provisioner.Value.SuspendSubscription(subscriptionId, locationName);
            //        break;

            //    case SubscriptionState.Unregistered:
            //    case SubscriptionState.Deleted:
            //        enqueued = await Provisioner.Value.DeleteSubscription(subscriptionId, locationName);
            //        break;

            //    default:
            //        throw new HttpArgumentException("state");
            //}

            return enqueued ?
                new HttpStatusResult(HttpStatusCode.Accepted, null) :
                new HttpStatusResult(HttpStatusCode.OK, responseBody);
        }
    }
}
