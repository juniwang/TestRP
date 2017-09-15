using AzNginx.Storage;
using AzNginx.Storage.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Handlers
{
    public interface INginxHandler<TEntity> where TEntity : NginxEntityBase, new()
    {
        void Init(JobTable<TEntity> jobTable, JobQueue jobQueue);

        Task Handle(TEntity entity, CancellationToken cancellationToken);

    }
}
