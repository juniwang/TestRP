using AzNginx.Common;
using AzNginx.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Storage.Entities
{
    public abstract class NginxEntityBase : TableEntity
    {
        public NginxEntityBase() : base() { }
        public NginxEntityBase(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);
            EntityPropertyConverter.Serialize(this, results);
            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            EntityPropertyConverter.DeSerialize(this, properties);
        }

    }
}
