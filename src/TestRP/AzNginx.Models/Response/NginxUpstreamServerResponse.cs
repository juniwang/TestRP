using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Models
{
    public class NginxUpstreamServerResponse
    {
        public string id { get; set; }
        public string host { get; set; }
        public string weight { get; set; }
        public string state { get; set; }
        public string updated { get; set; }
    }
}
