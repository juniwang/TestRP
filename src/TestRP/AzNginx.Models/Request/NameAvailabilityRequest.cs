using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Models
{
    // Data POST-ed to the nameAvailability action
    public class NameAvailabilityRequest
    {
        // always "Microsoft.Nginx/Nginx",
        public string type { get; set; }

        // e.g. "my-nginx-name-here"
        public string name { get; set; }
    }
}
