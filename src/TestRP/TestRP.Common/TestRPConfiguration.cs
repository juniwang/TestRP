using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Common
{
    public static class TestRPConfiguration
    {
        public static string AzureLog4NetConnectionString
        {
            get
            {
                return  CloudConfigurationManager.GetSetting("Log4NetConnectionString");
            }
        }

        public static string CsmCertificatesFetchUrl {
            get {
                return CloudConfigurationManager.GetSetting("CsmCertificatesFetchUrl");
            }
        }
    }
}
