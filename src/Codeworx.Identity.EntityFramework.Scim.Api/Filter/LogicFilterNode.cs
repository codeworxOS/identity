using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public class LogicFilterNode : FilterNode
    {
        public LogicFilterNode(FilterNode left, FilterNode right, LogicOperator logicOperator)
        {
            Left = left;
            Right = right;
            LogicOperator = logicOperator;
        }

        public FilterNode Left { get; }

        public FilterNode Right { get; }

        public LogicOperator LogicOperator { get; }

        public override Expression<Func<ScimEntity<TEntity>, bool>> Convert<TEntity>(IEnumerable<IResourceMapping<TEntity>> mappings)
        {
            var param = Expression.Parameter(typeof(ScimEntity<TEntity>), "p");

            var left = Left.ToExpression(mappings);
            var right = Right.ToExpression(mappings);

            var leftBody = left.Body.Replace<ParameterExpression>(p => true, param);
            var rightBody = right.Body.Replace<ParameterExpression>(p => true, param);

            Expression body;

            if (LogicOperator == LogicOperator.Add)
            {
                body = Expression.AndAlso(leftBody, rightBody);
            }
            else
            {
                body = Expression.OrElse(leftBody, rightBody);
            }

            return Expression.Lambda<Func<ScimEntity<TEntity>, bool>>(body, param);
        }
    }
}
