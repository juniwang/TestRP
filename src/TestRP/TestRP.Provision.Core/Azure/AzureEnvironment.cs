using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Provision.Core.Azure
{
    public class AzureEnvironment
    {
        private Dictionary<string, string> endpoints;

        public AzureEnvironment(Dictionary<string, string> endpoints)
        {
            this.endpoints = endpoints;
        }

        #region known Environments
        public static AzureEnvironment Azure = new AzureEnvironment(new Dictionary<string, string>()
        {
            { "portalUrl", "http://go.microsoft.com/fwlink/?LinkId=254433" },
            { "publishingProfileUrl", "http://go.microsoft.com/fwlink/?LinkId=254432"},
            { "managementEndpointUrl", "https://management.core.windows.net"},
            { "resourceManagerEndpointUrl", "https://management.azure.com/"},
            { "sqlManagementEndpointUrl", "https://management.core.windows.net:8443/"},
            { "sqlServerHostnameSuffix", ".database.windows.net"},
            { "galleryEndpointUrl", "https://gallery.azure.com/"},
            { "activeDirectoryEndpointUrl", "https://login.microsoftonline.com/"},
            { "activeDirectoryResourceId", "https://management.core.windows.net/"},
            { "activeDirectoryGraphResourceId", "https://graph.windows.net/"},
            { "activeDirectoryGraphApiVersion", "2013-04-05"},
            { "storageEndpointSuffix", ".core.windows.net"},
            { "keyVaultDnsSuffix", ".vault.azure.net"},
            { "azureDataLakeStoreFileSystemEndpointSuffix", "azuredatalakestore.net"},
            { "azureDataLakeAnalyticsCatalogAndJobEndpointSuffix", "azuredatalakeanalytics.net" }
        });

        public static AzureEnvironment AzureChina = new AzureEnvironment(new Dictionary<string, string>()
        { { "portalUrl", "http://go.microsoft.com/fwlink/?LinkId=301902"},
            { "publishingProfileUrl", "http://go.microsoft.com/fwlink/?LinkID=301774"},
            { "managementEndpointUrl", "https://management.core.chinacloudapi.cn"},
            { "resourceManagerEndpointUrl", "https://management.chinacloudapi.cn"},
            { "sqlManagementEndpointUrl", "https://management.core.chinacloudapi.cn:8443/"},
            { "sqlServerHostnameSuffix", ".database.chinacloudapi.cn"},
            { "galleryEndpointUrl", "https://gallery.chinacloudapi.cn/"},
            { "activeDirectoryEndpointUrl", "https://login.chinacloudapi.cn/"},
            { "activeDirectoryResourceId", "https://management.core.chinacloudapi.cn/"},
            { "activeDirectoryGraphResourceId", "https://graph.chinacloudapi.cn/"},
            { "activeDirectoryGraphApiVersion", "2013-04-05"},
            { "storageEndpointSuffix", ".core.chinacloudapi.cn"},
            { "keyVaultDnsSuffix", ".vault.azure.cn"},
            { "azureDataLakeStoreFileSystemEndpointSuffix", "N/A"},
            { "azureDataLakeAnalyticsCatalogAndJobEndpointSuffix", "N/A"},
        });

        public static AzureEnvironment AzureUSGovernment = new AzureEnvironment(new Dictionary<string, string>()
        { { "portalUrl", "https://manage.windowsazure.us"},
            { "publishingProfileUrl", "https://manage.windowsazure.us/publishsettings/index"},
            { "managementEndpointUrl", "https://management.core.usgovcloudapi.net"},
            { "resourceManagerEndpointUrl", "https://management.usgovcloudapi.net"},
            { "sqlManagementEndpointUrl", "https://management.core.usgovcloudapi.net:8443/"},
            { "sqlServerHostnameSuffix", ".database.usgovcloudapi.net"},
            { "galleryEndpointUrl", "https://gallery.usgovcloudapi.net/"},
            { "activeDirectoryEndpointUrl", "https://login-us.microsoftonline.com/"},
            { "activeDirectoryResourceId", "https://management.core.usgovcloudapi.net/"},
            { "activeDirectoryGraphResourceId", "https://graph.windows.net/"},
            { "activeDirectoryGraphApiVersion", "2013-04-05"},
            { "storageEndpointSuffix", ".core.usgovcloudapi.net"},
            { "keyVaultDnsSuffix", ".vault.usgovcloudapi.net"},
            { "azureDataLakeStoreFileSystemEndpointSuffix", "N/A"},
            { "azureDataLakeAnalyticsCatalogAndJobEndpointSuffix", "N/A"}
        });

        public static AzureEnvironment AzureGermany = new AzureEnvironment(new Dictionary<string, string>()
        { { "portalUrl", "http://portal.microsoftazure.de/"},
            { "publishingProfileUrl", "https://manage.microsoftazure.de/publishsettings/index"},
            { "managementEndpointUrl", "https://management.core.cloudapi.de"},
            { "resourceManagerEndpointUrl", "https://management.microsoftazure.de"},
            { "sqlManagementEndpointUrl", "https://management.core.cloudapi.de:8443/"},
            { "sqlServerHostnameSuffix", ".database.cloudapi.de"},
            { "galleryEndpointUrl", "https://gallery.cloudapi.de/"},
            { "activeDirectoryEndpointUrl", "https://login.microsoftonline.de/"},
            { "activeDirectoryResourceId", "https://management.core.cloudapi.de/"},
            { "activeDirectoryGraphResourceId", "https://graph.cloudapi.de/"},
            { "activeDirectoryGraphApiVersion", "2013-04-05"},
            { "storageEndpointSuffix", ".core.cloudapi.de"},
            { "keyVaultDnsSuffix", ".vault.microsoftazure.de"},
            { "azureDataLakeStoreFileSystemEndpointSuffix", "N/A"},
            { "azureDataLakeAnalyticsCatalogAndJobEndpointSuffix", "N/"}
});
        #endregion

        public Dictionary<string, string> Endpoints
        {
            get
            {
                return endpoints;
            }
        }

        #region Properties

        public string Portal
        {
            get
            {
                return endpoints["portalUrl"];
            }
        }

        public string PublishingProfile
        {
            get
            {
                return endpoints["publishingProfileUrl"];
            }
        }

        public string ManagementEndpoint
        {
            get
            {
                return endpoints["managementEndpointUrl"];
            }
        }

        public Uri ManagementEndpointUri
        {
            get
            {
                return new Uri(endpoints["managementEndpointUrl"]);
            }
        }

        public string ResourceManagerEndpoint
        {
            get
            {
                return endpoints["resourceManagerEndpointUrl"];
            }
        }

        public Uri ResourceManagerEndpointUri
        {
            get
            {
                return new Uri(endpoints["resourceManagerEndpointUrl"]);
            }
        }

        public string SqlManagementEndpoint
        {
            get
            {
                return endpoints["sqlManagementEndpointUrl"];
            }
        }

        public string SqlServerHostnameSuffix
        {
            get
            {
                return endpoints["sqlServerHostnameSuffix"];
            }
        }

        public string ActiveDirectoryEndpoint
        {
            get
            {
                return endpoints["activeDirectoryEndpointUrl"];
            }
        }

        public string ActiveDirectoryResourceId
        {
            get
            {
                return endpoints["activeDirectoryResourceId"];
            }
        }

        public string GalleryEndpoint
        {
            get
            {
                return endpoints["galleryEndpointUrl"];
            }
        }

        public string GraphEndpoint
        {
            get
            {
                return endpoints["activeDirectoryGraphResourceId"];
            }
        }

        public string ActiveDirectoryGraphApiVersion
        {
            get
            {
                return endpoints["activeDirectoryGraphApiVersion"];
            }
        }

        public string StorageEndpointSuffix
        {
            get
            {
                return endpoints["storageEndpointSuffix"];
            }
        }

        public string KeyVaultDnsSuffix
        {
            get
            {
                return endpoints["keyVaultDnsSuffix"];
            }
        }

        public string AzureDataLakeStoreFileSystemEndpointSuffix
        {
            get
            {
                return endpoints["azureDataLakeStoreFileSystemEndpointSuffix"];
            }
        }

        public string AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix
        {
            get
            {
                return endpoints["azureDataLakeAnalyticsCatalogAndJobEndpointSuffix"];
            }
        }
        #endregion
    }
}

