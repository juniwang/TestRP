using AzNginx.Provision.Core.ServiceSettings;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Scheduler
{
    /// <summary>
    /// Delegate whose instance can hold reference to a function that can be 
    /// used to get jobs.
    /// </summary>
    /// <param name="batchSize">The number of jobs to be fetched.</param>
    /// <param name="visibilityTimeOut">The visibility time out of the job 
    /// messages.</param>
    /// <param name="exceptionDetails">The exception details if any</param>
    /// <returns></returns>
    public delegate IEnumerable<CloudQueueMessage> GetJobsDelegate(
        short batchSize,
        TimeSpan? visibilityTimeOut,
        out ExceptionDetails exceptionDetails);

    public interface IJobManager
    {/// <summary>
     /// Initialization.
     /// </summary>
        void Initialize(IServiceSettings serviceSettings);

        /// <summary>
        /// The type of dispatching needed by the job types managed 
        /// by this IJobManager instance.
        /// </summary>
        JobDispatchingType JobDispatchingType { get; }

        /// <summary>
        /// Gets the job producer.
        /// </summary>
        IJobProducer JobProducer { get; }

        /// <summary>
        /// Gets the job consumer.
        /// </summary>
        IJobConsumer JobConsumer { get; }

        /// <summary>
        /// Mechanism to let this IJobManager instance know from the 
        /// Dispatcher that some exception occured.
        /// </summary>
        /// <param name="exception"></param>
        void OnError(Exception exception);
    }

    /// <summary>
    /// JobManager provide Dispatcher a way to access the IJobProducer instance 
    /// which act as an interface to retrieve the jobs (and to retrieve settings 
    /// such as number of jobs to retrieve in parallel, time to wait before 
    /// retrieving next batch of jobs etc..) and a way to access the IJobConsumer 
    /// instance which act as a handler for jobs processing. 
    /// </summary>
    public class JobManager : IJobManager, IJobProducer, IJobConsumer
    {
        IServiceSettings serviceSettings;

        #region IJobManager implementation

        /// <summary>
        /// Initializes JobManager.
        /// </summary>
        /// <param name="serviceSettings"></param>
        public void Initialize(IServiceSettings serviceSettings)
        {
            this.serviceSettings = serviceSettings;
        }

        /// <summary>
        /// The type of dispatching needed by the job types managed 
        /// by this JobManager instance.
        /// </summary>
        public JobDispatchingType JobDispatchingType
        {
            get
            {
                return this.serviceSettings.JobDispatchingType;
            }
        }

        /// <summary>
        /// Gets reference to the IJobProducer which can be used 
        /// to retrieve the jobs and to retrieve settings such as 
        /// number of jobs to retrieve in parallel, time to wait 
        /// before retrieving next batch of jobs etc..
        /// </summary>
        public IJobProducer JobProducer
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets reference to the IJobConsumer which can be act as 
        /// a handler for jobs processing.
        /// </summary>
        public IJobConsumer JobConsumer
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// A way to let JobManager know from the Dispatcher that some 
        /// exception occured.
        /// Note: The impleemntation of this method must be thread safe 
        /// as it can be called from multiple long running tasks spinned 
        /// up by the dispatcher.
        /// </summary>
        /// <param name="exception"></param>
        public void OnError(Exception exception)
        {
        }

        #endregion

        #region IJobProducer implementation

        /// <summary>
        /// IJobProducer::ProducerQueueInfos implementation.
        /// </summary>
        public IList<IProducerQueueInfo> ProducerQueueInfos
        {
            get
            {
                return serviceSettings.ProducerQueueInfos;
            }
        }

        #endregion

        #region IJobConsumer implementation

        /// <summary>
        /// IJobConsumer::OnJob implementation.
        /// </summary>
        /// <param name="QueueId"></param>
        /// <param name="jobQueueMessage"></param>
        /// <param name="exceptionDetails"></param>
        /// <returns></returns>
        public bool OnJob(int QueueId, CloudQueueMessage jobQueueMessage,
            out ExceptionDetails exceptionDetails)
        {
            exceptionDetails = null;
            return this.serviceSettings.JobProcessor.OnJob(jobQueueMessage,
                out exceptionDetails);
        }

        #endregion
    }
}
