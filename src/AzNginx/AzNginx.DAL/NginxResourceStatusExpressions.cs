using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.DAL
{
    // Useful LINQ expressions for passing to EF
    public static class NginxResourceStatusExpressions
    {
        public static Expression<Func<NginxResource, bool>> IsActive = nr =>
        (
            nr.Status == NginxResourceStatus.Active
        );

        public static Expression<Func<NginxResource, bool>> ShouldAutoCleanupExpression = nr =>
        (
            nr.Status == NginxResourceStatus.CreateFailed ||
            nr.Status == NginxResourceStatus.DeleteFailed ||
            nr.Status == NginxResourceStatus.DisableFailed
        );

        public static Expression<Func<NginxResource, bool>> IsNotInAnyOfDeleteState = nr => (
            nr.Status != NginxResourceStatus.Deleted &&
            nr.Status != NginxResourceStatus.DeleteFailed &&
            nr.Status != NginxResourceStatus.Deleting
       );
    }
}
