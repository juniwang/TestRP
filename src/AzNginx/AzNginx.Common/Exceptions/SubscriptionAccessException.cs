using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Common.Exceptions
{
    // Throw this exception to indicate we've hit a problem that can't access subscription in the pool. 
    [Serializable]
    public class SubscriptionAccessException : Exception
    {
        public SubscriptionAccessException()
        {
        }

        public SubscriptionAccessException(string message)
            : base(message)
        {
        }
        public SubscriptionAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
