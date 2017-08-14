using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TestRP.Common;
using TestRP.Common.Models;

namespace TestRP.Web.Controllers
{
    public class ActionMetadataController : BaseApiController
    {
        [AcceptVerbs("GET")]
        public virtual ActionMetadataListResponse Get()
        {

            AzureLog.Info("Get ActionMetadataListResponse");

            var actions = new ActionMetadataListResponse
            {
                value = new ActionMetadata[]
                {
                    CacheProviderAction("checknameavailability/action", "check name availability", "check name availability"),
                    CacheProviderAction("register/action", "register provider" ,"register provider"),
                    CacheProviderAction("unregister/action", "unregister provider" , "unregister provider"),

                    //RedisAction("write", Resources.OperationWriteRedis, Resources.OperationWriteRedisLong),
                    //RedisAction("read", Resources.OperationReadRedis, Resources.OperationReadRedisLong),
                    //RedisAction("delete", Resources.OperationDeleteRedis, Resources.OperationDeleteRedisLong),
                    //RedisAction("listKeys/action", Resources.OperationListKeysRedis, Resources.OperationListKeysRedisLong),
                    //RedisAction("regenerateKey/action", Resources.OperationRegenerateKeysRedis, Resources.OperationRegenerateKeysRedisLong),
                    //RedisAction("import/action", Resources.OperationImport, Resources.OperationImportLong),
                    //RedisAction("export/action", Resources.OperationExport, Resources.OperationExportLong),
                    //RedisAction("forceReboot/action", Resources.OperationForceReboot, Resources.OperationForceReboot),
                    //RedisAction("stop/action", Resources.OperationStop, Resources.OperationStop),
                    //RedisAction("start/action", Resources.OperationStart, Resources.OperationStart),

                    ////RedisAction("stopScript/action", Resources.OperationStopScript, Resources.OperationStopScript),

                    //RedisMetricDefinitionsAction("read", Resources.OperationReadMetricDefinitionsRedis, Resources.OperationReadMetricDefinitionsRedisLong, metricDefinitions),

                    //RedisUpgradeNotificationAction("read", Resources.OperationListUpgradeNotificationsRedis, Resources.OperationListUpgradeNotificationsRedisLong),

                    //RedisPatchSchedulesAction("read", Resources.OperationReadPatchSchedulesRedis, Resources.OperationReadPatchSchedulesRedisLong),
                    //RedisPatchSchedulesAction("write", Resources.OperationWritePatchSchedulesRedis, Resources.OperationWritePatchSchedulesRedisLong),
                    //RedisPatchSchedulesAction("delete", Resources.OperationDeletePatchSchedulesRedis, Resources.OperationDeletePatchSchedulesRedisLong),

                    //RedisFirewallRulesAction("read", Resources.OperationReadFirewallRulesRedis, Resources.OperationReadFirewallRulesRedisLong),
                    //RedisFirewallRulesAction("write", Resources.OperationWriteFirewallRulesRedis, Resources.OperationWriteFirewallRulesRedisLong),
                    //RedisFirewallRulesAction("delete", Resources.OperationDeleteFirewallRulesRedis, Resources.OperationDeleteFirewallRulesRedisLong),

                    //RedisLinkedServersAction("read", Resources.OperationReadLinkedServersRedis, Resources.OperationReadLinkedServersRedisLong),
                    //RedisLinkedServersAction("write", Resources.OperationWriteLinkedServersRedis, Resources.OperationWriteLinkedServersRedisLong),
                    //RedisLinkedServersAction("delete", Resources.OperationDeleteLinkedServersRedis, Resources.OperationDeleteLinkedServersRedisLong),
                }
            };
            return actions;
        }

        static ActionMetadata CacheProviderAction(string actionName, string operation, string operationDescription)
        {
            return new ActionMetadata
            {
                name = "Microsoft.Nginx/" + actionName,
                display = new ActionMetadataDisplay
                {
                    provider = "Microsoft Nginx",
                    operation = operation,
                    description = operationDescription,
                },
                properties = new ActionMetadataProperties()
            };
        }

        static ActionMetadata RedisAction(string actionName, string operation, string operationDescription)
        {
            return new ActionMetadata
            {
                name = "Microsoft.Nginx/nginx/" + actionName,
                display = new ActionMetadataDisplay
                {
                    provider = "Microsoft Nginx",
                    resource = "Nginx",
                    operation = operation,
                    description = operationDescription,
                },
                properties = new ActionMetadataProperties()
            };
        }
    }
}
