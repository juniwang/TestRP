using Microsoft.Azure;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzNginx.Common;
using AzNginx.Common.Exceptions;
using AzNginx.Provision.Core.Azure;
using Microsoft.Rest;
using Microsoft.WindowsAzure.Management;
using Microsoft.Azure.Management.Resources;

namespace AzNginx.Provision.Core.Sub
{
    public interface IAzureSubscription
    {
        string SubscriptionId { get; }
        SubscriptionCloudCredentials GetCredentials(AzureResource resource);
        AzureEnvironment AzureEnvironment { get; }
    }

    public static class IAzureSubscriptionExtensions
    {
        public static ManagementClient CreateManagementClient(this IAzureSubscription sub)
        {
            return new ManagementClient(
                sub.GetCredentials(AzureResource.ServiceManagement),
                sub.AzureEnvironment.ManagementEndpointUri);
        }

        public static ResourceManagementClient CreateResourceManagementClient(this IAzureSubscription sub)
        {
            return new ResourceManagementClient(
                sub.GetCredentials(AzureResource.ARM),
                sub.AzureEnvironment.ResourceManagerEndpointUri);
        }
    }

    public class AzureSubscription : IAzureSubscription
    {
        SubscripionConfig _subscriptionConfig;
        AzureEnvironment _azureEnvironment;
        string _authority;

        public AzureSubscription(SubscripionConfig subscripionConfig)
        {
            _subscriptionConfig = subscripionConfig;
            _azureEnvironment = subscripionConfig.Environment();
            _authority = _azureEnvironment.ActiveDirectoryEndpoint + subscripionConfig.TenantId ?? "common" + "/";
        }

        public override string ToString()
        {
            return _subscriptionConfig.SubscriptionId;
        }

        public string SubscriptionId
        {
            get { return _subscriptionConfig.SubscriptionId; }
        }

        public SubscriptionCloudCredentials GetCredentials(AzureResource resource)
        {
            if (resource == null)
            {
                string message = string.Format("cannot create azure credential due for sub {0} to invalid resource",
                    _subscriptionConfig.SubscriptionId);
                AzureLog.Error(message);
                throw new SubscriptionAccessException(message);
            }

            try
            {
                var authenticationResult = AcquireAccessTokenAysnc(resource);
                return new TokenCloudCredentials(authenticationResult.AccessToken);
            }
            catch (Exception e)
            {
                AzureLog.Error("Cannot acquire token from AAD for sub: " + SubscriptionId, e);
                throw new SubscriptionAccessException("cannot acquire token from AAD", e);
            }
        }

        public AzureEnvironment AzureEnvironment
        {
            get { return _azureEnvironment; }
        }

        private AuthenticationResult AcquireAccessTokenAysnc(AzureResource resource)
        {
            AuthenticationContext authContext = new AuthenticationContext(_authority, false);
            var credentials = new ClientCredential(_subscriptionConfig.ClientId, _subscriptionConfig.ClientSecret);
            return authContext.AcquireTokenSilentAsync(resource.Resource, credentials, UserIdentifier.AnyUser).Result;
        }
    }

}
