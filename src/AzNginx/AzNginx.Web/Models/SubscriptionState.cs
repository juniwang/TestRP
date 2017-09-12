using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Web.Models
{
    public enum SubscriptionState
    {
        Registered, // The subscription was entitled to use your “ResourceProviderNamespace/ResourceType”.
        Unregistered, // Customer has decided to stop using all of the resource types offered by the Resource Provider. Can be treated as a soft delete. All the existing resources would have already been deprovisioned.
        Suspended, // The subscription has been suspended (generally due to fraud or non-payment) and the Resource Provider should stop the subscription from generating any additional bills. . Pay-for-use resource should have access rights revoked when the subscription is disabled.  
        Deleted, // The customer has cancelled their Windows Azure subscription and its content can be cleaned up immediately.
        Warned, // The subscription has been mostly suspended, because customer hasn't been paying their bills.
    }
}
