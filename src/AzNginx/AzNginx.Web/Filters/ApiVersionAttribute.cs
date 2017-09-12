using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AzNginx.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ApiVersionAttribute : ActionFilterAttribute
    {
        public string[] SupportedVersions { get; set; }

        public ApiVersionAttribute(params string[] versions)
        {
            SupportedVersions = versions.Select(e => e.ToLowerInvariant()).ToArray();
        }

        public override void OnActionExecuting(HttpActionContext context)
        {
            //string value = UrlParsing.GetQueryParameter(context.Request, "api-version").ToLowerInvariant();
            //var r = this.SupportedVersions.SingleOrDefault(e => e.Equals(value));
            //if (r == null)
            //{
            //    ResourceProviderEventSource.Instance.LogEvent(
            //        EventLevel.Verbose,
            //        context.Request.AsRequestData(),
            //        "ApiVersionAttribute.OnActionExecuting - Rejecting request due to missing/incorrect api-version query string argument");

            //    context.Response = context.Request.CreateErrorResponse(
            //        HttpStatusCode.BadRequest,
            //        Resources.Error_ApiVersionRequired);
            //}
        }
    }
}