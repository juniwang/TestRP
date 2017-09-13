using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Storage
{
    /// <summary>
    /// Type representing an entry in the queue
    /// </summary>
    /// <remarks></remarks>
    public class JobQueueEntry
    {
        public string PartitionKey { get; internal set; }
        public string RowKey { get; internal set; }
        public int EntityType { get; internal set; }
        public int RetryCount { get; internal set; }
        public CloudQueueMessage CloudQueueMessage { get; internal set; }
        public long PublisherID
        {
            get
            {
                return int.Parse(PartitionKey);
            }
        }

        private const string JobQueueEntryFormatString = "{0}|{1}|{2}|{3}";

        /// <summary>
        /// Prevents a default instance of the <see cref="JobQueueEntry"/> class from being created.
        /// </summary>
        /// <remarks></remarks>
        private JobQueueEntry() { }

        /// <summary>
        /// Parses the queue entry message
        /// </summary>
        /// <param name="jobQueueMessage">The queue message.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static JobQueueEntry ParseJobEntry(CloudQueueMessage jobQueueMessage)
        {
            JobQueueEntry jobQueueEntry = ParseJobEntry(jobQueueMessage.AsString);
            jobQueueEntry.CloudQueueMessage = jobQueueMessage;
            return jobQueueEntry;
        }

        /// <summary>
        /// Parses the queue entry.
        /// </summary>
        /// <param name="jobEntry">The queue entry.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static JobQueueEntry ParseJobEntry(string jobEntry)
        {
            string[] components = jobEntry.Split('|');
            int task = Convert.ToInt32(components[2], CultureInfo.InvariantCulture);
            int retryCount = Convert.ToInt32(components[3], CultureInfo.InvariantCulture);
            return new JobQueueEntry { PartitionKey = components[0], RowKey = components[1], EntityType = task, RetryCount = retryCount };
        }

        /// <summary>
        /// Formats the query entry.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string formatQueryEntry(string partitionKey, string rowKey, int taskType)
        {
            return string.Format(JobQueueEntry.JobQueueEntryFormatString,
                partitionKey, rowKey, taskType, 0);
        }

        /// <summary>
        /// Formats the query entry.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="retryCount">The retry count</param>
        /// <returns></returns>
        public static string formatQueryEntry(string partitionKey, string rowKey, int taskType, int retryCount)
        {
            return string.Format(JobQueueEntry.JobQueueEntryFormatString,
                partitionKey, rowKey, taskType, retryCount);
        }
    }
}
