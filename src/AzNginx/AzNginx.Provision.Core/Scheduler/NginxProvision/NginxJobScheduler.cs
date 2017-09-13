using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzNginx.Provision.Core.Storage.Nginx;
using AzNginx.Provision.Core.Storage;
using AzNginx.Provision.Core.Scheduler;
using AzNginx.Provision.Core.ServiceSettings;

namespace AzNginx.Provision.Core.NginxProvision
{
    public class NginxJobScheduler : IJobScheduler
    {
        public static readonly int JobVisibilityTimeoutInMS = 6000;

        NginxProvisionJobTable jobTable;
        NginxProvisionJobQueue jobQueue;
        NginxProvisionSettings settings;
        TimeSpan jobVisibilityTimeout;

        public IJobTable JobTable
        {
            get
            {
                return jobTable;
            }
        }

        public NginxJobScheduler(NginxProvisionSettings settings)
        {
            this.settings = settings;
            jobVisibilityTimeout = new TimeSpan(0, 0, 0, 0, JobVisibilityTimeoutInMS);

            jobTable = new NginxProvisionJobTable();
            jobQueue = new NginxProvisionJobQueue();

            // jobScheduler = new VMDeployerJobScheduler(packageDeployerSettings);
            jobTable.Initialize(settings.ProvisionStorageAccount, settings.JobTableName);
            jobQueue.Initialize(settings.ProvisionStorageAccount, settings.JobQueueName);
        }
    }
}
