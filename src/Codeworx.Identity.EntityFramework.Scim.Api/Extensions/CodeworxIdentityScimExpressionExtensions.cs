using System.Collections.Generic;

namespace System.Linq.Expressions
{
    public static class CodeworxIdentityScimExpressionExtensions
    {
        public static Expression Replace<TExpression>(this Expression expression, Func<TExpression, bool> predicate, Expression replace)
            where TExpression : Expression
        {
            var visitor = new ReplaceVisitor<TExpression>(predicate, replace);
            return visitor.Visit(expression)!;
        }

        public static string GetPath(this Expression expression)
        {
            var path = new List<string>();

            var current = expression;

            while (!(current is ParameterExpression))
            {
                if (current is UnaryExpression unary)
                {
                    current = unary.Operand;
                }
                else if (current is MemberExpression member)
                {
                    path.Insert(0, member.Member.Name);
                    current = member.Expression;
                }
            }

            return string.Join(".", path);
        }

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

        private class ReplaceVisitor<TExpression> : ExpressionVisitor
        where TExpression : Expression
        {
            private readonly Func<TExpression, bool> _predicate;
            private readonly Expression _replace;

            public ReplaceVisitor(Func<TExpression, bool> predicate, Expression replace)
            {
                _predicate = predicate;
                _replace = replace;
            }

            public override Expression? Visit(Expression? node)
            {
                if (node is TExpression typed && _predicate(typed))
                {
                    return _replace;
                }

                return base.Visit(node);
            }
        }
    }
}
