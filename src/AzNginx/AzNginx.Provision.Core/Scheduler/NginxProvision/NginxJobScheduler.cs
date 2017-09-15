using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzNginx.Storage;
using AzNginx.Provision.Core.Scheduler;
using AzNginx.Provision.Core.ServiceSettings;
using AzNginx.Storage.Entities;
using System.Threading;
using AzNginx.Storage.Provision;
using AzNginx.Storage.Entities.Provision;

namespace AzNginx.Provision.Core.NginxProvision
{
    /// <summary>
    /// A class to schedule job for nginx provision
    /// </summary>
    public class NginxJobScheduler : IJobScheduler<NginxProvisionEntity>
    {
        public static readonly int JobVisibilityTimeoutInMS = 6000;

        NginxProvisionJobTable jobTable;
        NginxProvisionJobQueue jobQueue;
        NginxProvisionSettings settings;
        TimeSpan jobVisibilityTimeout;

        public IJobTable<NginxProvisionEntity> JobTable
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

        public async Task StartProvision(NginxProvisionEntity entity)
        {
            await ScheduleJobInternalAsync(entity, CancellationToken.None);
        }

        private async Task ScheduleJobInternalAsync(NginxProvisionEntity jobEntity, CancellationToken cancellationToken)
        {
            string jobQueueReference = JobQueueEntry.formatQueryEntry(
                jobEntity.PartitionKey,
                jobEntity.RowKey,
                jobEntity.EntityType);
            await this.jobTable.AddEntityAysnc(jobEntity, cancellationToken);
            await this.jobQueue.AddJobAsync(jobQueueReference, jobVisibilityTimeout, cancellationToken);
        }
    }
}
