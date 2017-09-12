using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Common.Exceptions
{
    [Serializable]
    public sealed class NginxResourceDoesntExistException : Exception
    {
        public NginxResourceDoesntExistException() { }
        public NginxResourceDoesntExistException(string message) : base(message) { }
    }
}
