using AzNginx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AzNginx.Common;

namespace AzNginx.Web.Controllers
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
    [RoutePrefix("providers/Microsoft.Nginx")]
    public class ActionMetadataController : BaseApiController
    {
        [HttpGet]
        [Route("operations")]
        public virtual ActionMetadataListResponse Get()
        {

            AzureLog.Info("Get ActionMetadataListResponse");

            var actions = new ActionMetadataListResponse
            {
                value = new ActionMetadata[]
                {
                    //ProviderAction("checknameavailability/action", "check name availability", "check name availability"),
                    ProviderAction("register/action", "register provider" ,"register provider"),
                    ProviderAction("unregister/action", "unregister provider" , "unregister provider"),

                    NginxAction("write", "create Nginx" ," create Nginx"),
                    NginxAction("read", "create Nginx" ," create Nginx"),
                    NginxAction("delete", "create Nginx" ," create Nginx"),
                }
            };
            return actions;
        }

        static ActionMetadata ProviderAction(string actionName, string operation, string operationDescription)
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
