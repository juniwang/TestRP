using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzNginx.Common;
using AzNginx.Common.Exceptions;

namespace AzNginx.Provision.Core.Sub
{
    public abstract class ResourcePool<TResourceKey, TResourceDetails, TResourceUsage, TResourceSelectionParams> : IDisposable
    {
        protected Timer _resourceRefreshTimer;
        private Random rnd = new Random();
        protected Dictionary<TResourceKey, TResourceDetails> _pool = new Dictionary<TResourceKey, TResourceDetails>();
        DateTimeOffset _lastResourceUsageAlertRaised = DateTimeOffset.MinValue;

        public abstract Task RefreshResourceUsage();
        protected abstract Task<Tuple<bool, string, TResourceUsage>> RefreshSingleResource(TResourceKey resourceKey);
        protected abstract bool IsResourceUsageCritical(TResourceUsage allUsage);
        protected abstract bool IsResourceUsageCritical(TResourceDetails monikerDetails);
        protected abstract IEnumerable<TResourceKey> GetResourcesOrderedByUsage(List<TResourceKey> fallBackResourcesList, TResourceSelectionParams input);
        public abstract Task UpdateUsageCountAsync(TResourceKey entry);
        protected abstract bool HasEnoughResources(TResourceDetails monikerDetails, TResourceSelectionParams input);

        public virtual void RefreshPool()
        {
            // no-op
        }

        protected virtual void ProcessCompletedCalls(Task<Tuple<bool, string, TResourceUsage>> completed)
        {
            // no-op
        }

        public int Count
        {
            get
            {
                return _pool.Count;
            }
        }

        #region Timer
        void PauseTimer()
        {
            TimeSpan disabled = TimeSpan.FromMilliseconds(-1);
            _resourceRefreshTimer.Change(disabled, disabled);
        }

        public virtual TimeSpan ResourceUsageRefreshInterval
        {
            get { return TimeSpan.FromMinutes(30); }
        }

        void ResumeTimer(TimeSpan dueTime)
        {
            _resourceRefreshTimer.Change(dueTime, ResourceUsageRefreshInterval);
        }

        public TimeSpan GetNextDueTime(TimeSpan elapsedTime)
        {
            if (elapsedTime > ResourceUsageRefreshInterval)
            {
                return TimeSpan.Zero; //Timer should fire immediately
            }
            else
            {
                return ResourceUsageRefreshInterval - elapsedTime;
            }
        }

        protected async void RefreshResourceUsageTimerProc(object s)
        {
            var started = DateTimeOffset.Now;
            try
            {
                // Refreshing the list can sometimes take a while, so pause the timer so that we don't get multiple timer events running simultaneously
                PauseTimer();
                await RefreshResourceUsage();
            }
            finally
            {
                TimeSpan elapsed = started - DateTimeOffset.Now;
                TimeSpan dueTime = GetNextDueTime(elapsed);
                ResumeTimer(dueTime);
            }
        }

        public virtual TimeSpan ResourceUsageRefreshInitialDelay // allows for Dependency Injection to complete before timer fires for first time
        {
            get { return TimeSpan.FromMinutes(1); }
        }

        #endregion

        protected TResourceDetails Lookup(TResourceKey resourceKey)
        {
            return _pool[resourceKey];
        }

        public async Task<TResourceKey> FindLeastUtilizedResource(TResourceSelectionParams input)
        {
            var resourceList = new List<TResourceKey>();
            List<TResourceKey> fallBackResourcesList = new List<TResourceKey>();
            // Sort by existing data
            foreach (var entry in GetResourcesOrderedByUsage(_pool.Keys.ToList(), input))
            {
                // Update with the latest resource counts
                await UpdateUsageCountAsync(entry);
                var monikerDetails = Lookup(entry);
                // in cases where we just want to return the least utilized resource
                // without checking the selection criteria
                if (input == null || HasEnoughResources(monikerDetails, input))
                {
                    if (!IsResourceUsageCritical(monikerDetails))
                    {
                        return entry;
                    }
                    else
                    {
                        fallBackResourcesList.Add(entry);
                    }
                }
            }

            // If no resources found, use any of the fallback resources
            if (fallBackResourcesList.Count() > 0)
            {
                var fallbackResource = GetResourcesOrderedByUsage(fallBackResourcesList, input).First();
                return fallbackResource;
            }

            throw new FatalException("Resources are all fully loaded");
        }

