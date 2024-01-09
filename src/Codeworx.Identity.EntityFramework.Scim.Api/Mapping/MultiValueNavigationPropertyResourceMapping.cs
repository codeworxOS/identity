using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class MultiValueNavigationPropertyResourceMapping<TEntity, TResource, TData> : ResourceMapping<TEntity, TResource, TData>, IReadOnly
        where TEntity : class
        where TResource : IScimResource
        where TData : IEnumerable<MultiValueResource>
    {
        public MultiValueNavigationPropertyResourceMapping(Expression<Func<TResource, TData>> resourceExpression, Expression<Func<TEntity, TData>> entityExpression, bool readOnly = false)
            : base(entityExpression, resourceExpression)
        {
            ReadOnly = readOnly;
        }

        public bool ReadOnly { get; }

        public override Task CopyValueAsync(DbContext db, TEntity entity, TResource resource)
        {
            var value = GetResourceValue(resource);
            ////_setValueDelegate(entity, value);

            return Task.CompletedTask;
        }

        protected override IEnumerable<MappedPropertyInfo> GetMappedProperties(DbContext db)
        {
            var parent = this.Resource.Body;

            if (parent.Type.IsEnumerable(out var elementType))
            {
                var memberInit = this.EntityExpression.Body.FindFirst<MemberInitExpression>(p => p.Type == elementType);

                if (memberInit != null)
                {
                    var members = elementType!.GetProperties();

                    foreach (var item in memberInit.Bindings)
                    {
                        var member = members.FirstOrDefault(d => d.Name == item.Member.Name) ?? item.Member;
                        IProperty? column = null;
                        if (item is MemberAssignment assign && assign.Expression is MemberExpression entityMember)
                        {
                            column = db.Model.FindEntityType(entityMember.Expression!.Type)?.FindProperty(entityMember.Member);
                        }

                        yield return new MappedPropertyInfo(member, column, parent);
                    }
                }
            }
        }

        protected override string GetMutability(IEnumerable<Attribute> attributes, MappedPropertyInfo property)
        {
            if (ReadOnly)
            {
                return "immutable";
            }

            return base.GetMutability(attributes, property);
        }
    }
}
