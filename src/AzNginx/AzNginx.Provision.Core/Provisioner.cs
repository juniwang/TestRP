using AzNginx.DAL;
using AzNginx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core
{
    public class Provisioner
    {
        public DeploymentStore Store { get; set; }

        public async Task<NginxResource> CreateResource(ResourceSpec spec, CreateNginxResourceRequest req)
        {
            // todo call acs and kubernetes APIs
            return new NginxResource
            {
                CreatedTime = DateTime.UtcNow,
                Location = spec.locationName,
                Tags = req.tags,
                NginxVersion = req.properties.nginxVersion,
                ResourceName = spec.resourceName,
                ResourceGroup = spec.resourceGroupName,
                Status = NginxResourceStatus.Creating,
                UserSubscriptionId = spec.subscriptionId,
                ResourceType = spec.resourceType,
            };
        }

        public async Task<NginxResource> GetResource(ResourceSpec spec, DateTimeOffset? ifModifiedSince = null)
        {
            var nginx = await Store.GetNginxResource(spec);
            if (ifModifiedSince.HasValue) {
                // return latest info which might be privisioning.
            }

            return nginx;
        }
    }
}
