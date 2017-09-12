using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Models
{
    public class NginxUpstreamServerListResponse
    {
        public NginxUpstreamServerListResponse()
        {
            Servers = new NginxUpstreamServerResponse[0];
        }

        public NginxUpstreamServerResponse[] Servers { get; set; }
    }
}
