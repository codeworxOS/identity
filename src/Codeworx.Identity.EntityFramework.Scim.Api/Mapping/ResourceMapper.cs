﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class ResourceMapper<TEntity> : IResourceMapper<TEntity>
        where TEntity : class
    {
        private readonly IEnumerable<IResourceMapping<TEntity>> _mappings;
        private readonly ImmutableDictionary<Type, ISchemaExtension> _schemas;

        public ResourceMapper(IEnumerable<IResourceMapping<TEntity>> mappings, IEnumerable<ISchemaExtension> schemas)
        {
            _mappings = mappings;
            _schemas = schemas.ToImmutableDictionary(p => p.TargetType, p => p);
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

        public async IAsyncEnumerable<SchemaDataResource> GetSchemasAsync(DbContext db)
        {
            var container = new ComplexMemberSchemaTreeItem(new SchemaPath("root", false), typeof(CommonResponseResource));

            foreach (var mapping in _mappings)
            {
                await foreach (var schemaInfo in mapping.GetSchemaAttributesAsync(db))
                {
                    container.Parse(schemaInfo);
                }
            }

            var grouped = container.Members.Values.GroupBy(p => p.ResourceType)
                            .ToDictionary(p => p.Key, p => p.ToList());

            foreach (var item in grouped)
            {
                if (_schemas.TryGetValue(item.Key, out var schema))
                {
                    yield return new SchemaDataResource(schema.Schema, schema.Name, item.Value.Select(p => p.GetAttribute()).ToList());
                }
            }
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

        private abstract class MemberSchemaTreeItem
        {
            public abstract Type ResourceType { get; }

            public abstract SchemaDataAttributeResource GetAttribute();
        }

        private class ScalarMemberSchemaTreeItem : MemberSchemaTreeItem
        {
            public ScalarMemberSchemaTreeItem(SchemaInfo info, Type resourceType)
            {
                Info = info;
                ResourceType = resourceType;
            }

            public SchemaInfo Info { get; }

            public override Type ResourceType { get; }

            public override SchemaDataAttributeResource GetAttribute()
            {
                return new SchemaDataAttributeResource(Info.Paths.Last().Name, Info.DataType, false, null, false, null, false, "readWrite", "default", "none", null);
            }
        }

        private class ComplexMemberSchemaTreeItem : MemberSchemaTreeItem
        {
            public ComplexMemberSchemaTreeItem(SchemaPath path, Type resourceType)
            {
                Path = path;
                ResourceType = resourceType;
            }

            public Dictionary<string, MemberSchemaTreeItem> Members { get; } = new Dictionary<string, MemberSchemaTreeItem>();

            public SchemaPath Path { get; }

            public override Type ResourceType { get; }

            public void Parse(SchemaInfo info)
            {
                var paths = new List<MemberExpression>();

                ComplexMemberSchemaTreeItem container = this;

                for (int i = 0; i < (info.Paths.Count - 1); i++)
                {
                    var currentPath = info.Paths[i];
                    if (!this.Members.TryGetValue(currentPath.Name, out var next))
                    {
                        next = new ComplexMemberSchemaTreeItem(currentPath, info.ResourceType);
                        this.Members.Add(currentPath.Name, next);
                    }

                    container = (ComplexMemberSchemaTreeItem)next;
                }

                container.Members.Add(info.Paths.Last().Name, new ScalarMemberSchemaTreeItem(info, info.ResourceType));
            }

            public override SchemaDataAttributeResource GetAttribute()
            {
                var result = new SchemaDataAttributeResource(Path.Name, Path.IsMulti, null, false, false);
                result.SubAttributes!.AddRange(Members.Values.Select(p => p.GetAttribute()));
                return result;
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
