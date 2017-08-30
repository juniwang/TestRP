using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Common.Models
{
    // Data POST-ed to the nameAvailability action
    public class NameAvailabilityRequest
    {
        // always "Microsoft.Cache/redis",
        public string type { get; set; }

        // e.g. "my-cache-name-here"
        public string name { get; set; }
    }
}
