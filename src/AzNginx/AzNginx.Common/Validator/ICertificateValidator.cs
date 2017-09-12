using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Common.Validator
{
    public interface ICertificateValidator
    {
        void Validate(X509Certificate2 certificate, Action<string> logAction);
    }
}
