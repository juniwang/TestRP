using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TestRP.Common;
using TestRP.Common.Models;

namespace TestRP.Web.Controllers
{
    /// <summary>
    /// See RPAPIv2: "Exposing Available Operations (for client discovery)"
    /// 
    /// Basically we need to give metadata to describe
    /// - ALL CUD Operations
    /// - ALL POST Operations
    /// that act on our resources via our REST API,
    /// so that role based security can show nice descriptions for it in the portal, powershell, etc.
    /// </summary>
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

                    //NginxAction("write", Resources.OperationWriteRedis, Resources.OperationWriteRedisLong),
                    //NginxAction("read", Resources.OperationReadRedis, Resources.OperationReadRedisLong),
                    //NginxAction("delete", Resources.OperationDeleteRedis, Resources.OperationDeleteRedisLong),
                    //NginxAction("listKeys/action", Resources.OperationListKeysRedis, Resources.OperationListKeysRedisLong),
                    //NginxAction("regenerateKey/action", Resources.OperationRegenerateKeysRedis, Resources.OperationRegenerateKeysRedisLong),
                    //NginxAction("import/action", Resources.OperationImport, Resources.OperationImportLong),
                    //NginxAction("export/action", Resources.OperationExport, Resources.OperationExportLong),
                    //NginxAction("forceReboot/action", Resources.OperationForceReboot, Resources.OperationForceReboot),
                    //NginxAction("stop/action", Resources.OperationStop, Resources.OperationStop),
                    //NginxAction("start/action", Resources.OperationStart, Resources.OperationStart),

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

        static ActionMetadata NginxAction(string actionName, string operation, string operationDescription)
        {
            return new ActionMetadata
            {
                name = "Microsoft.Nginx/Nginx/" + actionName,
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
