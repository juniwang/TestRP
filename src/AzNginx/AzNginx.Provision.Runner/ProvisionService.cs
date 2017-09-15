using Autofac;
using AzNginx.Provision.Core;
using AzNginx.Provision.Core.NginxProvision;
using AzNginx.Provision.Core.Scheduler;
using AzNginx.Provision.Core.ServiceSettings;
using AzNginx.Storage.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Runner
{
    public class ProvisionService
    {
        /// <summary>
        /// Start provision service. Call this method in your host server.
        /// </summary>
        public static void Run(IContainer container = null)
        {
            JobDispatcher dispacher;
            if (container == null)
            {
                NginxProvisionSettings settings = new NginxProvisionSettings();
                IJobManager jobManager = new JobManager();
                dispacher = new JobDispatcher(jobManager, settings);
            }
            else
            {
                dispacher = container.Resolve<JobDispatcher>();
            }
            dispacher.Start();
        }

        public static void RegisterServices(ContainerBuilder builder)
        {
            // for provision service only. IServiceSettings should be resovled to a different settings in a different service.
            builder.RegisterType<NginxProvisionSettings>().PropertiesAutowired().SingleInstance();
            builder.Register<IServiceSettings>(ctx => ctx.Resolve<NginxProvisionSettings>())
                .SingleInstance().PropertiesAutowired();
            builder.RegisterInstance(new JobManager()).As(typeof(IJobManager));
            builder.RegisterType<JobDispatcher>().SingleInstance();

            // for both provision service and the restAPIs
            builder.RegisterType<Provisioner>().PropertiesAutowired();
            builder.RegisterType<NginxResourcesStore>().PropertiesAutowired();
            builder.RegisterType<NginxResourceTable>().PropertiesAutowired();
            builder.RegisterType<NginxJobScheduler>().PropertiesAutowired();

        }
    }
}
