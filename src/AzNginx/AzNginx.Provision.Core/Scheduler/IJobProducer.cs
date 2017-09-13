using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Scheduler
{
    /// <summary>
    /// Producer represents a collection of queues that produces the jobs.
    /// </summary>
    public interface IJobProducer
    {
        IList<IProducerQueueInfo> ProducerQueueInfos { get; }
    }
}
