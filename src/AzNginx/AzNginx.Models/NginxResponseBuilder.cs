using AzNginx.Storage.Entities.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Models
{
    public class NginxResponseBuilder
    {
        public NginxResourceResponse MakeResponse(NginxResourceEntity resource, string apiVersion)
        {
            // return different response accoring to the apiVersion
            return new NginxResourceResponse
            {
                id = resource.ResourceId,
                location = resource.Location,
                name = resource.ResourceName,
                tags = resource.Tags,
                type = NginxResourceTypes.NginxWithRP,
                properties = new NginxResponseProperties
                {
                    nginxVersion = resource.NginxVersion,
                    status = resource.Status.ToString()
                }
            };
        }

        public NginxResourceListResponse MakeListResponse(IEnumerable<NginxResourceEntity> resources, string apiVersion)
        {
            return new NginxResourceListResponse
            {
                values = resources.Select(r => MakeResponse(r, apiVersion)).ToArray()
            };
        }
    }
}
