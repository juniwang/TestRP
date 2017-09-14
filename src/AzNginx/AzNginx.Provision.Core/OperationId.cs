using AzNginx.Provision.Core.Storage;
using AzNginx.Provision.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core
{
    /// <summary>
    /// Represents a type consists of tracking infomations
    /// </summary>
    public class OperationId
    {
        public string ClientRequestId { get; set; }
        public string CorrelationRequestId { get; set; }
        public string RequestId { get; set; }
        public string RoutingId { get; set; }
        public string ClientTenantId { get; set; }
        public string ClientPrincipalId { get; set; }
        public string ClientObjectId { get; set; }
    }
}
