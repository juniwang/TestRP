using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Common.Exceptions
{
    [Serializable]
    sealed class NonExactResourceException : Exception
    {
        public string ParamName { get; set; }

        public NonExactResourceException(string resourceSpecProperty)
            : base("The operation must specify a unique target resource.")
        {
            ParamName = resourceSpecProperty;
        }

        public NonExactResourceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ParamName = info.GetString("ParamName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ParamName", ParamName);
        }
    }
}
