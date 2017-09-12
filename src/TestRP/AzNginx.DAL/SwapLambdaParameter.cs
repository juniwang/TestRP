using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.DAL
{
    /// <summary>
    /// When adding two lambda expressions we need to ensure that both of them are operating on the same parameter 
    /// ex: 
    ///   Expression<Func<Deployment, true>> e1 = d1 => d1.Subscription....
    ///   Expression<Func<Deployment, true>> e2 = d2 => d2.Subscription....
    ///   
    ///  If we combine directly then it will be like (d1 => d1.Subscription....) && (e2 = d2 => d2.Subscription....). 
    ///  This visitor will help in re-writing/wiring the parameters
    /// </summary>
    public class SwapLambdaParameter : ExpressionVisitor
    {
        private readonly Expression from, to;
        public SwapLambdaParameter(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }
        public override Expression Visit(Expression node)
        {
            return node == from ? to : base.Visit(node);
        }

        public static Expression<Func<T, bool>> AndPredicates<T>(Expression<Func<T, bool>> e1, Expression<Func<T, bool>> e2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(
                new SwapLambdaParameter(e1.Parameters[0], e2.Parameters[0]).Visit(e1.Body),
                e2.Body), e2.Parameters);
        }
    }
}
