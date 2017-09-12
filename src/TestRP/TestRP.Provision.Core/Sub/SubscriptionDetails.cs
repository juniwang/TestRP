using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Provision.Core.Sub
{
    public class SubscriptionDetails
    {
        // The name of the subscription. 
        public string Name { get; set; }

        // Subscription Usage Details. 
        public SubscriptionUsage Usage { get; set; }
    }
}
