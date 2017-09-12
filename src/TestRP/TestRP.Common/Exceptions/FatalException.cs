using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Common.Exceptions
{
    // Throw this exception to indicate we've hit a problem that can't be solved with retry. 
    // This will kill the whole workflow and report back a fatal error to the user.  
    [Serializable]
    public class FatalException : Exception
    {
        public FatalException()
        {
        }

        public FatalException(string message)
            : base(message)
        {
        }
        public FatalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
