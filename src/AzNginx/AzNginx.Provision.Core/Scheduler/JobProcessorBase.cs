using AzNginx.Provision.Core.Handlers;
using AzNginx.Provision.Core.Storage;
using AzNginx.Provision.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Scheduler
{
    public abstract class JobProcessorBase
    {
        protected JobTable jobTableBase;
        protected JobQueue jobQueueBase;

        protected void Init(JobTable jobTable, JobQueue jobQueue)
        {
            this.jobTableBase = jobTable;
            this.jobQueueBase = jobQueue;
        }

        /// <summary>
        /// Gets a job entity from job table referrenced by the given queue entry.
        /// </summary>
        /// <typeparam name="T">The type of the job entity.</typeparam>
        /// <param name="jobQueueEntry">The job queue entry.</param>
        /// <param name="jobEntity">The retrieved job entity on return.</param>
        protected void GetJobEntity<T>(JobQueueEntry jobQueueEntry, out T jobEntity) where T : EntityBase
        {
            ExceptionDetails exceptionDetails = null;

            bool success = this.jobTableBase.GetJob<T>(jobQueueEntry, out jobEntity, out exceptionDetails);
            if (!success)
            {
                HandleJobEntityNotFound(jobQueueEntry, exceptionDetails);
            }
            else
            {
                jobEntity.SetJobQueueEntry(jobQueueEntry);
            }
        }

        /// <summary>
        /// Handle the case where job entity not found in the job table.
        /// </summary>
        /// <param name="jobQueueEntry">The jobQueueEntry.</param>
        /// <param name="exceptionDetails">Contains any exception while trying to handle not found case.</param>
        private void HandleJobEntityNotFound(JobQueueEntry jobQueueEntry, ExceptionDetails exceptionDetails)
        {
            if (exceptionDetails.HttpStatusCode == HttpStatusCode.NotFound)
            {
                if (!jobQueueBase.DeleteJob(jobQueueEntry.CloudQueueMessage, out exceptionDetails))
                {
                    // TODO: Log
                }
            }
        }
    }
}
