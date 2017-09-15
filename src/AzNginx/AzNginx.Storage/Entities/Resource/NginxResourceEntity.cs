using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Storage.Entities.Resource
{
    public class NginxResourceEntity : NginxEntityBase
    {
        public NginxResourceEntity() { }
        public NginxResourceEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        /// <summary>
        /// Standardized Azure resource Id.
        /// </summary>
        [IgnoreProperty]
        public string ResourceId
        {
            get
            {
                return $"/subscriptions/{UserSubscriptionId}/resourceGroups/{ResourceGroup}/providers/Microsoft.Nginx/Nginx/{ResourceName}";
            }
        }

        public string ResourceName { get; set; }
        public string UserSubscriptionId { get; set; }
        public string ResourceType { get; set; }
        public string ResourceGroup { get; set; }
        public string Location { get; set; }

        public int Status { get; set; }
        public string NginxVersion { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
        [ConvertableEntityProperty]
        public Dictionary<string, string> Tags { get; set; }

    }
}
