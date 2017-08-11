using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace TestRP.Kernel
{
    public interface ICertificateValidator
    {
        void Validate(X509Certificate2 certificate, Action<string> logAction);
    }
}
