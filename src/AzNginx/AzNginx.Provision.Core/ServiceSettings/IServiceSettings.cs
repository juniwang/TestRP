using AzNginx.Provision.Core.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.ServiceSettings
{
    /// <summary>
    /// Defines a contract that all settings class specific to a service needs to
    /// be implemented.  Common.Settings.Mover.MoverSettings and 
    /// Common.Settings.PackageDeployer.PackageDeployerSettings are examples for
    /// service specific settings.
    /// </summary>
    public interface IServiceSettings
    {
        /// <summary>
        /// Gets the type of job dispatching that jobs belongs to this service required.
        /// </summary>
        JobDispatchingType JobDispatchingType { get; }

        /// <summary>
        /// Gets the list of IProducerQueueInfo instances which acts as interface to 
        /// access the job producers and settings defined for producer (number of 
        /// jobs to retrieve in parallel, time to wait before retrieving next batch 
        /// of jobs etc..)
        /// </summary>
        IList<IProducerQueueInfo> ProducerQueueInfos { get; }

        /// <summary>
        /// Gets reference to the IJobProcessor instance which can handle jobs retrieved 
        /// from producers defined in ProducerQueueInfos.
        /// </summary>
        IJobProcessor JobProcessor { get; }
    }
}
