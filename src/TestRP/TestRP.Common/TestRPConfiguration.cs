using System;
using System.Collections.Generic;
using System.Configuration;
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
                return  ConfigurationManager.AppSettings["Log4NetConnectionString"];
            }
        }

        public static string CsmCertificatesFetchUrl {
            get {
                return ConfigurationManager.AppSettings["CsmCertificatesFetchUrl"];
            }
        }

        public static string NginxRPDbConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["NginxRPDbConnectionString"];
            }
        }
    }
}
