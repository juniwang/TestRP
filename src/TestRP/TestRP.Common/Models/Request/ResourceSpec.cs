using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TestRP.Common.Exceptions;

namespace TestRP.Common.Models
{
    public class ResourceSpec
    {
        public ResourceSpec()
        {
        }

        public ResourceSpec(string subId, string rgName, string rType, string rName) : this(null, subId, rgName, rType, rName)
        {
        }

        public ResourceSpec(string locName, string subId, string rgName, string rType, string rName)
        {
            resourceProvider = "Microsoft.Nginx";
            locationName = locName;
            subscriptionId = subId;
            resourceGroupName = rgName;
            resourceType = rType;
            resourceName = rName;
        }

        public string subscriptionId { get; set; }
        public string resourceGroupName { get; set; }
        public string resourceProvider { get; set; }
        public string resourceType { get; set; }
        public string resourceName { get; set; }
        public string childResourceType { get; set; }
        public string childResourceName { get; set; }

        private string _locationName { get; set; }

        public string locationName
        {
            get
            {
                return this._locationName;
            }

            set
            {
                // TODO: Look for better way
                this._locationName = value;
                if (!string.IsNullOrEmpty(value) && value.Contains("+"))
                {
                    this._locationName = HttpUtility.UrlDecode(value);
                }
            }
        }

        public void Verify()
        {
            if (string.IsNullOrEmpty(subscriptionId))
            {
                throw new NonExactResourceException("subscriptionId");
            }
            else if (string.IsNullOrEmpty(resourceGroupName))
            {
                throw new NonExactResourceException("resourceGroupName");
            }
            else if (!"Microsoft.Nginx".Equals(resourceProvider, StringComparison.OrdinalIgnoreCase))
            {
                throw new NonExactResourceException("resourceProvider");
            }
        }
    }
}
