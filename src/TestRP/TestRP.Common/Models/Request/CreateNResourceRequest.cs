using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Common.Models
{
    public class CreateNResourceRequest
    {
        public CreateNResourceRequest()
        {
            tags = new Dictionary<string, string>();
            properties = new CreateNResourceProperties();
        }
        public string location { get; set; }

        public Dictionary<string, string> tags { get; set; }

        public CreateNResourceProperties properties { get; set; }
    }

    public class CreateNResourceProperties
    {
        public string nginxVersion { get; set; }
    }
}
