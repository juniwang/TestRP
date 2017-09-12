using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Models
{
    public class NginxResourceResponse
    {
        public NginxResourceResponse()
        {
            tags = new Dictionary<string, string>();
            properties = new NginxResponseProperties();
        }

        // something like this: "/subscriptions/{id}/resourcegroups/{resourceGroupName}/providers/Microsoft.Nginx/Nginx/{name}",
        public string id { get; set; }

        // location: Azure GEO region: e.g. West US | East US | North Central US | South Central US | West Europe | North Europe | East Asia | Southeast Asia | etc. 
        // An RP should use this to create the resource in the appropriate region.  The geo region of a resource never changes after it is created.
        public string location { get; set; }

        // name of the resource
        public string name { get; set; }

        // type of resource - e.g. "Microsoft.Cache/Redis"
        public string type { get; set; }

        // A list of key value pairs that describe the resource. These tags can be used in viewing and grouping this resource (across resource groups). 
        // The resource provider is expected to store these tags with the resource. 
        // max tag count: 10
        // max key length: 128 
        // max value length: 256
        // value is NULLABLE
        // CSM: Tag-names created by Azure will have a prefixes of “microsoft”, “azure”, or “windows”.  Users will not be able to create tags with one of these prefixes. 
        public IDictionary<string, string> tags { get; set; }

        public NginxResponseProperties properties { get; set; }
    }

    public class NginxResponseProperties
    {
        public NginxResponseProperties()
        {
            servers = new NginxUpstreamServerResponse[0];
        }

        public string status { get; set; }

        public string nginxVersion { get; set; }

        public NginxUpstreamServerResponse[] servers { get; set; }
    }
}
