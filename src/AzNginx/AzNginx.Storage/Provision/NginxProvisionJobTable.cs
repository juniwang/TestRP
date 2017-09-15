using System;
using AzNginx.Common;
using AzNginx.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;
using AzNginx.Storage.Entities.Provision;

namespace AzNginx.Storage.Provision
{
    public class NginxProvisionJobTable : JobTable<NginxProvisionEntity>, IJobTable<NginxProvisionEntity>
    {
        public override bool IsEntityExists(string partitionKey, string rowKey)
        {
            return LookUpJobEntity(partitionKey, rowKey) != null;
        }

        public NginxProvisionEntity LookUpJobEntity(string partitionKey, string rowKey)
        {
            NginxProvisionEntity jobEntity = null;
            ExceptionDetails exceptionDetails = null;

            GetEntity(partitionKey, rowKey, out jobEntity, out exceptionDetails);
            if (exceptionDetails != null)
            {
                if (exceptionDetails.HttpStatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    throw exceptionDetails.Exception;
                }
            }

            return jobEntity;
        }

        public void ReplaceUpdateJob(NginxProvisionEntity jobEntity)
        {
            this.Merge(this.jobTableName, jobEntity);
        }

    }
}
