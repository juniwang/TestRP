using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using TestRP.Common.Validator;
using TestRP.Common.Helpers;
using TestRP.Common;
using System.Web.Http;
using Autofac.Integration.WebApi;
using System.Net.Http.Formatting;
using TestRP.Web.Handlers;

[assembly: OwinStartup(typeof(TestRP.Web.Startup))]

namespace TestRP.Web
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
            Configure(app, CreateContainer(), trustedCsmCerts);
        }

        public void Configure(
            IAppBuilder appBuilder,
            IContainer container, string[] trustedCsmCerts, bool isUnitTest = false)
        {
            ICertificateValidator csmCertificateValidator = null;
            if (!isUnitTest)
            {
                string csmCertificatesFetchUrl = TestRPConfiguration.CsmCertificatesFetchUrl;
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
            builder.RegisterInstance(new CertificateRetriever()).As<ICertificateRetriever>();
        }

        private Action<IAppBuilder> GetAppBuilderAction(IContainer container, ICertificateValidator certificateValidator, bool isUnitTest = false)
        {
            Action<Owin.IAppBuilder> act = (app) =>
            {
                HttpConfiguration config = CreateHttpConfiguration(container, isUnitTest, certificateValidator);

                app.UseWebApi(config);
                config.EnsureInitialized();

            };

            return act;
        }

        private HttpConfiguration CreateHttpConfiguration(IContainer container, bool isUnitTest, ICertificateValidator certificateValidator)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            //config.Services.Replace(typeof(ITraceWriter), new WebApiTracer());
            //config.Services.Replace(typeof(IAssembliesResolver), new JustThisAssemblyResolver());

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
                // TODO disable certification verification temporary
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
