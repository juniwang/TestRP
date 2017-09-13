using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzNginx.Common;
using Newtonsoft.Json;
using System.Threading;
using System.Net;

namespace AzNginx.Provision.Core.Storage
{
    /// <summary>
    /// Abstracts operations on a single Windows Azure Table which stores jobs.
    /// </summary>
    public class JobTable : AzureTable
    {
        /// <summary>
        ///  the name of the job table.
        /// </summary>
        protected string jobTableName;

        /// <summary>
        /// Initializes the JobTable
        /// </summary>
        /// <param name="storageAccount">The object representing azure storage account</param>
        /// <param name="jobTableName"></param>
        /// <returns>true if initialized false otherwise</returns>
        public bool Initialize(CloudStorageAccount storageAccount, string jobTableName)
        {
            try
            {
                this.jobTableName = jobTableName;
                base.Initialize(storageAccount);
                CloudTable cloudTable = tableServiceProxy.GetTableReference(jobTableName);
                cloudTable.CreateIfNotExists();
                return true;
            }
            catch (Exception exception)
            {
                AzureLog.Error("JobTable Initialize() fails. ", exception);
            }

            return false;
        }

        /// <summary>
        /// Add a job to table representing job requests.
        /// </summary>
        /// <param name="jobEntity">The job to add.</param>
        /// <param name="exceptionDetails">object holding details of exception on return</param>
        /// <returns>true if successfully added the job, false otherwise.</returns>
        public bool AddJob(ITableEntity jobEntity, out ExceptionDetails exceptionDetails)
        {
            exceptionDetails = null;
            try
            {
                AddEntity(this.jobTableName, jobEntity);
                return true;
            }
            catch (StorageException stgException)
            {
                exceptionDetails = ExceptionDetails.Create(stgException);
                AzureLog.Error(JsonConvert.SerializeObject(jobEntity), stgException);
            }
            catch (Exception generalException)
            {
                exceptionDetails = ExceptionDetails.Create(generalException);
                AzureLog.Error(JsonConvert.SerializeObject(jobEntity), generalException);
            }

            return false;
        }

        /// <summary>
        /// Asynchronously adds a job to table representing job requests.
        /// </summary>
        /// <param name="jobEntity">The job to add.</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>true if successfully added the job, false otherwise.</returns>
        public async Task AddJobAsync(ITableEntity jobEntity, CancellationToken cancellationToken)
        {
            await AddEntityAsync(this.jobTableName, jobEntity, cancellationToken);
        }

        /// <summary>
        /// Replaces the job exists in the job table.
        /// </summary>
        /// <param name="jobEntity">The job to update.</param>
        /// <param name="exceptionDetails">object holding details of exception on return</param>
        /// <returns>true if successfully updated the job, false otherwise.</returns>
        public bool ReplaceUpdateJob(ITableEntity jobEntity, out ExceptionDetails exceptionDetails)
        {
            exceptionDetails = null;
            try
            {
                ReplaceUpdateEntity(this.jobTableName, jobEntity);
                return true;
            }
            catch (StorageException stgException)
            {
                exceptionDetails = ExceptionDetails.Create(stgException);
                AzureLog.Error(JsonConvert.SerializeObject(jobEntity), stgException);
            }
            catch (Exception generalException)
            {
                exceptionDetails = ExceptionDetails.Create(generalException);
                AzureLog.Error(JsonConvert.SerializeObject(jobEntity), generalException);
            }

            return false;
        }

        /// <summary>
        /// Asynchronously replaces the job exists in the job table.
        /// </summary>
        /// <param name="jobEntity">The job to update.</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>true if successfully updated the job, false otherwise.</returns>
        public async Task ReplaceUpdateJobAsync(ITableEntity jobEntity, CancellationToken cancellationToken)
        {
            await ReplaceUpdateEntityAync(this.jobTableName, jobEntity, cancellationToken);
        }

        /// <summary>
        /// Replaces the job exists, or inserts a new job in the job table.
        /// </summary>
        /// <param name="jobEntity">The job to update.</param>
        /// <param name="exceptionDetails">object holding details of exception on return</param>
        /// <returns>true if successfully updated the job, false otherwise.</returns>
        public bool InsertOrReplaceJob(ITableEntity jobEntity, out ExceptionDetails exceptionDetails)
        {
            exceptionDetails = null;
            try
            {
                InsertOrReplaceUpdateEntity(this.jobTableName, jobEntity);
                return true;
            }
            catch (StorageException stgException)
            {
                exceptionDetails = ExceptionDetails.Create(stgException);
                AzureLog.Error(JsonConvert.SerializeObject(jobEntity), stgException);
            }
            catch (Exception generalException)
            {
                exceptionDetails = ExceptionDetails.Create(generalException);
                AzureLog.Error(JsonConvert.SerializeObject(jobEntity), generalException);
            }

            return false;
        }

        /// <summary>
        /// Asynchronously replaces the job exists, or inserts a new job in the job table.
        /// </summary>
        /// <param name="jobEntity">The job to update.</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>true if successfully updated the job, false otherwise.</returns>
        public async Task InsertOrReplaceJobAsync(ITableEntity jobEntity, CancellationToken cancellationToken)
        {
            await InsertOrReplaceUpdateEntityAync(this.jobTableName, jobEntity, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobQueueEntry"></param>
        /// <param name="job"></param>
        /// <param name="exceptionDetails"></param>
        /// <returns></returns>
        public bool GetJob<T>(JobQueueEntry jobQueueEntry, out T job,
            out ExceptionDetails exceptionDetails) where T : TableEntity
        {
            return GetJob<T>(jobQueueEntry.PartitionKey,
                jobQueueEntry.RowKey,
                out job,
                out exceptionDetails);
        }

        /// <summary>
        /// Get a job of type T identifed by the given row and partition key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <param name="job">The job retrieved on return</param>
        /// <param name="exceptionDetails">object holding details of exception on return</param>
        /// <returns>true if successfully retrived the job, false otherwise.</returns>
        public bool GetJob<T>(string partitionKey, string rowKey, out T job,
            out ExceptionDetails exceptionDetails) where T : TableEntity
        {
            job = null;
            exceptionDetails = null;

            try
            {
                GetEntity<T>(this.jobTableName, partitionKey, rowKey, out job);
                if (job == null)
                {
                    exceptionDetails = ExceptionDetails.Create(new Exception(string.Format("Table entry not found, got null. PartitionKey:{0} RowKey:{1}", partitionKey, rowKey)));
                    exceptionDetails.HttpStatusCode = HttpStatusCode.NotFound;
                    return false;
                }

                return true;
            }
            catch (StorageException stgException)
            {
                exceptionDetails = ExceptionDetails.Create(stgException);
                AzureLog.Error("GetJob() fails, partitionKey: " + partitionKey + ", rowKey: " + rowKey, stgException);
            }
            catch (Exception generalException)
            {
                exceptionDetails = ExceptionDetails.Create(generalException);
                AzureLog.Error("GetJob() fails, partitionKey: " + partitionKey + ", rowKey: " + rowKey, generalException);
            }

            return false;
        }

        public virtual bool IsJobExists(string partitionKey, string rowKey)
        {
            throw new NotImplementedException();
        }
    }
}
