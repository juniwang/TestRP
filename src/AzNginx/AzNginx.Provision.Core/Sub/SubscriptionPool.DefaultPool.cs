using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Sub
{
    public partial class SubscriptionPool
    {
        private static SubscriptionPool _defaultPool;
        public static SubscriptionPool DefaultPool
        {
            get
            {
                if (_defaultPool == null)
                {
                    var defaultFactory = new FileSubscriptionPoolFactory();
                    _defaultPool = defaultFactory.CreatePool();
                }
                return _defaultPool;
            }
        }
    }
}
