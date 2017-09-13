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
    public class AzureQueue
    {
        /// <summary>
        /// The retry count for table storage operations
        /// </summary>
        public static readonly int QueueStorageRetryCount = 5;

        /// <summary>
        /// The retry interval between for table storage retry operations
        /// </summary>
        public static readonly int QueueStorageRetryInterval = 5;

        /// <summary>
        /// client to make request aganist azure queue service.
        /// </summary>
        public CloudQueueClient queueServiceProxy { get; protected set; }

        /// <summary>
        /// Initializes the AzureQueue.
        /// </summary>
        /// <param name="storageAccount">The object representing azure storage account</param>
        public void Initialize(CloudStorageAccount storageAccount)
        {
            queueServiceProxy = storageAccount.CreateCloudQueueClient();
            queueServiceProxy.DefaultRequestOptions.RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.LinearRetry(
                TimeSpan.FromSeconds(QueueStorageRetryInterval), QueueStorageRetryCount);
        }

        /// <summary>
        /// Insert a string message to the queue with the given name with a visibility timeout.
        /// </summary>
        /// <param name="queueName">The name of the storage queue exists under the storage account
        /// with which this AzureQueue is configured.</param>
        /// <param name="message">The message to insert to queue.</param>
        /// <param name="initialVisibilityDelay">the time interval before the item become available.</param>
        public void AddMessage(string queueName, string message, TimeSpan? initialVisibilityDelay)
        {
            CloudQueue cloudQueue = queueServiceProxy.GetQueueReference(queueName);
            cloudQueue.AddMessage(new CloudQueueMessage(message), null, initialVisibilityDelay);
        }

        /// <summary>
        /// Asynchronously insert a string message to the queue with the given name with a visibility timeout.
        /// </summary>
        /// <param name="queueName">The name of the storage queue exists under the storage account
        /// with which this AzureQueue is configured.</param>
        /// <param name="message">The message to insert to queue.</param>
        /// <param name="initialVisibilityDelay">the time interval before the item become available.</param>
        public Task AddMessageAsync(string queueName, string message, TimeSpan? initialVisibilityDelay, CancellationToken cancellationToken)
        {
            CloudQueue cloudQueue = queueServiceProxy.GetQueueReference(queueName);
            return cloudQueue.AddMessageAsync(new CloudQueueMessage(message), null, initialVisibilityDelay, null, null, cancellationToken);
        }

        /// <summary>
        /// Deletes a message from the queue with the given name.
        /// </summary>
        /// <param name="queueName">The name of the storage queue exists under the storage account
        /// with which this AzureQueue is configured.</param>
        /// <param name="queueMessage"></param>
        public void DeleteMessage(string queueName, CloudQueueMessage queueMessage)
        {
            CloudQueue cloudQueue = queueServiceProxy.GetQueueReference(queueName);
            cloudQueue.DeleteMessage(queueMessage);
        }

        /// <summary>
        /// Asynchronously deletes a message from the queue with the given name.
        /// </summary>
        /// <param name="queueName">The name of the storage queue exists under the storage account
        /// with which this AzureQueue is configured.</param>
        /// <param name="queueMessage"></param>
        public Task DeleteMessageAsync(string queueName, CloudQueueMessage queueMessage, CancellationToken cancellationToken)
        {
            CloudQueue cloudQueue = queueServiceProxy.GetQueueReference(queueName);
            return cloudQueue.DeleteMessageAsync(queueMessage, cancellationToken);
        }

        /// <summary>
        /// Updates the visibility timeout of a message in a queue with given name.
        /// </summary>
        /// <param name="queueName">The name of the storage queue exists under the storage 
        /// account with which this AzureQueue is configured.</param>
        /// <param name="queueMessage">The queue message whose visibility timeout to be 
        /// updated.</param>
        /// <param name="visibilityTimeout">The new visibility timeout.</param>
        public void UpdateMessageVisibility(string queueName, CloudQueueMessage queueMessage,
            TimeSpan visibilityTimeout)
        {
            CloudQueue cloudQueue = queueServiceProxy.GetQueueReference(queueName);
            cloudQueue.UpdateMessage(queueMessage, visibilityTimeout, MessageUpdateFields.Visibility);
        }

        /// <summary>
        /// Asynchronously updates the visibility timeout of a message in a queue with given name.
        /// </summary>
        /// <param name="queueName">The name of the storage queue exists under the storage 
        /// account with which this AzureQueue is configured.</param>
        /// <param name="queueMessage">The queue message whose visibility timeout to be 
        /// updated.</param>
        /// <param name="visibilityTimeout">The new visibility timeout.</param>
        public Task UpdateMessageVisibilityAsync(string queueName, CloudQueueMessage queueMessage,
            TimeSpan visibilityTimeout, CancellationToken cancellationToken)
        {
            CloudQueue cloudQueue = queueServiceProxy.GetQueueReference(queueName);
            return cloudQueue.UpdateMessageAsync(queueMessage, visibilityTimeout, MessageUpdateFields.Visibility, cancellationToken);
        }

        /// <summary>
        /// Retrieve enumerable list of 'messageCount' mesaages from the queue with given name.
        /// </summary>
        /// <param name="queueName">The name of the storage queue exists under the storage account
        /// with which this AzureQueue is configured.</param>
        /// <param name="messageCount">The maximum number of messages to retrieve.</param>
        /// <param name="visibilityTimeout">the time interval before the retrieved messages become 
        /// available again.</param>
        /// <returns></returns>
        public IEnumerable<CloudQueueMessage> GetMessages(string queueName, short messageCount,
            TimeSpan? visibilityTimeout)
        {
            if (messageCount == 0)
            {
                return new List<CloudQueueMessage>();
            }

            CloudQueue jobQueue = queueServiceProxy.GetQueueReference(queueName);
            return jobQueue.GetMessages(messageCount, visibilityTimeout);
        }
    }
}
