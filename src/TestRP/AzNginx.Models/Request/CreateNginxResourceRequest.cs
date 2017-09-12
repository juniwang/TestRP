using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Models
{
    public class CreateNginxResourceRequest
    {
        public CreateNginxResourceRequest()
        {
            tags = new Dictionary<string, string>();
            properties = new CreateNginxResourceRequestProperties();
        }
        public string location { get; set; }

        public Dictionary<string, string> tags { get; set; }

        public CreateNginxResourceRequestProperties properties { get; set; }
    }

    public class CreateNginxResourceRequestProperties
    {
        public string nginxVersion { get; set; }
    }
}
