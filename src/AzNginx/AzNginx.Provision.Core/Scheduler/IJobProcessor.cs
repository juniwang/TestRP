using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Scheduler
{
    /// <summary>
    /// Defines a contract that all JobProcessors specific to service needs to be implemented. 
    /// Common.Mover.Scheduler.MoverJobProcessor is an example for job processor specific to
    /// Mover service.
    /// </summary>
    public interface IJobProcessor
    {
        /// <summary>
        /// Call-back for handling a job
        /// </summary>
        /// <param name="jobQueueMessage"></param>
        /// <param name="exceptionDetails"></param>
        /// <returns></returns>
        bool OnJob(CloudQueueMessage jobQueueMessage, out ExceptionDetails exceptionDetails);
    }
}
