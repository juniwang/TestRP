using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzNginx.Storage.Entities;
using AzNginx.Storage.Entities.Provision;

namespace AzNginx.Provision.Core.Handlers
{
    public class ACRInitHandler : ProvisionHandlerBase
    {
        public override async Task Handle(NginxProvisionEntity entity, CancellationToken cancellationToken)
        {
            entity.JobState = NginxProvisionState.ACRStatus;
            await UpdateJobAndSchedule(entity, cancellationToken);
        }
    }
}
