using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using AzNginx.Common.Exceptions;
using AzNginx.Provision.Core.Entity;

namespace AzNginx.Provision.Core.Storage.Nginx
{
    public class NginxProvisionJobTable : JobTable, IJobTable
    {
        public T LookUpJobEntity<T>(string partitionKey, string rowKey) where T : TableEntity
        {
            T jobEntity = null;
            ExceptionDetails exceptionDetails = null;

            GetJob<T>(partitionKey, rowKey, out jobEntity, out exceptionDetails);
            if (exceptionDetails != null)
            {
                if (exceptionDetails.HttpStatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    throw exceptionDetails.Exception;
                }
            }

            return jobEntity;
        }

        void IJobTable.ReplaceUpdateJob(ITableEntity jobEntity)
        {
            ReplaceUpdateEntity(this.jobTableName, jobEntity);
        }

        public override bool IsJobExists(string partitionKey, string rowKey)
        {
            return LookUpJobEntity<NginxProvisionEntity>(partitionKey, rowKey) != null;
        }
    }
}
