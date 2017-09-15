using AzNginx.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Scheduler
{
    public interface IJobScheduler<TEntity> where TEntity : TableEntity, new()
    {
        IJobTable<TEntity> JobTable { get; }
    }
}
