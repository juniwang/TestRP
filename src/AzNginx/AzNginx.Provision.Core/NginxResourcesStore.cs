using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzNginx.Models;
using AzNginx.Common.Exceptions;
using AzNginx.Storage.Resource;
using Microsoft.WindowsAzure.Storage;
using AzNginx.Common;
using AzNginx.Storage.Entities;
using AzNginx.Storage.Entities.Resource;

namespace AzNginx.Provision.Core
{
    /// <summary>
    /// a class that encapsulates complex access(es) to resource table. 
    /// Call NginxResourceTable instance methods directly for simple CRUD operations.
    /// </summary>
    public class NginxResourcesStore
    {
        public NginxResourceTable ResourceTable { get; set; }

        public NginxResourceEntity GetNginxResource(ResourceSpec spec)
        {
            spec.MustSpecifyExactResource();
            var nginx = TryGetNginxResource(spec);
            if (nginx == null)
            {
                throw new NginxResourceDoesntExistException();
            }

            return nginx;
        }

        public NginxResourceEntity TryGetNginxResource(ResourceSpec resource)
        {
            resource.MustSpecifyExactResource();

            // possibly filter by Region too?
            return ResourceTable.TryGetNginxResource(
                nr => (nr.PartitionKey == resource.subscriptionId
                    && nr.ResourceGroup == resource.resourceGroupName
                    && nr.ResourceName == resource.resourceName),
                excludeDeleted: true);

            //return ResourceTable.CreateQuery().Where(nr => (nr.PartitionKey == resource.subscriptionId
            //        && nr.ResourceGroup == resource.resourceGroupName
            //        && nr.ResourceName == resource.resourceName)
            //        && nr.Status != NginxResourceStatus.Deleted
            //        && nr.Status != NginxResourceStatus.Deleting
            //        && nr.Status != NginxResourceStatus.DeleteFailed).FirstOrDefault();
        }

        public virtual IEnumerable<NginxResourceEntity> GetAllNginxResources(ResourceSpec spec, bool excludeDeleted = true)
        {
            if (spec.resourceGroupName != null)
            {
                return ResourceTable.GetNginxResources(
                    nr =>
                        nr.UserSubscriptionId == spec.subscriptionId
                        && nr.ResourceGroup == spec.resourceGroupName,
                        //&& nr.Region.Equals(spec.locationName, StringComparison.OrdinalIgnoreCase),
                        excludeDeleted
                    );
            }
            else
            {
                return ResourceTable.GetNginxResources(
                    nr =>
                        nr.UserSubscriptionId == spec.subscriptionId,
                        //&& nr.Region.Equals(spec.locationName, StringComparison.OrdinalIgnoreCase),
                        excludeDeleted
                    );
            }
        }
    }
}
