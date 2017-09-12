using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.DAL
{
    public static class NginxDbContextExtensions
    {
        public static Task<NginxResource> TryGetNginxResource(this NginxDbContext context,
            Expression<Func<NginxResource, bool>> expr, bool excludeDeleted)
        {
            expr = RefreshFilter(expr, excludeDeleted);
            return context.Set<NginxResource>().FirstOrDefaultAsync(expr);
        }

        public static async Task<IEnumerable<NginxResource>> GetNginxResourcesAsync(this NginxDbContext context,
            Expression<Func<NginxResource, bool>> expr, bool excludeDeleted)
        {
            expr = RefreshFilter(expr, excludeDeleted);
            var result = context.Set<NginxResource>().Where(expr);
            return await result.ToArrayAsync();
        }

        /// <summary>
        /// Helper to encapsulate all the deleted states. For LINQ we need to give 
        /// expression to push the query to the data source
        /// </summary>
        public static Expression<Func<NginxResource, bool>> AndNotDeleted(Expression<Func<NginxResource, bool>> withoutDeleteCaluse)
        {
            return SwapLambdaParameter.AndPredicates(withoutDeleteCaluse, NginxResourceStatusExpressions.IsNotInAnyOfDeleteState);
        }

        public static Expression<Func<NginxResource, bool>> RefreshFilter(Expression<Func<NginxResource, bool>> expr, bool excludeDeleted)
        {
            if (expr == null)
            {
                expr = d => true;
            }

            if (excludeDeleted)
            {
                expr = AndNotDeleted(expr);
            }
            return expr;
        }
    }
}
