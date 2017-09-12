using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzNginx.Common;

namespace AzNginx.Provision.Core.Sub
{
    public interface ISubscriptionPoolFactory
    {
        SubscriptionPool CreatePool();
    }

    public class FileSubscriptionPoolFactory : ISubscriptionPoolFactory
    {
        public SubscriptionPool CreatePool()
        {
            var pool = new SubscriptionPool();

            var dir = new DirectoryInfo(ConfigDir);
            if (!dir.Exists)
            {
                AzureLog.Warn("No subscriptions loaded");
                return pool;
            }

            foreach (var file in dir.GetFiles("*.json"))
            {
                using (StreamReader reader = new StreamReader(file.OpenRead()))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    SubscriptionConfigs configs = (SubscriptionConfigs)serializer.Deserialize(reader, typeof(SubscriptionConfigs));
                    foreach (var sub in configs.Subscriptions)
                    {
                        pool.Add(new AzureSubscription(sub), sub.Name);
                    }
                }

            }

            return pool;
        }

        string ConfigDir
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sub");
            }
        }
    }
}
