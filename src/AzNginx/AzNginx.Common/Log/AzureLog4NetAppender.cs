using log4net.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Common.Log
{
    public class AzureLog4NetAppender : log4net.Appender.AzureTableAppender
    {
        private CloudStorageAccount _account;
        private CloudTableClient _client;
        private CloudTable _table;
        public new string ConnectionString
        {
            set
            {
                base.ConnectionString = AzNginxConfiguration.AzureLog4NetConnectionString;
            }
        }
        public override void ActivateOptions()
        {
            _account = CloudStorageAccount.Parse(base.ConnectionString);
            _client = _account.CreateCloudTableClient();
            _table = _client.GetTableReference(TableName);
            _table.CreateIfNotExists(null, null);
        }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            var grouped = events.GroupBy(evt => evt.LoggerName);
            foreach (var group in grouped)
            {
                foreach (var batch in group.Batch(100))
                {
                    var batchOperation = new TableBatchOperation();
                    foreach (var azureLoggingEvent in batch.Select(GetLogEntity))
                    {
                        batchOperation.Insert(azureLoggingEvent);
                    }
                    _table.ExecuteBatch(batchOperation);
                }
            }
        }

        private ITableEntity GetLogEntity(LoggingEvent @event)
        {
            return new LogEntry(@event, PartitionKeyType);
        }
    }
}
