using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Web.Models
{
    public class UpdateSubscriptionRequest
    {
        // The new State of the subscription
        [JsonProperty("state")]
        public SubscriptionState? State { get; set; }

        // The day of the month on which the subscription was originally created; maps to the billing date for this subscription and can be used for emitting usage data.
        [JsonProperty("registrationDate")]
        public string RegistrationDate { get; set; }

        // Supposedly:
        // "Property bag containing name/value pairs that can be used for telemetry and logging. 
        // Some examples include: EMail, OptIn"
        // This doesn't seem important to process
        [JsonProperty("properties")]
        public Dictionary<string, object> Properties { get; set; }
    }
}
