using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzNginx.Storage
{
    /// <summary>
    /// Type that abstract operation aganist windows azure storage tables in a given storage account.
    /// </summary>
    public class AzureTable<TEntity> where TEntity : ITableEntity, new()
    {
        #region Constants
        /// <summary>
        /// The retry interval between for table storage retry operations
        /// </summary>
        static readonly int TableStorageRetryInterval = 5;

        /// <summary>
        /// The retry count for table storage operations
        /// </summary>
        static readonly int TableStorageRetryCount = 5;
        #endregion

        /// <summary>
        /// client to make request aganist azure table storage.
        /// </summary>
        protected CloudTableClient tableServiceProxy;

        /// <summary>
        /// Initializes the AzureTable.
        /// </summary>
        /// <param name="storageAccount">The object representing azure storage account</param>
        public void Initialize(CloudStorageAccount storageAccount)
        {
            tableServiceProxy = storageAccount.CreateCloudTableClient();
            tableServiceProxy.DefaultRequestOptions.RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.LinearRetry(
                TimeSpan.FromSeconds(TableStorageRetryInterval), TableStorageRetryCount);
        }

        /// <summary>
        /// Add an entity to the given table.
        /// </summary>
        /// <param name="tableName">The name of the storage table exists under the storage account
        /// with which this AzureTable is configured.</param>
        /// <param name="entity">The entity to add.</param>
        public TableResult Insert(string tableName, TEntity entity)
        {
            TableOperation insertOperation = TableOperation.Insert(entity);
            CloudTable cloudTable = tableServiceProxy.GetTableReference(tableName);
            TableResult intsertResult = cloudTable.Execute(insertOperation);
            return intsertResult;
        }

        /// <summary>
        /// Asynchronously add an entity to the given table.
        /// </summary>
        /// <param name="tableName">The name of the storage table exists under the storage account
        /// with which this AzureTable is configured.</param>
        /// <param name="entity">The entity to add.</param>
        public Task<TableResult> InsertAsync(string tableName, TEntity entity, CancellationToken cancellationToken)
        {
            TableOperation insertOperation = TableOperation.Insert(entity);
            CloudTable cloudTable = tableServiceProxy.GetTableReference(tableName);
            return cloudTable.ExecuteAsync(insertOperation, cancellationToken);
        }

        /// <summary>
        /// Replaces an entity in the given table.
        /// </summary>
        /// <param name="tableName">The name of the storage table exists under the storage account
        /// with which this AzureTable is configured.</param>
        /// <param name="entity">The job to update.</param>
        public TableResult Merge(string tableName, TEntity entity)
        {
            TableOperation mergeOperation = TableOperation.Merge(entity);
            CloudTable jobRequestTable = tableServiceProxy.GetTableReference(tableName);
            TableResult mergeResult = jobRequestTable.Execute(mergeOperation);
            return mergeResult;
        }

        /// <summary>
        /// Asynchronously replaces an entity in the given table.
        /// </summary>
        /// <param name="tableName">The name of the storage table exists under the storage account
        /// with which this AzureTable is configured.</param>
        /// <param name="entity">The job to update.</param>
        public Task<TableResult> MergeAsync(string tableName, TEntity entity, CancellationToken cancellationToken)
        {
            TableOperation mergeOperation = TableOperation.Merge(entity);
            CloudTable jobRequestTable = tableServiceProxy.GetTableReference(tableName);
            return jobRequestTable.ExecuteAsync(mergeOperation, cancellationToken);
        }
        /// <summary>
        /// Replaces an existing entity, or insert a new entity in the given table.
        /// </summary>
        /// <param name="tableName">The name of the storage table exists under the storage account
        /// with which this AzureTable is configured.</param>
        /// <param name="entity">The job to update.</param>
        public TableResult InsertOrReplace(string tableName, TEntity entity)
        {
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(entity);
            CloudTable jobRequestTable = tableServiceProxy.GetTableReference(tableName);
            TableResult mergeResult = jobRequestTable.Execute(insertOrReplaceOperation);
            return mergeResult;
        }

        /// <summary>
        /// Asynchronously replaces an existing entity, or insert a new entity in the given table.
        /// </summary>
        /// <param name="tableName">The name of the storage table exists under the storage account
        /// with which this AzureTable is configured.</param>
        /// <param name="entity">The job to update.</param>
        public Task<TableResult> InsertOrReplaceAsync(string tableName, TEntity entity, CancellationToken cancellationToken)
        {
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(entity);
            CloudTable jobRequestTable = tableServiceProxy.GetTableReference(tableName);
            return jobRequestTable.ExecuteAsync(insertOrReplaceOperation, cancellationToken);
        }

        /// <summary>
        /// delete an entity identified by the given keys from the given table.
        /// </summary>
        /// <param name="tableName">The name of the storage table exists under the storage account
        /// with which this AzureTable is configured.</param>
        /// <param name="partitionKey">The partitionkey.</param>
        /// <param name="rowKey">The row.</param>
        public TableResult Delete(string tableName, string partitionKey, string rowKey)
        {
            TableOperation deleteOperation = TableOperation.Delete(new TableEntity(partitionKey, rowKey) { ETag = "*" });
            CloudTable cloudTable = tableServiceProxy.GetTableReference(tableName);
            TableResult deleteResult = cloudTable.Execute(deleteOperation);
            return deleteResult;
        }

        /// <summary>
        /// Asynchronously delete an entity identified by the given keys from the given table.
        /// </summary>
        /// <param name="tableName">The name of the storage table exists under the storage account
        /// with which this AzureTable is configured.</param>
        /// <param name="partitionKey">The partitionkey.</param>
        /// <param name="rowKey">The row.</param>
        public Task<TableResult> DeleteAsync(string tableName, string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            TableOperation deleteOperation = TableOperation.Delete(new TableEntity(partitionKey, rowKey) { ETag = "*" });
            CloudTable cloudTable = tableServiceProxy.GetTableReference(tableName);
            return cloudTable.ExecuteAsync(deleteOperation);
        }

        /// <summary>
        /// Get an entity of type T identifed by the given row and partition key from the given table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">The name of the storage table exists under the storage account
        /// with which this AzureTable is configured.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <param name="entity">The job entity retrieved on return</param>
        public void Retrieve<T>(string tableName, string partitionKey, string rowKey,
            out T entity) where T : TableEntity
        {
            entity = null;
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            CloudTable jobRequestTable = tableServiceProxy.GetTableReference(tableName);
            TableResult retrieveResult = jobRequestTable.Execute(retrieveOperation);
            entity = (T)retrieveResult.Result;
        }

        /// <summary>
        /// Get an TableQuery that can be modified using LINQ
        /// </summary>
        /// <typeparam name="T">The entity type of the table</typeparam>
        /// <param name="tableName">the name of the table</param>
        /// <returns></returns>
        public TableQuery<TEntity> CreateQuery(string tableName)
        {
            CloudTable cloudTable = tableServiceProxy.GetTableReference(tableName);
            return cloudTable.CreateQuery<TEntity>();
        }
    }
}
