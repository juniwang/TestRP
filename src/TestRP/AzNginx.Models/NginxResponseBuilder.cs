using AzNginx.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Models
{
    public class NginxResponseBuilder
    {
        public NginxResourceResponse MakeResponse(NginxResource resource, string apiVersion)
        {
            // return different response accoring to the apiVersion
            return new NginxResourceResponse
            {
                id = resource.ResourceId,
                location = resource.Location,
                name = resource.ResourceName,
                tags = resource.Tags,
                type = NginxResourceTypes.Nginx,
                properties = new NginxResponseProperties
                {
                    nginxVersion = resource.NginxVersion,
                    status = resource.Status.ToString()
                }
            };
        }

        public NginxResourceListResponse MakeListResponse(IEnumerable<NginxResource> resources, string apiVersion)
        {
            return new NginxResourceListResponse
            {
                values = resources.Select(r => MakeResponse(r, apiVersion)).ToArray()
            };
        }
    }
}
