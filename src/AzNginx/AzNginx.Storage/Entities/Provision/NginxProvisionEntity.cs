using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Storage.Entities.Provision
{
    public class NginxProvisionEntity : JobEntityBase
    {
        public NginxProvisionEntity() { }
        public NginxProvisionEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        /// <summary>
        /// the RowKey of the relevent NginxResourceEntity. For easier query in provision process.
        /// </summary>
        public string ResourceEntityRowKey { get; set; }

        public DateTime ScheduleTime { get; set; }

        [ConvertableEntityProperty]
        public NginxProvisionState JobState { get; set; }

        public string UserSubscription { get; set; }

        public string ResourceGroup { get; set; }

        public string ExceptionLogAsJsonString { get; set; }
        public int ErrorCode { get; set; }

        public override bool IsInProgress()
        {
            return !IsFailed() && !IsSucceeded();
        }

        public override bool IsFailed()
        {
            return JobState == NginxProvisionState.Failed;
        }

        public override bool IsSucceeded()
        {
            return JobState == NginxProvisionState.Succeeded;
        }
    }
}
