using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzNginx.Provision.Core.Sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Sub.Tests
{
    [TestClass]
    public class FileSubscriptionPoolFactoryTests
    {
        [TestMethod]
        public void CreatePoolTest()
        {
            FileSubscriptionPoolFactory factory = new FileSubscriptionPoolFactory();
            SubscriptionPool pool = factory.CreatePool();
            Assert.AreEqual(pool.Count, 2);

            var sub = pool.GetByName("sub 1");
            Assert.IsNotNull(sub);
            Assert.AreEqual(sub.SubscriptionId, "11111111-1111-1111-1111-111111111111");
            Assert.IsNotNull(sub.AzureEnvironment);
            Assert.AreEqual(sub.AzureEnvironment.ResourceManagerEndpoint, "https://management.azure.com/");

            Assert.IsNull(pool.GetById("no such sub"));
        }
    }
}