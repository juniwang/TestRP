using Microsoft.WindowsAzure.Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AzNginx.Provision.Core.Sub
{
    public interface IAzureResourceManager
    {
         Task<SubscriptionUsage> GetSubscriptionUsage(IAzureSubscription subscription);
    }

    public class AzureResourceManager : IAzureResourceManager
    {
        public Task<SubscriptionUsage> GetSubscriptionUsage(IAzureSubscription subscription)
        {
            using (var client = subscription.CreateManagementClient())
            {
                SubscriptionGetResponse resp = client.Subscriptions.GetAsync(CancellationToken.None).Result;

                return Task.FromResult(new SubscriptionUsage
                (
                    currentCores: resp.CurrentCoreCount,
                    maxCores: resp.MaximumCoreCount,
                    currentStorageAccounts: resp.CurrentStorageAccounts,
                    maxStorageAccounts: resp.MaximumStorageAccounts
                ));
            }
        }
    }
}
