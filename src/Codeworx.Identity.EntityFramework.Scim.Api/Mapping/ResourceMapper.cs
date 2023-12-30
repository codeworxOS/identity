using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class ResourceMapper<TEntity> : IResourceMapper<TEntity>
        where TEntity : class
    {
        private readonly IEnumerable<IResourceMapping<TEntity>> _mappings;

        public ResourceMapper(IEnumerable<IResourceMapping<TEntity>> mappings)
        {
            _mappings = mappings;
        }

        public IQueryable<Dictionary<string, IScimResource>> GetResourceQuery(IQueryable<TEntity> baseQuery)
        {
            var entity = Expression.Parameter(typeof(TEntity), "p");

            var resources = _mappings.GroupBy(p => p.ResourceExpression.Parameters[0].Type);

            var addMethod = typeof(Dictionary<string, IScimResource>).GetMethod(nameof(Dictionary<string, IScimResource>.Add), new Type[] { typeof(string), typeof(IScimResource) })!;

            List<ElementInit> elementInits = new List<ElementInit>();

            foreach (var resource in resources)
            {
                var members = new MemberBindingContainerTreeItem(resource.Key);

                foreach (var item in resource)
                {
                    var visitor = new ReplaceParameterVisitor(item.EntityExpression.Parameters[0], entity);

                    members.Parse(item.ResourceExpression, visitor.Visit(item.EntityExpression.Body));
                }

                var expression = members.GetExpression();
                elementInits.Add(Expression.ElementInit(addMethod, Expression.Constant(resource.Key.Name), expression));
            }

            var select = Expression.Lambda<Func<TEntity, Dictionary<string, IScimResource>>>(
                Expression.ListInit(
                    Expression.New(typeof(Dictionary<string, IScimResource>)),
                    elementInits), entity);

            return baseQuery.Select(select);
        }

        public async Task ToDatabaseAsync(DbContext db, TEntity entity, IEnumerable<ISchemaResource> resources)
        {
            foreach (var resource in resources)
            {
                foreach (var mapping in _mappings)
                {
                    await mapping.ToDatabaseAsync(db, entity, resource);
                }
            }
        }

        private abstract class MemberBindingTreeItem
        {
            public abstract Expression GetExpression();
        }

        private class MemberBindingExpressionTreeItem : MemberBindingTreeItem
        {
            public MemberBindingExpressionTreeItem(Expression mappedExpression)
            {
                MappedExpression = mappedExpression;
            }

            public Expression MappedExpression { get; }

            public override Expression GetExpression()
            {
                return MappedExpression;
            }
        }

        private class MemberBindingContainerTreeItem : MemberBindingTreeItem
        {
            public MemberBindingContainerTreeItem(Type dataType)
            {
                DataType = dataType;
            }

            public Dictionary<MemberInfo, MemberBindingTreeItem> MemberBindings { get; } = new Dictionary<MemberInfo, MemberBindingTreeItem>();

            public Type DataType { get; }

            public void Parse(LambdaExpression resourceExpression, Expression mappedExpression)
            {
                var paths = new List<MemberExpression>();

                var current = resourceExpression.Body;
                while (current != resourceExpression.Parameters[0] && current != null)
                {
                    if (current is MemberExpression member)
                    {
                        paths.Add(member);
                        current = member.Expression;
                    }
                    else
                    {
                        throw new NotSupportedException("Only member expressions are supported e.g. (p.Username or p.Name.FirstName)");
                    }
                }

                if (current == null)
                {
                    throw new NotSupportedException("The root expression must be the resource parameter.");
                }

                MemberBindingContainerTreeItem container = this;

                for (int i = paths.Count - 1; i > 0; i--)
                {
                    var member = paths[i];

                    if (!container.MemberBindings.TryGetValue(member.Member, out var value))
                    {
                        value = new MemberBindingContainerTreeItem(member.Type);
                        container.MemberBindings.Add(member.Member, value);
                    }

                    if (value is MemberBindingContainerTreeItem containerValue)
                    {
                        container = containerValue;
                    }
                    else
                    {
                        throw new NotSupportedException("This should not happen!");
                    }
                }

                container.MemberBindings.Add(paths[0].Member, new MemberBindingExpressionTreeItem(mappedExpression));
            }

            public override Expression GetExpression()
            {
                var bindings = MemberBindings.Select(p => Expression.Bind(p.Key, p.Value.GetExpression()));

                return Expression.MemberInit(
                                    Expression.New(DataType),
                                    bindings);
            }
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _search;
            private readonly ParameterExpression _replace;

            public ReplaceParameterVisitor(ParameterExpression search, ParameterExpression replace)
            {
                _search = search;
                _replace = replace;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _search ? _replace : base.VisitParameter(node);
            }
        }
    }
}
