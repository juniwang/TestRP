using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzNginx.Provision.Core.Azure;

namespace AzNginx.Provision.Core.Sub
{
    public class SubscriptionConfigs
    {
        public SubscriptionConfigs()
        {
            Subscriptions = new SubscripionConfig[0];
        }

        public SubscripionConfig[] Subscriptions { get; set; }
    }

    public class SubscripionConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubscriptionId { get; set; }
        public CloudEnvironment CloudEnvironment { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
    }

    public enum CloudEnvironment
    {
        Azure,
        AzureChina,
        AzureUSGovernment,
        AzureGermany,
    }

    public static class SubscripionConfigExtensions
    {
        public static AzureEnvironment Environment(this SubscripionConfig config)
        {
            switch (config.CloudEnvironment)
            {
                case CloudEnvironment.AzureChina:
                    return AzureEnvironment.AzureChina;
                case CloudEnvironment.AzureUSGovernment:
                    return AzureEnvironment.AzureUSGovernment;
                case CloudEnvironment.AzureGermany:
                    return AzureEnvironment.AzureGermany;
                default:
                    return AzureEnvironment.Azure;
            }
        }
    }
}
