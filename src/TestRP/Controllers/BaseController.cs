using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace TestRP.Controllers
{
    public class BaseController : ApiController
    {
        public string BaseAddress
        {
            get
            {
                var builder = new UriBuilder(ControllerContext.Request.RequestUri.AbsoluteUri)
                {
                    Path = ControllerContext.RequestContext.Url.Route("BaseAddress", new object()),
                    Query = null,
                };
                return builder.Uri.AbsoluteUri;
            }
        }
    }
}
