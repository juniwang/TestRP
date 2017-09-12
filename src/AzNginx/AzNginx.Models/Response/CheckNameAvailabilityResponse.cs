using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Models
{
    public class CheckNameAvailabilityResponse
    {
        // make ctor private to prevent invalid reason
        private CheckNameAvailabilityResponse()
        {

        }

        public bool nameAvailable { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string reason { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string message { get; set; }

        public static CheckNameAvailabilityResponse OK()
        {
            return new CheckNameAvailabilityResponse
            {
                nameAvailable = true
            };
        }

        public static CheckNameAvailabilityResponse Invalid(string message)
        {
            return Failed("Invalid", message);
        }

        public static CheckNameAvailabilityResponse AlreadyExists(string message)
        {
            return Failed("AlreadyExists", message);
        }

        static CheckNameAvailabilityResponse Failed(string reason, string message)
        {
            /*
              reason should be "Invalid" or "AlreadyExists". Required if nameAvailable==false

              message is localized and required if nameAvailable==false. If reason == invalid, provide the user 
              with the reason why the given name is invalid, and provide the resource naming requirements so that 
              the user can select a valid name.  If reason == AlreadyExists, explain that <resourceName> is already 
              in use, and direct them to select a different name.
             */

            return new CheckNameAvailabilityResponse
            {
                nameAvailable = false,
                reason = reason,
                message = message
            };
        }
    }
}
