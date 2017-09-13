using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Storage.Entity
{
    public abstract class EntityBase : TableEntity
    {
        public EntityBase() : base() { }
        public EntityBase(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public int EntityType { get; set; }
        public int RetryCount { get; set; }

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

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);
            EntityPropertyConverter.Serialize(this, results);
            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            EntityPropertyConverter.DeSerialize(this, properties);
        }

        public abstract bool IsInProgress();

        public abstract bool IsFailed();

        public abstract bool IsSucceeded();
    }
}
