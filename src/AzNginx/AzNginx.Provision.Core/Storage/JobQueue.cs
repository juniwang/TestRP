using AzNginx.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Storage
{
    public class JobQueue : AzureQueue
    {
        /// <summary>
        /// the name of the job queue.
        /// </summary>
        protected string jobQueueName;

        /// <summary>
        /// Initializes the JobQueue
        /// </summary>
        /// <param name="stgAccount">The object representing azure storage account</param>
        /// <param name="jobQueueName"></param>
        /// <returns>true if initialized false otherwise</returns>
        public bool Initialize(CloudStorageAccount stgAccount, string jobQueueName)
        {
            try
            {
                this.jobQueueName = jobQueueName;
                base.Initialize(stgAccount);
                CloudQueue jobQueue = queueServiceProxy.GetQueueReference(jobQueueName);
                jobQueue.CreateIfNotExists();
                return true;
            }
            catch (Exception exception)
            {
                AzureLog.Error("initializes the JobQueue fail. ", exception);
            }

            return false;
        }

        /// <summary>
        /// Add a job reference to job queue. The job reference will uniquely identifies
        /// a job in the job table.
        /// </summary>
        /// <param name="jobEntry">string represents the job reference</param>
        /// <param name="initialVisibilityDelay">the time interval before the item become available</param>
        /// <param name="exceptionDetails">object holding details of exception on return</param>
        /// <returns>true if successfully added the job, false otherwise.</returns>
        public bool AddJob(string jobEntry, TimeSpan? initialVisibilityDelay, out ExceptionDetails exceptionDetails)
        {
            exceptionDetails = null;
            try
            {
                base.AddMessage(this.jobQueueName, jobEntry, initialVisibilityDelay);
                return true;
            }
            catch (StorageException stgException)
            {
                exceptionDetails = ExceptionDetails.Create(stgException);
                string partitionKey, rowKey;
                tryExtractKeys(jobEntry, out partitionKey, out rowKey);
                AzureLog.Error("AddJob() fails. partitionKey: " + partitionKey + ", rowKey: " + rowKey, stgException);
            }
            catch (Exception generalException)
            {
                exceptionDetails = ExceptionDetails.Create(generalException);
                string partitionKey, rowKey;
                tryExtractKeys(jobEntry, out partitionKey, out rowKey);
                AzureLog.Error("AddJob() fails. partitionKey: " + partitionKey + ", rowKey: " + rowKey, generalException);
            }

            return false;
        }

        /// <summary>
        /// Asynchronously add a job reference to job queue. The job reference will uniquely identifies
        /// a job in the job table.
        /// </summary>
        /// <param name="jobEntry">string represents the job reference</param>
        /// <param name="initialVisibilityDelay">the time interval before the item become available</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>true if successfully added the job, false otherwise.</returns>
        public async Task AddJobAsync(string jobEntry, TimeSpan? initialVisibilityDelay, CancellationToken cancellationToken)
        {
            await base.AddMessageAsync(this.jobQueueName, jobEntry, initialVisibilityDelay, cancellationToken);
        }

        /// <summary>
        /// Delete a job reference from job queue.
        /// </summary>
        /// <param name="jobMessage">The queue entry to delete.</param>
        /// <param name="exceptionDetails">object holding details of exception on return.</param>
        /// <returns>true if successfully added the job, false otherwise.</returns>
        public bool DeleteJob(CloudQueueMessage jobMessage, out ExceptionDetails exceptionDetails)
        {
            exceptionDetails = null;
            try
            {
                base.DeleteMessage(this.jobQueueName, jobMessage);
                return true;
            }
            catch (StorageException stgException)
            {
                exceptionDetails = ExceptionDetails.Create(stgException);
                string partitionKey, rowKey;
                tryExtractKeys(jobMessage.AsString, out partitionKey, out rowKey);
                AzureLog.Error("DeleteJob() fails. partitionKey: " + partitionKey + ", rowKey: " + rowKey, stgException);
            }
            catch (Exception generalException)
            {
                exceptionDetails = ExceptionDetails.Create(generalException);
                string partitionKey, rowKey;
                tryExtractKeys(jobMessage.AsString, out partitionKey, out rowKey);
                AzureLog.Error("DeleteJob() fails. partitionKey: " + partitionKey + ", rowKey: " + rowKey, generalException);
            }

            return false;
        }

        /// <summary>
        /// Asynchronously delete a job reference from job queue.
        /// </summary>
        /// <param name="jobMessage">The queue entry to delete.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        public async Task DeleteJobAsync(CloudQueueMessage jobMessage, CancellationToken cancellationToken)
        {
            await base.DeleteMessageAsync(this.jobQueueName, jobMessage, cancellationToken);
        }

        /// <summary>
        /// Gets the job references.
        /// </summary>
        /// <param name="messageCount">The message count.</param>
        /// <returns>collection of queue message holding the job references</returns>
        public IEnumerable<CloudQueueMessage> GetJobs(short messageCount, TimeSpan? visibilityTimeout,
            out ExceptionDetails exceptionDetails)
        {
            exceptionDetails = null;
            try
            {
                return base.GetMessages(this.jobQueueName, messageCount, visibilityTimeout);
            }
            catch (StorageException stgException)
            {
                exceptionDetails = ExceptionDetails.Create(stgException);
                AzureLog.Error("JobQueue.GetJobs() fails. ", stgException);
            }
            catch (Exception generalException)
            {
                exceptionDetails = ExceptionDetails.Create(generalException);
                AzureLog.Error("JobQueue.GetJobs() fails. ", generalException);
            }

            return new List<CloudQueueMessage>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobMessage"></param>
        /// <param name="newVisibilityTimeout"></param>
        /// <returns></returns>
        public bool UpdateJobVisibilityTimeOut(CloudQueueMessage jobMessage, TimeSpan newVisibilityTimeout)
        {
            base.UpdateMessageVisibility(this.jobQueueName, jobMessage, newVisibilityTimeout);
            return true;
        }

        /// <summary>
        /// Asynchronously update the visibility timeout of the given queue message.
        /// </summary>
        /// <param name="jobMessage"></param>
        /// <param name="newVisibilityTimeout"></param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns></returns>
        public async Task UpdateJobVisibilityTimeOutAsync(CloudQueueMessage jobMessage, TimeSpan newVisibilityTimeout, CancellationToken cancellationToken)
        {
            await base.UpdateMessageVisibilityAsync(this.jobQueueName, jobMessage, newVisibilityTimeout, cancellationToken);
        }


        /// <summary>
        /// Try to extract the parition key and row key from the job reference for logging.
        /// </summary>
        /// <param name="jobEntry">The job reference.</param>
        /// <param name="paritionKey">holds the partition key on return.</param>
        /// <param name="rowKey">holds the row key on return.</param>
        protected void tryExtractKeys(string jobEntry, out string paritionKey, out string rowKey)
        {
            paritionKey = null;
            rowKey = null;

            try
            {
                var jobQueueEntry = JobQueueEntry.ParseJobEntry(jobEntry);
                paritionKey = jobQueueEntry.PartitionKey;
                rowKey = jobQueueEntry.RowKey;
            }
            catch (Exception)
            { }
        }
    }
}
