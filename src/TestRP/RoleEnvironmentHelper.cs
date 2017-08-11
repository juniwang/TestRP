using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace TestRP
{
    public static class RoleEnvironmentHelper
    {
        public static bool AzureIsAvailable
        {
            get
            {
                try
                {
                    return RoleEnvironment.IsAvailable;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool AzureIsEmulated
        {
            get
            {
                try
                {
                    return RoleEnvironment.IsEmulated;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool IsRunningInRealAzure()
        {
            if (AzureIsAvailable)
            {
                if (!AzureIsEmulated)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
