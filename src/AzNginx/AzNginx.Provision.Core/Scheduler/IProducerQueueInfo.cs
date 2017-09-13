using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Scheduler
{
    /// <summary>
    /// Represents a single queue that produces the jobs.
    /// </summary>
    public interface IProducerQueueInfo
    {
        /// <summary>
        /// A unique Id associated with the queue
        /// </summary>
        int QueueId { get; }

        /// <summary>
        /// The visibility time out of the job messages those are
        /// fetched by the long running task.
        /// </summary>
        TimeSpan? VisibilityTimeOut { get; }

        /// <summary>
        /// Number of long running tasks that polls the job queue.
        /// Each long running task fetch and process 'DequeueBatchSize'
        /// jobs at a time.
        /// </summary>
        short Concurrency { get; }

        /// <summary>
        /// The maximum number of jobs needs to be fetched as a batch
        /// by a single long running task.
        /// </summary>
        short DequeueBatchSize { get; }

        /// <summary>
        /// The time in milliseconds to wait after fetching a batch of
        /// jobs from the job queue.
        /// </summary>
        TimeSpan? DequeueInterval { get; }

        /// <summary>
        /// The function reference used by each long running task to
        /// retrieve 'DequeueBatchSize' jobs from the job queue.
        /// </summary>
        GetJobsDelegate GetJobs { get; }
    }


    /// <summary>
    /// Implementation of IProducerQueueInfo.
    /// </summary>
    public class ProducerQueueInfo : IProducerQueueInfo
    {
        private int queueId;
        private short concurrency;
        private short dequeueBatchSize;
        private TimeSpan? dequeueInterval;
        private TimeSpan? visibilityTimeOut;
        private GetJobsDelegate getJobs;

        /// <summary>
        /// Constructs ProducerQueueInfo instance representing details of a queue.
        /// </summary>
        /// <param name="queueId">Id assigned to the queue.</param>
        /// <param name="visibilityTimeOutInMilliSeconds">Visibility timeout for the 
        /// messages read.</param>
        /// <param name="concurrency">Number of tasks that can read from this queue 
        /// in parallel.</param>
        /// <param name="dequeueBatchSize">Number of jobs to be fetched by a single 
        /// task.</param>
        /// <param name="dequeueIntervalInMilliSeconds">Number of milliseconds to wait 
        /// after reading tasks.</param>
        /// <param name="getJobs">Reference to the function to be used for fetching 
        /// the jobs.</param>
        public ProducerQueueInfo(int queueId, int visibilityTimeOutInMilliSeconds,
            short concurrency,
            short dequeueBatchSize,
            int dequeueIntervalInMilliSeconds,
            GetJobsDelegate getJobs)
        {
            this.queueId = queueId;
            this.visibilityTimeOut = new TimeSpan(0, 0, 0, 0, visibilityTimeOutInMilliSeconds);
            this.concurrency = concurrency;
            this.dequeueBatchSize = dequeueBatchSize;
            this.dequeueInterval = new TimeSpan(0, 0, 0, 0, dequeueIntervalInMilliSeconds);
            this.getJobs = getJobs;
        }

        public int QueueId
        {
            get
            {
                return this.queueId;
            }
        }

        public TimeSpan? VisibilityTimeOut
        {
            get
            {
                return this.visibilityTimeOut;
            }
        }

        public short Concurrency
        {
            get
            {
                return this.concurrency;
            }
        }

        public short DequeueBatchSize
        {
            get
            {
                return this.dequeueBatchSize;
            }
        }

        public TimeSpan? DequeueInterval
        {
            get
            {
                return this.dequeueInterval;
            }
        }

        public GetJobsDelegate GetJobs
        {
            get
            {
                return this.getJobs;
            }
        }
    }
}
