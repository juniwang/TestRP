using AzNginx.Common;
using AzNginx.Models;
using AzNginx.Provision.Core.NginxProvision;
using AzNginx.Storage.Entities;
using AzNginx.Storage.Entities.Provision;
using AzNginx.Storage.Entities.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core
{
    /// <summary>
    /// A class as a main entrance to start any provision job.
    /// </summary>
    public class Provisioner
    {
        public NginxResourcesStore Store { get; set; }

        public NginxJobScheduler Scheduler { get; set; }

        public async Task<NginxResourceEntity> CreateResource(ResourceSpec spec, CreateNginxResourceRequest req, OperationId operationId)
        {
            var resource = new NginxResourceEntity
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
                PartitionKey = spec.subscriptionId,
                RowKey = Entities.GetNewRowKey().Key
            };

            var provisionEntity = new NginxProvisionEntity
            {
                EntityType = EntityType.ProvisionNginx,
                JobState = NginxProvisionState.ACRInit,
                RowKey = Entities.GetNewRowKey().Key,
                ResourceEntityRowKey = resource.RowKey,
                ResourceGroup = spec.resourceGroupName,
                UserSubscription = spec.subscriptionId,
                PartitionKey = spec.subscriptionId,
                RetryCount = 0,
                ScheduleTime = DateTime.UtcNow,
                OperationId = operationId,
            };
            await Scheduler.StartProvision(provisionEntity);

            return resource;
        }

        public async Task<NginxResourceEntity> GetResource(ResourceSpec spec, DateTimeOffset? ifModifiedSince = null)
        {
            var nginx = Store.GetNginxResource(spec);
            if (ifModifiedSince.HasValue)
            {
                // return latest info which might be privisioning.
            }

            return nginx;
        }
    }
}
