using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Storage.Entities.Resource
{
    public static class NginxResourceEntityExtension
    {
        public static bool IsDeleted(this NginxResourceEntity entity)
        {
            return entity.Status != NginxResourceStatus.Deleted
                && entity.Status != NginxResourceStatus.Deleting
                && entity.Status != NginxResourceStatus.DeleteFailed;
        }

        public static bool IsNotDeleted(this NginxResourceEntity entity)
        {
            return !entity.IsDeleted();
        }
    }
}
