using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzNginx.DAL;
using TestRP.Provision.Core.Sub;
using AzNginx.Models;
using TestRP.Common.Exceptions;

namespace TestRP.Provision.Core
{
    public class DeploymentStore
    {
        public SubscriptionPool SubscriptionPool { get; set; }

        public NginxDbContext Context { get; set; }

        public async Task<NginxResource> GetNginxResource(ResourceSpec spec)
        {
            spec.MustSpecifyExactResource();

            var nginx = await TryGetDeployment(spec);
            if (nginx == null)
            {
                throw new NginxResourceDoesntExistException();
            }

            return nginx;
        }

        public virtual async Task<NginxResource> TryGetDeployment(ResourceSpec resource)
        {
            resource.MustSpecifyExactResource();

            return await Context.TryGetNginxResource(nr =>
                                nr.UserSubscriptionId == resource.subscriptionId
                                && nr.ResourceGroup == resource.resourceGroupName
                                && nr.ResourceName == resource.resourceName,
                                //&& nr.Location.Equals(resource.locationName, StringComparison.OrdinalIgnoreCase),
                                excludeDeleted: true);
        }

        public virtual async Task<IEnumerable<NginxResource>> GetAllNginxResources(ResourceSpec spec, bool excludeDeleted = true)
        {
            if (spec.resourceGroupName != null)
            {
                return await Context.GetNginxResourcesAsync(
                    nr =>
                        nr.UserSubscriptionId == spec.subscriptionId
                        && nr.ResourceGroup == spec.resourceGroupName,
                        //&& nr.Location.Equals(spec.locationName, StringComparison.OrdinalIgnoreCase),
                        excludeDeleted
                    );
            }
            else
            {
                return await Context.GetNginxResourcesAsync(
                    nr =>
                        nr.UserSubscriptionId == spec.subscriptionId,
                        //&& nr.Location.Equals(spec.locationName, StringComparison.OrdinalIgnoreCase),
                        excludeDeleted
                    );
            }
        }
    }
}
