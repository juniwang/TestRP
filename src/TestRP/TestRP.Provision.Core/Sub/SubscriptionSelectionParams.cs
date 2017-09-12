using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Provision.Core.Sub
{
    public class SubscriptionSelectionParams
    {
        public string VmSize { get; set; }
        public int InstanceCount { get; set; }
        public double CoreToStorageRatio { get; set; }
    }
}
