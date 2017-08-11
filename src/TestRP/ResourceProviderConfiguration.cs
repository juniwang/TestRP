using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TestRP
{
    public class ResourceProviderConfiguration
    {
        public string CsmCertificatesFetchUrl {
            get {
                return ConfigurationManager.AppSettings["CsmCertificatesFetchUrl"];
            }
        }
    }
}
