using System.Collections.Generic;

namespace System.Linq.Expressions
{
    public static class CodeworxIdentityScimExpressionExtensions
    {
        public static TExpression? FindFirst<TExpression>(this Expression expression, Func<TExpression, bool> predicate)
            where TExpression : Expression
        {
            var visitor = new FindVisitor<TExpression>(predicate);
            visitor.Visit(expression);

            return visitor.Found.FirstOrDefault();
        }

        private class FindVisitor<TExpression> : ExpressionVisitor
            where TExpression : Expression
        {
            private Func<TExpression, bool> _predicate;

            public FindVisitor(Func<TExpression, bool> predicate)
            {
                _predicate = predicate;
                this.Found = new List<TExpression>();
            }

            public List<TExpression> Found { get; }

            public override Expression? Visit(Expression? node)
            {
                if (node is TExpression typed && _predicate(typed))
                {
                    Found.Add(typed);
                }

                return base.Visit(node);
            }
        }
    }
}
