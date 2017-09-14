using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Entity
{
    /// <summary>
    /// Don't change the int value of the enum, that may cause unexpected issue since the integer will be stored in azure table
    /// </summary>
    public enum NginxProvisionState
    {
        ACRInit = 0,
        ACRStatus = 1,
        KubernetesInit = 2,
        KubernetesStatus = 3,
        VNetInit = 4,
        VNetStatus = 5,
        DNSInit = 6,
        DNSStatus = 7,
        Succeeded = 20,
        Failed = 21,
    }
}
