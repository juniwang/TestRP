using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Azure
{
    /// <summary>
    /// The resource to acquire access token by certain authority
    /// </summary>
    public class AzureResource
    {
        static readonly string ARMResource = "https://management.azure.com/";
        static readonly string ServiceManagementResource = "https://management.core.windows.net/";
        static readonly string GraphResource = "https://graph.windows.net/";

        // Following resources are for Azure.
        public static readonly AzureResource ARM = new AzureResource { Resource = ARMResource };
        public static readonly AzureResource ServiceManagement = new AzureResource { Resource = ServiceManagementResource };
        public static readonly AzureResource Graph = new AzureResource { Resource = GraphResource };

        public string Resource { get; set; }

    }
}
