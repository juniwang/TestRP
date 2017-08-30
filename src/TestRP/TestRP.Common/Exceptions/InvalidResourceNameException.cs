using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Common.Exceptions
{
    [Serializable]
    sealed class InvalidResourceNameException : Exception
    {
        public string Name { get; set; }

        public InvalidResourceNameException(string name)
        {
            Name = name;
        }
    }
}
