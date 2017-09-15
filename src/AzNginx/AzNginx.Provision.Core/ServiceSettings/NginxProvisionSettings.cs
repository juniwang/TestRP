using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzNginx.Provision.Core.Scheduler;
using AzNginx.Provision.Core.Scheduler.NginxProvision;
using Microsoft.WindowsAzure.Storage;
using AzNginx.Common;

namespace AzNginx.Provision.Core.ServiceSettings
{
    public class NginxProvisionSettings : IServiceSettings
    {
        public int jobQueueMessageVisibilityTimeoutMS; // in millisecond
        int jobQueueDequeueIntervalMS;
        short jobQueueConcurrency;
        short jobQueueBatchSize;
        IList<IProducerQueueInfo> producerQueueInfos;
        NginxJobProcessor jobProcessor;

        public NginxProvisionSettings()
        {
            ProvisionStorageAccount = CloudStorageAccount.Parse(AzNginxConfiguration.Storage.ConnectionString);
            jobQueueMessageVisibilityTimeoutMS = AzNginxConfiguration.Storage.MessageVisibilityTimeoutMS;
            jobQueueDequeueIntervalMS = AzNginxConfiguration.Storage.DequeueIntervalMS;
            jobQueueConcurrency = AzNginxConfiguration.Storage.JobQueueConcurrency;
            jobQueueBatchSize = AzNginxConfiguration.Storage.DequeueBatchSize;
            JobQueueName = AzNginxConfiguration.Storage.ProvisionJobQueueName;
            JobTableName = AzNginxConfiguration.Storage.ProvisionJobTableName;
        }

        /// <summary>
        /// Gets the name of the Azure Queue holding the jobs supported by NginxProvision service.
        /// </summary>
        public string JobQueueName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the name of the Azure Table holding the job description pointed by 
        /// jobs in Azure Job Queue ('NginxProvisionSettings.JobQueueName').
        /// </summary>
        public string JobTableName
        {
            get;
            internal set;
        }

        public CloudStorageAccount ProvisionStorageAccount
        {
            get;
            internal set;
        }

        private NginxJobProcessor NginxJobProcessor
        {
            get
            {
                if (jobProcessor == null)
                {
                    jobProcessor = new NginxJobProcessor(this);
                }

                return jobProcessor;
            }
        }

        #region Implementation of  IServiceSettings
        public JobDispatchingType JobDispatchingType
        {
            get
            {
                return JobDispatchingType.NoWaitAnyTime;
            }
        }

        public IJobProcessor JobProcessor
        {
            get
            {
                return NginxJobProcessor;
            }
        }

        public IList<IProducerQueueInfo> ProducerQueueInfos
        {
            get
            {
                if (producerQueueInfos == null)
                {
                    producerQueueInfos = new List<IProducerQueueInfo>();
                    producerQueueInfos.Add(new ProducerQueueInfo(1,
                        jobQueueMessageVisibilityTimeoutMS,
                        jobQueueConcurrency,
                        jobQueueBatchSize,
                        jobQueueDequeueIntervalMS,
                        NginxJobProcessor.JobQueue.GetJobs));
                }

                return producerQueueInfos;
            }
        }
        #endregion
    }
}
