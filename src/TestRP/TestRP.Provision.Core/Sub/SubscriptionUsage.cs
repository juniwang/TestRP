using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Provision.Core.Sub
{
    public class SubscriptionUsage
    {
        public const int CoreUsageAlertLevel = 90; // To account for scale 
        public const int StorageAccountUsageAlertLevel = 95;

        public int CurrentCores { get; private set; }
        public int MaxCores { get; private set; }
        public int CurrentStorageAccounts { get; private set; }
        public int MaxStorageAccounts { get; private set; }
        public int CurrentLoadBalancers { get; private set; }
        public int MaxLoadBalancers { get; private set; }

        public SubscriptionUsage(int currentCores, int maxCores, int currentStorageAccounts, int maxStorageAccounts)
        {
            CurrentCores = currentCores;
            MaxCores = maxCores;
            CurrentStorageAccounts = currentStorageAccounts;
            MaxStorageAccounts = maxStorageAccounts;
        }

        public SubscriptionUsage(
            int currentCores, int maxCores,
            int currentStorageAccounts, int maxStorageAccounts,
            int currentLoadBalancers, int maxLoadBalancers)
            : this(currentCores, maxCores, currentStorageAccounts, maxStorageAccounts)
        {
            CurrentLoadBalancers = currentLoadBalancers;
            MaxLoadBalancers = maxLoadBalancers;
        }

        public int AvailableStorageAccounts
        {
            get { return MaxStorageAccounts - CurrentStorageAccounts; }
        }

        public int AvailableCores
        {
            get { return MaxCores - CurrentCores; }
        }

        public double PercentCoresUsed
        {
            get { return GetUsagePercentage(CurrentCores, MaxCores); }
        }

        public double PercentStorageAccountsUsed
        {
            get { return GetUsagePercentage(CurrentStorageAccounts, MaxStorageAccounts); }
        }

        public double PercentLoadBalancersUsed
        {
            get { return GetUsagePercentage(CurrentLoadBalancers, MaxLoadBalancers); }
        }

        public bool ResourceUsageCritical
        {
            get
            {
                return PercentCoresUsed > CoreUsageAlertLevel || PercentStorageAccountsUsed > StorageAccountUsageAlertLevel;
            }
        }

        public bool CoreUsageCritical
        {
            get
            {
                return PercentCoresUsed > CoreUsageAlertLevel;
            }
        }

        // Refer GenerateCsDefs.ps1 for vmSize to Azure VM size mapping
        // Refer https://azure.microsoft.com/en-us/documentation/articles/cloud-services-sizes-specs/ for size to core mappings
        public static int GetCoreCount(string vmSize)
        {
            switch (vmSize)
            {
                case "C0":
                    return 1;
                case "C1":
                    return 1;
                case "C2":
                    return 2;
                case "C3":
                    return 4;
                case "C4":
                    return 2;
                case "C5":
                    return 4;
                case "C6":
                    return 8;
                case "P1":
                    return 2;
                case "P2":
                    return 4;
                case "P3":
                    return 4;
                case "P4":
                    return 8;
                default:
                    throw new ArgumentException(string.Format("Unknown VM size {0}", vmSize));
            }
        }

        public double GetUsagePercentage(int currentCount, int maximumCount)
        {
            if (maximumCount == 0)
            {
                return 100.0;
            }

            double percentUsed = (currentCount * 100) / maximumCount;
            return Math.Round(percentUsed, 1);
        }

        public override string ToString()
        {
            return string.Format(
                "CurrentCoreCount: {0}, MaximumCoreCount: {1}, CurrentStorageAccounts: {2}, MaximumStorageAccounts:{3}",
                CurrentCores,
                MaxCores,
                CurrentStorageAccounts,
                MaxStorageAccounts);
        }

        public bool HasEnoughResources(string vmSize, int instanceCount)
        {
            if ((GetCoreCount(vmSize) * instanceCount) <= AvailableCores && AvailableStorageAccounts > 0)
            {
                return true;
            }

            return false;
        }
    }
}
