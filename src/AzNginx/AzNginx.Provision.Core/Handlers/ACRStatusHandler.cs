using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzNginx.Provision.Core.Storage.Entity;

namespace AzNginx.Provision.Core.Handlers
{
    public class ACRStatusHandler : ProvisionHandlerBase
    {
        public override Task Handle(NginxProvisionEntity entity, CancellationToken cancellationToken)
        {
            entity.JobState = NginxProvisionState.Succeeded;
            return UpdateJobAndStop(entity, cancellationToken);
        }
    }
}
