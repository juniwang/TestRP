using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzNginx.Common.Exceptions;

namespace AzNginx.Models
{
    /// ResourceName should be suitable as a hostname/DNS fragment and 
    /// contain only ascii lower case letters and numbers
    public static class ResourceNamePolicy
    {
        // http://en.wikipedia.org/wiki/Hostname 
        //  length [1, 63]
        //  case in-sensitive
        //  letter, numbers & hyphen allowed
        //  hyphen can't be the first or last character
        static Regex validNameRegex = new Regex(@"\A([a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9]|[a-zA-Z0-9])\z");
        public static readonly int MinNameLength = 1;
        public static readonly int MaxNameLength = 63;

        public static bool IsResourceNameValid(string resourceName)
        {
            return resourceName != null &&
                resourceName.Length >= MinNameLength &&
                resourceName.Length <= MaxNameLength &&
                validNameRegex.IsMatch(resourceName, 0) &&
                !resourceName.Contains("--");
        }

        public static void ValidateResourceName(ResourceSpec resource)
        {
            if (!IsResourceNameValid(resource.resourceName))
            {
                throw new InvalidResourceNameException(resource.resourceName);
            }
            if (resource.childResourceName != null)
            {
                throw new InvalidResourceNameException(resource.childResourceName);
            }
        }
    }
}
