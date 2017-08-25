using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Common.Models
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
