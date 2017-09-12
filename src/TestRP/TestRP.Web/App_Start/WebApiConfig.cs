using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using TestRP.Web.Handlers;

namespace TestRP.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.MessageHandlers.Add(new RequiredHeadersHandler());
            ////config.MessageHandlers.Add(new CertificateValidatorHandler(certificateValidator, certificateRetriever));
            //config.MessageHandlers.Add(new DoubleSlashSubscriptionsHandler());
            //config.MessageHandlers.Add(new XHttpMethodHandler());
            //config.MessageHandlers.Add(new XMsClientRequestIdHandler());
            //config.MessageHandlers.Add(new AcceptLanguageHeaderHandler());
            //config.MessageHandlers.Add(new LogHttpHandler());

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
