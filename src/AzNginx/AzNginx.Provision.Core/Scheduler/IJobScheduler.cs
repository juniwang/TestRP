using AzNginx.Provision.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Scheduler
{
    public interface IJobScheduler
    {
        IJobTable JobTable { get; }
    }
}
