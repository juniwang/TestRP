using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using AzNginx.Provision.Core.ServiceSettings;
using AzNginx.Storage;
using AzNginx.Storage.Entities;
using AzNginx.Provision.Core.Handlers;
using System.Threading;
using AzNginx.Storage.Provision;
using AzNginx.Common;
using AzNginx.Storage.Entities.Provision;

namespace AzNginx.Provision.Core.Scheduler.NginxProvision
{
    public class NginxJobProcessor : JobProcessorBase<NginxProvisionEntity>, IJobProcessor
    {
        /// <summary>
        /// The job visibility timeout for write in milliseconds.
        /// </summary>
        public static readonly int JobVisibilityTimeoutMS = 6000;

        static IDictionary<NginxProvisionState, Type> provisionHandlers = new Dictionary<NginxProvisionState, Type>()
        {
            { NginxProvisionState.ACRInit, typeof(ACRInitHandler) },
            { NginxProvisionState.ACRStatus, typeof(ACRStatusHandler) },
        };

        NginxProvisionJobTable jobTable;
        NginxProvisionJobQueue jobQueue;
        NginxProvisionSettings settings;
        TimeSpan jobVisibilityTimeout;

        public NginxJobProcessor(NginxProvisionSettings settings)
        {
            this.jobVisibilityTimeout = new TimeSpan(0, 0, 0, 0, JobVisibilityTimeoutMS);
            this.settings = settings;

            jobTable = new NginxProvisionJobTable();
            jobQueue = new NginxProvisionJobQueue();

            // jobScheduler = new VMDeployerJobScheduler(packageDeployerSettings);
            jobTable.Initialize(settings.ProvisionStorageAccount, settings.JobTableName);
            jobQueue.Initialize(settings.ProvisionStorageAccount, settings.JobQueueName);

            base.Init(jobTable, jobQueue);
        }

        public NginxProvisionJobQueue JobQueue
        {
            get { return jobQueue; }
        }

        public bool OnJob(CloudQueueMessage jobQueueMessage, out ExceptionDetails exceptionDetails)
        {
            JobQueueEntry jobQueueEntry = JobQueueEntry.ParseJobEntry(jobQueueMessage);
            return OnProvisionJob(jobQueueEntry, out exceptionDetails);
        }

        private bool OnProvisionJob(JobQueueEntry jobQueueEntry, out ExceptionDetails exceptionDetails)
        {
            exceptionDetails = null;
            if (jobQueueEntry.EntityType == EntityType.ProvisionNginx)
            {
                NginxProvisionEntity entity = null;
                GetJobEntity(jobQueueEntry, out entity);
                if (entity == null)
                {
                    return false;
                }

                if (entity.IsSucceeded() || entity.IsFailed())
                {
                    if (!jobQueue.DeleteJob(jobQueueEntry.CloudQueueMessage, out exceptionDetails))
                    {
                        // log?
                    }
                    return false;
                }

                // provision
                INginxHandler<NginxProvisionEntity> handler = CreateProvisionHandler(entity.JobState);
                handler.Init(jobTable, jobQueue);
                handler.Handle(entity, CancellationToken.None).Wait();
                return true;
            }
            else
            {
                return false;
            }
        }

        private INginxHandler<NginxProvisionEntity> CreateProvisionHandler(NginxProvisionState state)
        {
            if (!provisionHandlers.ContainsKey(state))
            {
                throw new InvalidOperationException(string.Format("No valid handler registered for the job state {0}", state.ToString()));
            }

            var type = provisionHandlers[state];
            var handlerConstructor = type.GetConstructor(Type.EmptyTypes);
            if (handlerConstructor == null)
            {
                throw new InvalidOperationException(string.Format("Handler requires default constructor {0}", type.Name));
            }

            var handler = (INginxHandler<NginxProvisionEntity>)handlerConstructor.Invoke(new object[] { });
            return handler;
        }
    }
}
