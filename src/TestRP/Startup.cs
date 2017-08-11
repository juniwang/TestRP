using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Autofac;
using TestRP.Kernel;
using System.Web.Http;
using System.Web.Http.Tracing;
using TestRP.Handlers;
using Autofac.Integration.WebApi;
using System.Net.Http.Formatting;
using TestRP.Controllers;

namespace TestRP
{
    public class Startup
    {
        internal class AutoFacServiceProvider : IServiceProvider
        {
            IComponentContext _container;

            public AutoFacServiceProvider(IComponentContext ctx)
            {
                _container = ctx;
            }

            public object GetService(Type serviceType)
            {
                return _container.Resolve(serviceType);
            }
        }

        public static void Configure(
            IAppBuilder appBuilder,
            IContainer container, string[] trustedCsmCerts, bool isUnitTest = false)
        {
            ICertificateValidator csmCertificateValidator = null;
            if (!isUnitTest)
            {
                string csmCertificatesFetchUrl = container.Resolve<ResourceProviderConfiguration>().CsmCertificatesFetchUrl;
                csmCertificateValidator = new DownloadedListCertificateValidator(csmCertificatesFetchUrl, trustedCsmCerts);
            }
            var csmAction = GetAppBuilderAction(container, csmCertificateValidator, isUnitTest);
            csmAction(appBuilder);
        }

        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.Register<IServiceProvider>(ctx => new AutoFacServiceProvider(ctx.Resolve<IComponentContext>()));

            // start monitoring settings blob
            //StartMonitoringResourceProviderSettings(builder);

            RegisterServices(builder);

            IContainer container = builder.Build();
            return container;
        }

        public static void RegisterServices(ContainerBuilder builder)
        {
            //RegisterDataDependencies(builder);
            RegisterDefaultDependencies(builder);
            //ProvisionServiceStartup.RegisterServices(builder);
            RegisterControllers(builder);
        }

        //public to allow unit tests to call this method directly.  Product code should call RegisterServices
        public static void RegisterControllers(ContainerBuilder builder)
        {
            //builder.RegisterType<ActionMetadataController>().PropertiesAutowired();
            builder.RegisterType<SubscriptionsController>().PropertiesAutowired();
            //builder.RegisterType<RedisResourceController>().PropertiesAutowired();
            //builder.RegisterType<RedisResourcePatchSchedulesController>().PropertiesAutowired();
            //builder.RegisterType<RedisResourceFirewallRulesController>().PropertiesAutowired();
            //builder.RegisterType<RedisResourceListUpgradeNotificationsController>().PropertiesAutowired();
            //builder.RegisterType<RedisResourceLinkedServerController>().PropertiesAutowired();
            //builder.RegisterType<RedisResourceLinkedServerInternalController>().PropertiesAutowired();
            //builder.RegisterType<NotificationServiceController>().PropertiesAutowired();
            //builder.RegisterType<NamesController>().PropertiesAutowired();
            //builder.RegisterType<EnumerateResourcesController>().PropertiesAutowired();
            //builder.RegisterType<ClassicVirtualNetworksResourceController>().PropertiesAutowired();
            //builder.RegisterType<VirtualNetworksResourceController>().PropertiesAutowired();
        }

        public static void RegisterDefaultDependencies(ContainerBuilder builder)
        {
            builder.RegisterInstance(new CertificateRetriever()).As<ICertificateRetriever>();
            builder.RegisterInstance(new ResourceProviderConfiguration());
        }

        private static Action<Owin.IAppBuilder> GetAppBuilderAction(IContainer container, ICertificateValidator certificateValidator, bool isUnitTest = false)
        {
            Action<Owin.IAppBuilder> act = (app) =>
            {
                HttpConfiguration config = CreateHttpConfiguration(container, isUnitTest, certificateValidator);

                // Construct OWIN pipeline
                //app.Use<OwinRequestResponseLogger>();
                app.UseWebApi(config);
                config.EnsureInitialized();

            };

            return act;
        }

        private static HttpConfiguration CreateHttpConfiguration(IContainer container, bool isUnitTest, ICertificateValidator certificateValidator)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            //config.Services.Replace(typeof(ITraceWriter), new WebApiTracer());
            //config.Services.Replace(typeof(IAssembliesResolver), new JustThisAssemblyResolver());

            var certificateRetriever = container.Resolve<ICertificateRetriever>();
            ConfigureMessageHandlers(config, isUnitTest, certificateRetriever, certificateValidator);
            ConfigureFormatters(config);
            RouteConfig.RegisterRoutes(config);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            return config;
        }

        // Public to allow unit tests to call this method directly. 
        public static void ConfigureMessageHandlers(HttpConfiguration config, bool isUnitTest, ICertificateRetriever certificateRetriever, ICertificateValidator certificateValidator)
        {
            config.MessageHandlers.Add(new RequiredHeadersHandler());
            if (!isUnitTest)
            {
                config.MessageHandlers.Add(new CertificateValidatorHandler(certificateValidator, certificateRetriever));
            }
            config.MessageHandlers.Add(new DoubleSlashSubscriptionsHandler());
            config.MessageHandlers.Add(new XHttpMethodHandler());
            config.MessageHandlers.Add(new XMsClientRequestIdHandler());
            config.MessageHandlers.Add(new AcceptLanguageHeaderHandler());
        }

        private static void ConfigureFormatters(HttpConfiguration config)
        {
            config.Formatters.Add(new JsonMediaTypeFormatter
            {
                Indent = true,
                UseDataContractJsonSerializer = false,
                SerializerSettings = JsonHelpers.SerializerSettings
            });
        }
    }
}
