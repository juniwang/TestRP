using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Models
{
    public sealed class NginxProvider
    {
        public static readonly string Default = "Microsoft.Nginx";
    }

    public sealed class NginxResourceTypes
    {
        public static readonly string Nginx = "Nginx";
        public static readonly string NginxWithRP = "Microsoft.Nginx/Nginx";
    }
}
