using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRP.Common.Helpers;

namespace AzNginx.DAL
{
    public partial class NginxResource
    {
        /// <summary>
        /// Standardized Azure resource Id. By contrast, Id is an auto-increased integer in DB.
        /// </summary>
        public string ResourceId
        {
            get
            {
                return string.Format("/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Nginx/Nginx/{2}"
                    , UserSubscriptionId, ResourceGroup, ResourceName);
            }
        }

        public IDictionary<string, string> Tags
        {
            get
            {
                return JsonHelpers.DeserializeDict(this.TagsSerialized);
            }
            set
            {
                this.TagsSerialized = JsonHelpers.SerializeObject(value ?? new Dictionary<string, string>());
            }
        }
    }

    public static class NginxResourcesExtension
    {
        public static bool IsCreating(this NginxResource nginx)
        {
            return nginx.Status == NginxResourceStatus.Creating;
        }
    }
}
