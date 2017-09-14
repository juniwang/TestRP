using AzNginx.DAL;
using AzNginx.Models;
using AzNginx.Provision.Core.NginxProvision;
using AzNginx.Provision.Core.Entity;
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

        public NginxJobScheduler Scheduler { get; set; }

        public async Task<NginxResource> CreateResource(ResourceSpec spec, CreateNginxResourceRequest req, OperationId operationId)
        {
            var resource = new NginxResource
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

            var provisionEntity = new NginxProvisionEntity
            {
                EntityType = EntityType.ProvisionNginx,
                JobState = NginxProvisionState.ACRInit,
                RowKey = Entities.GetNewRowKey().Key,
                PartitionKey = spec.subscriptionId,
                RetryCount = 0,
                ScheduleTime = DateTime.UtcNow,
                OperationId = operationId,
            };
            await Scheduler.StartProvision(provisionEntity);

            return resource;
        }

        public async Task<NginxResource> GetResource(ResourceSpec spec, DateTimeOffset? ifModifiedSince = null)
        {
            var nginx = await Store.GetNginxResource(spec);
            if (ifModifiedSince.HasValue)
            {
                // return latest info which might be privisioning.
            }

            return nginx;
        }
    }
}
