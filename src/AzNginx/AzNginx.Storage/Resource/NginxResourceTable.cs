using AzNginx.Common;
using AzNginx.Storage.Entities;
using AzNginx.Storage.Entities.Resource;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Storage.Resource
{
    public class NginxResourceTable : JobTable<NginxResourceEntity>
    {
        public NginxResourceTable()
        {
            var storage = CloudStorageAccount.Parse(AzNginxConfiguration.Storage.ConnectionString);
            var resourceTable = AzNginxConfiguration.Storage.ResourceTableName;
            Initialize(storage, resourceTable);
        }
    }
}
