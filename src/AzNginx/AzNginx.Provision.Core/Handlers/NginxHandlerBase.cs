using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzNginx.Storage;
using AzNginx.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzNginx.Provision.Core.Handlers
{
    public abstract class NginxHandlerBase<TEntity> : INginxHandler<TEntity>
        where TEntity : JobEntityBase, new()
    {
        private const int defaultMaxRetry = 5;

        protected JobTable<TEntity> JobTable { get; set; }
        protected JobQueue JobQueue { get; set; }

        public abstract Task Handle(TEntity entity, CancellationToken cancellationToken);

        public void Init(JobTable<TEntity> jobTable, JobQueue jobQueue)
        {
            this.JobTable = jobTable;
            this.JobQueue = jobQueue;
        }

        #region Retry
        protected async Task ScheduleForRetry(TEntity jobEntity, CancellationToken cancellationToken, int visibilityTimeoutInMS = 10000)
        {
            jobEntity.RetryCount++;
            await UpdateJobAndSchedule(jobEntity, cancellationToken, visibilityTimeoutInMS);
        }

        protected void ResetRetryState(TEntity jobEntity)
        {
            jobEntity.RetryCount = 0;
        }

        public static bool MaxRetryReached(TEntity jobEntity, int maxRetry = defaultMaxRetry)
        {
            return jobEntity.RetryCount >= defaultMaxRetry;
        }

        /// <summary>
        /// Method that invokes an action that is prone to transient failure. If the invoked action failed
        /// due to the transient failure and caller says this is not the last retry 
        /// (MaxRetryReached() == false) then return exception info wrapped inside 'RetriableActionResult'
        /// instance. If caller says this was the last retry then rethrow the exception (irrespective of 
        /// the exception type).
        /// 
        /// If the action succeeded then warp the action result in 'RetriableActionResult' and return it.
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="maxRetryReached"></param>
        /// <returns></returns>
        protected async Task<RetriableActionResult<TResult>>
            RetriableActionAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken,
            bool maxRetryReached) where TResult : class
        {
            try
            {
                TResult actionResult = await action(cancellationToken);
                return new RetriableActionResult<TResult>
                {
                    ActionResult = actionResult,
                    Exception = null
                };
            }
            catch (Exception exception)
            {
                if (!maxRetryReached)
                {
                    return new RetriableActionResult<TResult>
                    {
                        ActionResult = null,
                        Exception = exception,
                    };
                }

                throw;
            }
        }
        #endregion

        protected virtual async Task UpdateJobAndStop(TEntity jobEntity, CancellationToken cancellationToken)
        {
            await JobTable.ReplaceUpdateEntityAsync(jobEntity, cancellationToken);
            CloudQueueMessage jobQqueueMessage = jobEntity.GetJobQueueEntry().CloudQueueMessage;
            await JobQueue.DeleteJobAsync(jobQqueueMessage, cancellationToken);
        }

        protected async Task UpdateJobAndSchedule(TEntity jobEntity,
            CancellationToken cancellationToken,
            int visibilityTimeoutInMS = 10000)
        {
            await JobTable.ReplaceUpdateEntityAsync(jobEntity, cancellationToken);
            CloudQueueMessage jobQqueueMessage = jobEntity.GetJobQueueEntry().CloudQueueMessage;
            await JobQueue.UpdateJobVisibilityTimeOutAsync(jobQqueueMessage,
                new TimeSpan(0, 0, 0, 0, visibilityTimeoutInMS), cancellationToken);
        }

    }

    public class RetriableActionResult<T>
    {
        public T ActionResult { get; set; }
        public Exception Exception { get; set; }
        public bool isFailed
        {
            get
            {
                return Exception != null;
            }
        }
    };
}
