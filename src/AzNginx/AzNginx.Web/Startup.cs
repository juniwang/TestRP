using Autofac;
using Autofac.Integration.WebApi;
using AzNginx.Common;
using AzNginx.Common.Helpers;
using AzNginx.Common.Validator;
using AzNginx.Models;
using AzNginx.Provision.Core.Sub;
using AzNginx.Provision.Runner;
using AzNginx.Web.Handlers;
using Microsoft.Owin;
using Owin;
using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;

[assembly: OwinStartup(typeof(AzNginx.Web.Startup))]

namespace AzNginx.Web
{
    public partial class Startup
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

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            X509Certificate2 mySslCert = null;
            string[] trustedCsmCerts = AppSettingsHelper.ReadStringArray("TrustedCsmClientCertThumbprints");
            if (mySslCert != null)
            {
                trustedCsmCerts = trustedCsmCerts.Concat(new[] {
                    mySslCert.Thumbprint
                }).ToArray();
            }

            var container = CreateContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            Configure(app, container, trustedCsmCerts);
        }

        public void Configure(
            IAppBuilder appBuilder,
            IContainer container, string[] trustedCsmCerts, bool isUnitTest = false)
        {
            ICertificateValidator csmCertificateValidator = null;
            if (!isUnitTest)
            {
                string csmCertificatesFetchUrl = AzNginxConfiguration.Rest.CsmCertificatesFetchUrl;
                csmCertificateValidator = new DownloadedListCertificateValidator(csmCertificatesFetchUrl, trustedCsmCerts);
            }
            var csmAction = GetAppBuilderAction(container, csmCertificateValidator, isUnitTest);
            csmAction(appBuilder);
        }

        IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.Register<IServiceProvider>(ctx => new AutoFacServiceProvider(ctx.Resolve<IComponentContext>()));

            // start monitoring settings blob
            //StartMonitoringResourceProviderSettings(builder);

            RegisterServices(builder);

            IContainer container = builder.Build();
            return container;
        }

        void RegisterServices(ContainerBuilder builder)
        {
            RegisterDependencies(builder);
            RegisterControllers(builder);
        }

        void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<NginxResponseBuilder>().InstancePerLifetimeScope().PropertiesAutowired();
            builder.RegisterInstance(new CertificateRetriever()).As<ICertificateRetriever>();
            builder.RegisterType<AzureResourceManager>().As<IAzureResourceManager>().PropertiesAutowired();
            builder.RegisterInstance(new FileSubscriptionPoolFactory()).As<ISubscriptionPoolFactory>();

            // register provision services
            ProvisionService.RegisterServices(builder);

            // register SubscriptionPool
            builder.Register<SubscriptionPool>(ctx => GetSubscriptionPool(ctx)).SingleInstance().PropertiesAutowired();
        }

        void RegisterControllers(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(typeof(WebApiApplication).Assembly).PropertiesAutowired();
        }

        static SubscriptionPool GetSubscriptionPool(IComponentContext ctx)
        {
            var factory = ctx.Resolve<ISubscriptionPoolFactory>();
            return factory.CreatePool();
        }

        private Action<IAppBuilder> GetAppBuilderAction(IContainer container, ICertificateValidator certificateValidator, bool isUnitTest = false)
        {
            Action<Owin.IAppBuilder> act = (app) =>
            {
                HttpConfiguration config = CreateHttpConfiguration(container, isUnitTest, certificateValidator);

                app.UseWebApi(config);
                config.EnsureInitialized();

                ProvisionService.Run(container);
            };

            return act;
        }

        private HttpConfiguration CreateHttpConfiguration(IContainer container, bool isUnitTest, ICertificateValidator certificateValidator)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            var certificateRetriever = container.Resolve<ICertificateRetriever>();
            ConfigureMessageHandlers(config, isUnitTest, certificateRetriever, certificateValidator);
            ConfigureFormatters(config);

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            return config;
        }

        // Public to allow unit tests to call this method directly. 
        public void ConfigureMessageHandlers(HttpConfiguration config, bool isUnitTest, ICertificateRetriever certificateRetriever, ICertificateValidator certificateValidator)
        {
            config.MessageHandlers.Add(new RequiredHeadersHandler());
            if (!isUnitTest)
            {
                // TODO disable certification verification temporary. Seems like that DF env doesn't send the cert correctly.
                //config.MessageHandlers.Add(new CertificateValidatorHandler(certificateValidator, certificateRetriever));
            }
            config.MessageHandlers.Add(new DoubleSlashSubscriptionsHandler());
            config.MessageHandlers.Add(new XHttpMethodHandler());
            config.MessageHandlers.Add(new XMsClientRequestIdHandler());
            config.MessageHandlers.Add(new AcceptLanguageHeaderHandler());
            config.MessageHandlers.Add(new LogHttpHandler());
        }

        private void ConfigureFormatters(HttpConfiguration config)
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
