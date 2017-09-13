using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Storage.Entity
{
    public class NginxProvisionEntity : EntityBase
    {
        public NginxProvisionEntity() { }
        public NginxProvisionEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public DateTime ScheduleTime { get; set; }

        [ConvertableEntityProperty]
        public NginxProvisionState JobState { get; set; }

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
