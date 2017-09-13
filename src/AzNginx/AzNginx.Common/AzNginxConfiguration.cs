using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Common
{
    /// <summary>
    /// Put the config in a single file for decoupling. In case we put some confidential configs such as 
    /// connection strings to somewhere else, e.g. CloudService settings or KeyVault. Then the file will 
    /// be the only codes to update.
    /// </summary>
    public static class AzNginxConfiguration
    {
        #region Log
        public static string AzureLog4NetConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["Log4Net.ConnectionString"];
            }
        }
        #endregion


        #region RP Rest API service
        public static class Rest
        {
            public static string CsmCertificatesFetchUrl
            {
                get
                {
                    return ConfigurationManager.AppSettings["AzNginx.Web.CsmCertificatesFetchUrl"];
                }
            }
        }
        #endregion


        public static string AzNginxDBConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["AzNginx.DB.ConnectionString"];
            }
        }

        #region Provision
        public static class Provision
        {
            public static string StorageConnectionString
            {
                get
                {
                    return ConfigurationManager.AppSettings["AzNginx.Provision.StorageConnectionString"];
                }
            }

            public static string JobQueueName
            {
                get
                {
                    return ConfigurationManager.AppSettings["AzNginx.Provision.JobQueueName"];
                }
            }

            public static string JobTableName
            {
                get
                {
                    return ConfigurationManager.AppSettings["AzNginx.Provision.JobTableName"];
                }
            }

            public static int MessageVisibilityTimeoutMS
            {
                get
                {
                    return int.Parse(ConfigurationManager.AppSettings["AzNginx.Provision.MessageVisibilityTimeoutMS"]);
                }
            }

            public static int DequeueIntervalMS
            {
                get
                {
                    return int.Parse(ConfigurationManager.AppSettings["AzNginx.Provision.DequeueIntervalMS"]);
                }
            }

            public static short JobQueueConcurrency
            {
                get
                {
                    return short.Parse(ConfigurationManager.AppSettings["AzNginx.Provision.JobQueueConcurrency"]);
                }
            }

            public static short DequeueBatchSize
            {
                get
                {
                    return short.Parse(ConfigurationManager.AppSettings["AzNginx.Provision.DequeueBatchSize"]);
                }
            }
        }

        #endregion
    }
}
