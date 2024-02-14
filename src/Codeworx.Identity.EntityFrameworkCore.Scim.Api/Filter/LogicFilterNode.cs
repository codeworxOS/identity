using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public class LogicFilterNode : BooleanFilterNode
    {
        public LogicFilterNode(BooleanFilterNode left, BooleanFilterNode right, LogicOperator logicOperator)
        {
            Left = left;
            Right = right;
            LogicOperator = logicOperator;
        }

        public BooleanFilterNode Left { get; }

        public BooleanFilterNode Right { get; }

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

        public override bool Evaluate(JsonObject json)
        {
            if (LogicOperator == LogicOperator.Add)
            {
                return Left.Evaluate(json) && Right.Evaluate(json);
            }

            return Left.Evaluate(json) || Right.Evaluate(json);
        }

        protected override IEnumerable<FilterNode> GetChildren()
        {
            return Left.Flatten().Concat(Right.Flatten());
        }
    }
}
