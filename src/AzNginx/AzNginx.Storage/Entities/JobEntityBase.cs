using AzNginx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Storage.Entities
{
    public abstract class JobEntityBase : NginxEntityBase
    {
        public JobEntityBase() { }
        public JobEntityBase(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public int EntityType { get; set; }
        public int RetryCount { get; set; }

        [ConvertableEntityProperty]
        public OperationId OperationId { get; set; }

        private JobQueueEntry _jobQueueEntry;
        public void SetJobQueueEntry(JobQueueEntry jobQueueEntry)
        {
            _jobQueueEntry = jobQueueEntry;
        }

        public JobQueueEntry GetJobQueueEntry()
        {
            if (_jobQueueEntry == null)
            {
                throw new InvalidOperationException("_jobQueueEntry needs to be set");
            }

            return _jobQueueEntry;
        }


        public abstract bool IsInProgress();

        public abstract bool IsFailed();

        public abstract bool IsSucceeded();
    }
}