        public async Task<TResourceKey> FindRandomLeastUtilizedResource(TResourceSelectionParams input)
        {
            int leastUtilizedResources = 30;
            List<TResourceKey> resourceList = GetResourcesOrderedByUsage(_pool.Keys.ToList(), input).ToList();
            List<TResourceUsage> subs = new List<TResourceUsage>();
            List<TResourceKey> fallBackResourcesList = new List<TResourceKey>();
            // Sort by existing data
            while (resourceList.Any())
            {
                int idx = 0;
                if (resourceList.Count >= leastUtilizedResources)
                {
                    idx = rnd.Next(leastUtilizedResources);
                }
                else
                {
                    idx = rnd.Next(resourceList.Count);
                }
                var entry = resourceList[idx];

                // Update with the latest resource counts
                await UpdateUsageCountAsync(entry);
                var monikerDetails = Lookup(entry);
                // in cases where we just want to return the least utilized resource
                // without checking the selection criteria
                if (input == null || HasEnoughResources(monikerDetails, input))
                {
                    if (!IsResourceUsageCritical(monikerDetails))
                    {
                        return entry;
                    }
                    else
                    {
                        fallBackResourcesList.Add(entry);
                    }
                }
                //Remove resource from consideration
                resourceList.RemoveAt(idx);
            }

            // If no resources found, use any of the fallback resources
            if (fallBackResourcesList.Count() > 0)
            {
                var fallbackResource = GetResourcesOrderedByUsage(fallBackResourcesList, input).First();
                return fallbackResource;
            }

            throw new FatalException("Resources are all fully loaded");
        }

        protected virtual async Task<int> DoRefreshWork()
        {
            if (_pool.Count == 0)
            {
                AzureLog.Fatal("{0} -> No resources present in the pool. Returning...",
                    GetResourceUsageFunctionName());
                return 0;
            }

            int resourceCount = 0;

            var outstandingCalls = new List<Task<Tuple<bool, string, TResourceUsage>>>();
            _pool.Keys.ToList().ForEach(p => outstandingCalls.Add(RefreshSingleResource(p)));

            while (outstandingCalls.Count > 0)
            {
                var completed = await Task.WhenAny(outstandingCalls);
                outstandingCalls.Remove(completed);
                resourceCount++;

                if (!completed.Result.Item1)
                {
                    AzureLog.Warn(completed.Result.Item2);
                }

                // process completed calls outside of if/else statement 
                // so that we always do the processing on all the RefreshSingleResource calls irrespective of their outcome.
                ProcessCompletedCalls(completed);
            }
            return resourceCount;
        }

        protected string GetResourceUsageFunctionName()
        {
            return "Refresh" + this.GetType().Name + "Usage";
        }

        protected virtual void LogResourceUsageStats(TResourceUsage allUsage, int resourceCount)
        {
            if (IsResourceUsageCritical(allUsage) &&
                _lastResourceUsageAlertRaised < (DateTimeOffset.Now - TimeSpan.FromDays(1)))
            {
                _lastResourceUsageAlertRaised = DateTimeOffset.Now;
                AzureLog.Fatal("{0} -> Critically low resources. ResourceCount: {1}, Usage:{2} ",
                    GetResourceUsageFunctionName(),
                    resourceCount.ToString(),
                    allUsage.ToString());
            }
            else
            {
                AzureLog.Info("{0} ->ResourceCount: {1}, Usage: {2}  ",
                    GetResourceUsageFunctionName(),
                    resourceCount.ToString(),
                    allUsage.ToString());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _resourceRefreshTimer.Dispose();
            }
        }
    }
}
