using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Storage
{
    public interface IJobTable<TEntity> where TEntity : TableEntity
    {
        /// <summary>
        /// Look for the job entity with given parition key and row key of type T
        /// and return the job entity if found, if such job entity not exists 
        /// return null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="partitionKey">The parition key of the job entity</param>
        /// <param name="rowKey">The row key of the job entity</param>
        /// <returns></returns>
        TEntity LookUpJobEntity(string partitionKey, string rowKey);

        /// <summary>
        ///  Replace or update the job identified by 'jobEntity' in the underlying job
        ///  table.
        /// </summary>
        /// <param name="jobEntity"></param>
        /// <param name="exceptionDetails"></param>
        /// <returns></returns>
        void ReplaceUpdateJob(TEntity jobEntity);
    }
}
