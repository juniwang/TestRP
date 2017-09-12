using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestRP.Common;

namespace TestRP.Provision.Core.Sub
{
    public sealed class SubscriptionPool
        : ResourcePool<IAzureSubscription, SubscriptionDetails, SubscriptionUsage, SubscriptionSelectionParams>
    {
        public IAzureResourceManager AzureResourceMgr { get; set; }

        int allSubCurrentCores = 0;
        int allSubMaxCores = 0;
        int allSubCurrentStorageAccounts = 0;
        int allSubMaxStorageAccounts = 0;

        public SubscriptionPool()
        {
            _resourceRefreshTimer = new Timer(RefreshResourceUsageTimerProc, null, ResourceUsageRefreshInitialDelay, ResourceUsageRefreshInterval);
        }

        #region Abstract members
        public override async Task RefreshResourceUsage()
        {
            if (AzureResourceMgr != null)
            {
                allSubCurrentCores = 0;
                allSubMaxCores = 0;
                allSubCurrentStorageAccounts = 0;
                allSubMaxStorageAccounts = 0;

                int subscriptionCount = await DoRefreshWork();

                // log overall pool usage
                var allSubscriptionUsage = new SubscriptionUsage
                (
                    currentCores: allSubCurrentCores,
                    maxCores: allSubMaxCores,
                    currentStorageAccounts: allSubCurrentStorageAccounts,
                    maxStorageAccounts: allSubMaxStorageAccounts
                );
                LogResourceUsageStats(allSubscriptionUsage, subscriptionCount);
            }
        }

        public override async Task UpdateUsageCountAsync(IAzureSubscription subscription)
        {
            var details = Lookup(subscription);
            details.Usage = await AzureResourceMgr.GetSubscriptionUsage(subscription);
            AzureLog.Info("Subscription: {0} Usage: {1}", subscription.SubscriptionId, details.Usage.ToString());
        }

        protected override IEnumerable<IAzureSubscription> GetResourcesOrderedByUsage(List<IAzureSubscription> fallBackResourcesList, SubscriptionSelectionParams input)
        {
            double coreToStorageRatio = GetCoreToStorageRatio(input);
            IEnumerable<IAzureSubscription> fallBackResourcesListWithoutMaxedOutResources;
            IEnumerable<IAzureSubscription> fallBackResourcesListWithMaxedOutResources;
            if (input != null)
            {
                fallBackResourcesListWithoutMaxedOutResources = fallBackResourcesList.Where(e => HasEnoughResources(_pool[e], input));
                fallBackResourcesListWithMaxedOutResources = fallBackResourcesList.Where(e => !HasEnoughResources(_pool[e], input));
            }
            else
            {
                fallBackResourcesListWithoutMaxedOutResources = fallBackResourcesList.Where(e => _pool[e].Usage.AvailableCores > 0 && _pool[e].Usage.AvailableStorageAccounts > 0);
                fallBackResourcesListWithMaxedOutResources = fallBackResourcesList.Where(e => !(_pool[e].Usage.AvailableCores > 0 && _pool[e].Usage.AvailableStorageAccounts > 0));
            }
            return fallBackResourcesListWithoutMaxedOutResources.OrderByDescending(e => _pool[e].Usage.AvailableCores * coreToStorageRatio + _pool[e].Usage.AvailableStorageAccounts).Concat(fallBackResourcesListWithMaxedOutResources);

        }

        protected override bool HasEnoughResources(SubscriptionDetails monikerDetails, SubscriptionSelectionParams input)
        {
            return monikerDetails.Usage.HasEnoughResources(input.VmSize, input.InstanceCount);
        }

        protected override bool IsResourceUsageCritical(SubscriptionDetails monikerDetails)
        {
            return monikerDetails.Usage.CoreUsageCritical;
        }

        protected override bool IsResourceUsageCritical(SubscriptionUsage allUsage)
        {
            return allUsage.ResourceUsageCritical;
        }

        protected override async Task<Tuple<bool, string, SubscriptionUsage>> RefreshSingleResource(IAzureSubscription subscription)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var subDetails = _pool[subscription];
            bool success = true;
            string message;

            try
            {
                subDetails.Usage = await AzureResourceMgr.GetSubscriptionUsage(subscription);
                message = string.Format("RefreshSubscriptionUsage for subid {0} with Usage {1}.  ElapsedTime {2}", subscription.SubscriptionId, subDetails.Usage, sw.Elapsed);

                if (subDetails.Usage.CoreUsageCritical)
                {
                    string details = string.Format("RefreshSubscriptionUsage -> Critically low resources. SubscriptionId: {0}, Usage:{1}",
                        subscription.SubscriptionId,
                        subDetails.Usage);

                    // Explicitly provide messageId and requestId to avoid confusion around which overload we are calling
                    AzureLog.Fatal(details);
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = string.Format("Subscription {0} usage refresh failed with {1}.  ElapsedTime {2}", subscription.SubscriptionId, ex.ToString(), sw.Elapsed);
            }

            return Tuple.Create(success, message, subDetails.Usage);
        }
        #endregion

        public void Add(IAzureSubscription subscription, string subName)
        {
            var details = new SubscriptionDetails()
            {
                Name = subName,
                Usage = new SubscriptionUsage(
                    currentCores: 0,
                    maxCores: 200,
                    currentStorageAccounts: 0,
                    maxStorageAccounts: 100)
            };
            _pool.Add(subscription, details);
        }

        public IAzureSubscription GetById(string subscriptionId)
        {
            return _pool.SingleOrDefault(e => e.Key.SubscriptionId.Equals(subscriptionId, StringComparison.OrdinalIgnoreCase)).Key;
        }

        public IAzureSubscription GetByName(string subscriptionName)
        {
            return _pool.SingleOrDefault(e => e.Value.Name.Equals(subscriptionName, StringComparison.OrdinalIgnoreCase)).Key;
        }

        protected override void ProcessCompletedCalls(Task<Tuple<bool, string, SubscriptionUsage>> completed)
        {
            allSubCurrentCores += completed.Result.Item3.CurrentCores;
            allSubMaxCores += completed.Result.Item3.MaxCores;
            allSubCurrentStorageAccounts += completed.Result.Item3.CurrentStorageAccounts;
            allSubMaxStorageAccounts += completed.Result.Item3.MaxStorageAccounts;
        }

        private double GetCoreToStorageRatio(SubscriptionSelectionParams input)
        {
            double coreToStorageRatio = 1;
            if (input != null)
            {
                coreToStorageRatio = input.CoreToStorageRatio;
            }

            return coreToStorageRatio;
        }

    }
}
