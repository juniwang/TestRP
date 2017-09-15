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

        #region Storage
        public static class Storage
        {
            public static string ConnectionString
            {
                get
                {
                    return ConfigurationManager.AppSettings["AzNginx.Storage.ConnectionString"];
                }
            }

            public static string ProvisionJobQueueName
            {
                get
                {
                    return ConfigurationManager.AppSettings["AzNginx.Storage.ProvisionJobQueueName"];
                }
            }

            public static string ProvisionJobTableName
            {
                get
                {
                    return ConfigurationManager.AppSettings["AzNginx.Storage.ProvisionJobTableName"];
                }
            }

            public static string ResourceTableName
            {
                get
                {
                    return ConfigurationManager.AppSettings["AzNginx.Storage.ResourceTableName"];
                }
            }

            public static int MessageVisibilityTimeoutMS
            {
                get
                {
                    return int.Parse(ConfigurationManager.AppSettings["AzNginx.Storage.MessageVisibilityTimeoutMS"]);
                }
            }

            public static int DequeueIntervalMS
            {
                get
                {
                    return int.Parse(ConfigurationManager.AppSettings["AzNginx.Storage.DequeueIntervalMS"]);
                }
            }

            public static short JobQueueConcurrency
            {
                get
                {
                    return short.Parse(ConfigurationManager.AppSettings["AzNginx.Storage.JobQueueConcurrency"]);
                }
            }

            public static short DequeueBatchSize
            {
                get
                {
                    return short.Parse(ConfigurationManager.AppSettings["AzNginx.Storage.DequeueBatchSize"]);
                }
            }
        }

        #endregion
    }
}
