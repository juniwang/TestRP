using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Common
{
    public static class ApiVersions
    {
        public const string April2014Preview = "2014-04-01-preview";    // Inital version supported during preview and early months after GA
        public const string April2014 = "2014-04-01";                   // Initial GA version
        public const string April2014Alpha = "2014-04-01-alpha";        // Used internally for pre-flighting features in production (e.g. could route this version to different RP Instance)
        public const string March2015 = "2015-03-01";                   // Add support for customer-provided redis configuration settings
        public const string January2015 = "2015-01-01";                 // Add support for Shoebox V1 self serve of metric definitions
        public const string August2015 = "2015-08-01";                  // Add support for classic virtual networks
        public const string April2016 = "2016-04-01";                   // Add support for non-classic virtual networks
        public const string Feb2017 = "2017-02-01";                     // Add support for external replication servers

        public static bool IsMarch2015OrLater(string apiVersion)
        {
            return String.Compare(apiVersion, March2015, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool IsAugust2015OrLater(string apiVersion)
        {
            return String.Compare(apiVersion, August2015, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool IsApril2016OrLater(string apiVersion)
        {
            return String.Compare(apiVersion, April2016, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool IsFeb2017OrLater(string apiVersion)
        {
            return String.Compare(apiVersion, Feb2017, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
