using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using TestRP.Common;

namespace TestRP.Web
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            //log4Net appender
            log4net.Config.XmlConfigurator.Configure();
            AzureLog.Info("log4net initialized.");

            return base.OnStart();
        }
    }
}
