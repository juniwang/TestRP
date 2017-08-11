using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Owin.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace TestRP
{
    class Program
    {
        static string baseAddress = "http://localhost:10225";

        static void Main(string[] args)
        {
            X509Certificate2 mySslCert = null;
            IContainer container = Startup.CreateContainer();
            string[] trustedCsmCerts = AppSettingsHelper.ReadStringArray("TrustedCsmClientCertThumbprints");
            if (mySslCert != null)
            {
                trustedCsmCerts = trustedCsmCerts.Concat(new[] {
                    mySslCert.Thumbprint
                }).ToArray();
            }

            using (StartServer(container, trustedCsmCerts))
            {
                Console.ReadLine();
            }
        }

        public static IDisposable StartServer(IContainer container, string[] trustedCsmCerts)
        {
            try
            {
                return WebApp.Start(baseAddress, app => Startup.Configure(app, container, trustedCsmCerts: trustedCsmCerts));
            }
            catch
            {
                throw;
            }
        }
    }
}
