using AzNginx.Storage.Entities.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Storage.Resource
{
    public static class NginxResourceTableExtensions
    {
        static Expression<Func<NginxResourceEntity, bool>> IsNotDeletedExpr = nr => (
            nr.Status != NginxResourceStatus.Deleted &&
            nr.Status != NginxResourceStatus.DeleteFailed &&
            nr.Status != NginxResourceStatus.Deleting
       );

        public static NginxResourceEntity TryGetNginxResource(this NginxResourceTable table,
               Expression<Func<NginxResourceEntity, bool>> expr, bool excludeDeleted)
        {
            // TODO the filter is case-sensitive !!! we must support case-insensive query!!!
            expr = RefreshFilter(expr, excludeDeleted);
            return table.CreateQuery().Where(expr).FirstOrDefault();
        }

        public static IEnumerable<NginxResourceEntity> GetNginxResources(this NginxResourceTable table,
               Expression<Func<NginxResourceEntity, bool>> expr, bool excludeDeleted)
        {
            expr = RefreshFilter(expr, excludeDeleted);
            return table.CreateQuery().Where(expr).ToArray();
        }

        public static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
        {
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    exp1.Body, new ExpressionParameterReplacer(exp2.Parameters, exp1.Parameters).Visit(exp2.Body)),
                exp1.Parameters);
        }

        public static Expression<Func<NginxResourceEntity, bool>> RefreshFilter(Expression<Func<NginxResourceEntity, bool>> expr, bool excludeDeleted)
        {
            if (expr == null)
            {
                expr = d => true;
            }

            if (excludeDeleted)
            {
                expr = And(expr, IsNotDeletedExpr);
            }
            return expr;
        }
    }

    public class ExpressionParameterReplacer : ExpressionVisitor
    {
        public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
        {
            ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();
            for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
                ParameterReplacements.Add(fromParameters[i], toParameters[i]);
        }

        private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements { get; set; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            ParameterExpression replacement;
            if (ParameterReplacements.TryGetValue(node, out replacement))
                node = replacement;
            return base.VisitParameter(node);
        }
    }
}
