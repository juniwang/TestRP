using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Common.Models
{
    public class NginxResourceListResponse
    {
        public NginxResourceResponse[]  values { get; set; }

        /// <summary>
        /// The nextLink field is expected to point to the URL the client should use to fetch the next page
        /// *If we return a paged response.* Otherwise it can be omitted from the response.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string nextLink { get; set; }
    }
}
