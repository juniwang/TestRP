﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRP.Common.Models;

namespace TestRP.Common
{
    public static class NDataExtensions
    {
        public static NginxResourceResponse ToResponse(this NResource resource)
        {
            return new NginxResourceResponse
            {
                id = resource.Id,
                location = resource.Location,
                name = resource.Name,
                tags = resource.Tags,
                type = "Microsoft.Nginx/Nginx",
                properties = new NginxResponseProperties
                {
                    enabled = resource.Enabled,
                    nginxVersion = resource.properties.NginxVersion,
                }
            };
        }

        public static NResource SearchBySpec(this NData data, ResourceSpec spec)
        {
            return data.Resources.FirstOrDefault(
                p => p.SubscriptionId.Equals(spec.subscriptionId, StringComparison.OrdinalIgnoreCase)
                && p.ResourceGroup.Equals(spec.resourceGroupName, StringComparison.OrdinalIgnoreCase)
                && p.Name.Equals(spec.resourceName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
