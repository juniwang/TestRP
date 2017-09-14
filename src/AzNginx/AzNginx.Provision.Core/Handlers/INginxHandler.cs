using AzNginx.Provision.Core.Storage;
using AzNginx.Provision.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Handlers
{
    public interface INginxHandler<TEntity> where TEntity : EntityBase
    {
        void Init(JobTable jobTable, JobQueue jobQueue);

        Task Handle(TEntity entity, CancellationToken cancellationToken);

    }
}
