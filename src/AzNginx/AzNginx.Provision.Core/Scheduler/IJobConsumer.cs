using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Scheduler
{
    /// <summary>
    /// Consumer that handles jobs
    /// </summary>
    public interface IJobConsumer
    {
        /// <summary>
        /// Handler for the job.
        /// </summary>
        /// <param name="QueueId">Id of the job queue from which this job retreived.</param>
        /// <param name="jobQueueMessage">the job to be processed.</param>
        /// <param name="exceptionDetails">the exception details.</param>
        /// <returns></returns>
        bool OnJob(int QueueId, CloudQueueMessage jobQueueMessage, out ExceptionDetails exceptionDetails);
    }
}
