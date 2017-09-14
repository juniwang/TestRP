using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzNginx.Provision.Core.Entity;

namespace AzNginx.Provision.Core.Handlers
{
    public class ACRStatusHandler : ProvisionHandlerBase
    {
        public override async Task Handle(NginxProvisionEntity entity, CancellationToken cancellationToken)
        {
            entity.JobState = NginxProvisionState.Succeeded;
            await UpdateJobAndStop(entity, cancellationToken);

        }
    }
}
