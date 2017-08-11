using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace TestRP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "actionMetadata",
                routeTemplate: "providers/Microsoft.Cache/operations",
                defaults: new { controller = "ActionMetadata", action = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "nameAvailability",
                routeTemplate: "subscriptions/{subscriptionId}/providers/Microsoft.Cache/checkNameAvailability",
                defaults: new { controller = "Names", action = "CheckNameAvailability" });

            config.Routes.MapHttpRoute(
                name: "RedisConfigDefinition",
                routeTemplate: "location/{locationName}/subscriptions/{subscriptionId}/providers/Microsoft.Cache/RedisConfigDefinition",
                defaults: new { controller = "RedisConfigDefinition", action = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "RedisEventSourceInfo",
                routeTemplate: "eventsourceinfo",
                defaults: new { controller = "NotificationService", action = RouteParameter.Optional });

            // Routes for:
            // (GET 'redis') (listing all in resource group)
            // (PUT/GET/DELETE/PATCH 'redis/foo')
            config.Routes.MapHttpRoute(
                name: "RedisResourceRoute",
                routeTemplate: "location/{locationName}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Cache/Redis/{resourceName}",
                defaults: new
                {
                    controller = "RedisResource",
                    action = RouteParameter.Optional,
                    resourceProvider = "Microsoft.Cache",
                    resourceType = "redis",
                    resourceName = RouteParameter.Optional
                });

            // Routes for:
            // (GET 'redis/foo/patchSchedules') (listing all schedules in a redis cache)
            // (PUT/GET/PATCH/DELETE 'redis/foo/patchSchedules/default')
            config.Routes.MapHttpRoute(
                name: "RedisResourcePatchSchedulesRoute",
                routeTemplate: "location/{locationName}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Cache/Redis/{resourceName}/patchSchedules/{childResourceName}",
                defaults: new
                {
                    controller = "RedisResourcePatchSchedules",
                    action = RouteParameter.Optional,
                    resourceProvider = "Microsoft.Cache",
                    resourceType = "redis",
                    childResourceType = "patchSchedules",
                    childResourceName = RouteParameter.Optional
                });

            // Routes for:
            // (GET 'redis/foo/firewallRules') (listing all firewall rules in a redis cache)
            // (PUT/GET/PATCH/DELETE 'redis/foo/firewallRules/{childResourceName}')
            config.Routes.MapHttpRoute(
                name: "RedisResourceFirewallRulesRoute",
                routeTemplate: "location/{locationName}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Cache/Redis/{resourceName}/firewallRules/{childResourceName}",
                defaults: new
                {
                    controller = "RedisResourceFirewallRules",
                    action = RouteParameter.Optional,
                    resourceProvider = "Microsoft.Cache",
                    resourceType = "redis",
                    childResourceType = "firewallRules",
                    childResourceName = RouteParameter.Optional
                });

            // Routes for:
            // (GET 'redis/foo/linkedservers') (listing all linkedservers for a given cache)
            // (PUT 'redis/foo/linkedservers') (setup external replication a cache)
            config.Routes.MapHttpRoute(
                name: "RedisResourceLinkedServerRoute",
                routeTemplate: "location/{locationName}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Cache/Redis/{resourceName}/linkedservers/{childResourceName}",
                defaults: new
                {
                    controller = "RedisResourceLinkedServer",
                    resourceProvider = "Microsoft.Cache",
                    resourceType = "redis",
                    childResourceType = "linkedServers",
                    childResourceName = RouteParameter.Optional
                });

            // Routes for external replication calls that are not exposed to ARM:
            // (POST actions: setup/validate/handshake/unlink)
            // (GET actions: status)
            config.Routes.MapHttpRoute(
                name: "RedisResourceLinkedServerInternalRoute",
                routeTemplate: "location/{locationName}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Cache/Redis/{resourceName}/linkedservers/{childResourceName}/{action}",
                defaults: new
                {
                    controller = "RedisResourceLinkedServerInternal",
                    resourceProvider = "Microsoft.Cache",
                    resourceType = "redis",
                    childResourceType = "linkedServers",
                    childResourceName = RouteParameter.Optional
                });

            // (GET 'redis/foo/listUpgradeNotifications') (listing all upgrade notifications in a redis cache)
            config.Routes.MapHttpRoute(
                name: "RedisResourceListUpgradeNotificationsRoute",
                routeTemplate: "location/{locationName}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Cache/Redis/{resourceName}/listUpgradeNotifications",
                defaults: new
                {
                    controller = "RedisResourceListUpgradeNotifications",
                    action = RouteParameter.Optional,
                    resourceProvider = "Microsoft.Cache",
                    resourceType = "redis",
                });

            // Routes for:
            // (POST 'redis/foo/{action}')
            // (actions: notify/listKeys/regenerateKey/import/export/forceReboot/stop/start)
            config.Routes.MapHttpRoute(
                name: "RedisResourceActionRoute",
                routeTemplate: "location/{locationName}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Cache/Redis/{resourceName}/{action}",
                defaults: new
                {
                    controller = "RedisResource",
                    resourceProvider = "Microsoft.Cache",
                    resourceType = "redis",
                });

            // (POST 'virtualNetworks/foo/providers/Microsoft.Cache/{notify}') (Microsoft.ClassicNetwork)
            config.Routes.MapHttpRoute(
                name: "ClassicVirtualNetworkResourceRoute",
                routeTemplate: "location/{locationName}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ClassicNetwork/virtualNetworks/{resourceName}/providers/Microsoft.Cache/{action}",
                defaults: new
                {
                    controller = "ClassicVirtualNetworksResource",
                    resourceProvider = "Microsoft.ClassicNetwork",
                    resourceType = "virtualNetworks"
                });

            // (POST 'virtualNetworks/foo/providers/Microsoft.Cache/{notify}') (Microsoft.Network)
            config.Routes.MapHttpRoute(
                name: "VirtualNetworkResourceRoute",
                routeTemplate: "location/{locationName}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Network/virtualNetworks/{resourceName}/providers/Microsoft.Cache/{action}",
                defaults: new
                {
                    controller = "VirtualNetworksResource",
                    resourceProvider = "Microsoft.Network",
                    resourceType = "virtualNetworks"
                });

            config.Routes.MapHttpRoute(
                name: "enumerateResources",
                routeTemplate: "location/{locationName}/subscriptions/{subscriptionId}/providers/Microsoft.Cache/{resourceType}",
                defaults: new
                {
                    controller = "EnumerateResources",
                    action = RouteParameter.Optional,
                    resourceProvider = "Microsoft.Cache"
                });

            config.Routes.MapHttpRoute(
                name: "validateMoveResources",
                routeTemplate:
                "location/{locationName}/subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/validateMoveResources",
                defaults: new
                {
                    controller = "Subscriptions",
                    action = "validateMoveResources"
                });

            config.Routes.MapHttpRoute(
                name: "moveResources",
                routeTemplate:
                "location/{locationName}/subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/moveResources",
                defaults: new
                {
                    controller = "Subscriptions",
                    action = "moveResources"
                });

            config.Routes.MapHttpRoute(
                name: "moveResourcesStatusCheck",
                routeTemplate:
                "location/{locationName}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Cache/operations/{operationId}",
                defaults: new
                {
                    controller = "Subscriptions",
                    action = RouteParameter.Optional,
                    resourceProvider = "Microsoft.Cache"
                });

            config.Routes.MapHttpRoute(
                name: "subscriptions",
                routeTemplate:
                "location/{locationName}/subscriptions/{subscriptionId}",
                defaults: new { controller = "Subscriptions", action = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "operationResults",
                routeTemplate:
                  "location/{rpName}/subscriptions/{subscriptionId}/providers/Microsoft.Cache/locations/{locationName}/operationresults/{operationId}",
                defaults: new
                {
                    controller = "Subscriptions",
                    action = RouteParameter.Optional,
                    resourceProvider = "Microsoft.Cache"
                });

            // CSM does notify on proxy-only endpoints as well. CSM doesn't have any hook do exclude those calls.
            // This registration is route to a dummy no-op implementation to keep CSM happy.
            config.Routes.MapHttpRoute(
                name: "proxyonlysubscriptions",
                routeTemplate:
                "subscriptions/{subscriptionId}",
                defaults: new { controller = "Subscriptions", action = RouteParameter.Optional });

            //// Dummy route so that controllers can look up the application base address
            config.Routes.MapHttpRoute(
                name: "BaseAddress",
                routeTemplate: "");
        }

    }
}
