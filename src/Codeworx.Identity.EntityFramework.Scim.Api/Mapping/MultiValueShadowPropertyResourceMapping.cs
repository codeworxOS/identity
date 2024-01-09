using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class MultiValueShadowPropertyResourceMapping<TEntity, TResource, TMultiValueResource, TData> : ResourceMapping<TEntity, TResource, ICollection<TMultiValueResource>>, IPropertyName
        where TEntity : class
        where TResource : IScimResource
        where TMultiValueResource : MultiValueResource<TData>, new()
    {
        public MultiValueShadowPropertyResourceMapping(Expression<Func<TResource, ICollection<TMultiValueResource>>> resourceExpression, string propertyName, string type, bool primary = false)
            : base(
                      p => new List<TMultiValueResource>()
                      {
                          new TMultiValueResource
                          {
                              Value = EF.Property<TData>(p, propertyName),
                              Type = type,
                              Primary = primary,
                          },
                      },
                      resourceExpression)
        {
            Type = type;
            PropertyName = propertyName;
        }

        public string Type { get; }

        public string PropertyName { get; }

        public override Task CopyValueAsync(DbContext db, TEntity entity, TResource resource)
        {
            var entry = db.Entry(entity);
            var resourceValue = GetResourceValue(resource);
            var filtered = resourceValue.Where(p => p.Type == Type).FirstOrDefault();

            if (filtered != null)
            {
                entry.Property(PropertyName).CurrentValue = filtered.Value;
            }
            else
            {
                entry.Property(PropertyName).CurrentValue = null;
            }

            return Task.CompletedTask;
        }

        protected override IEnumerable<MappedPropertyInfo> GetMappedProperties(DbContext db)
        {
            var resourceType = typeof(TMultiValueResource);

            var column = db.Model.FindEntityType(typeof(TEntity))?.FindProperty(PropertyName);
            var valueMember = resourceType.GetProperty(nameof(MultiValueResource<string>.Value));
            if (valueMember != null)
            {
                yield return new MappedPropertyInfo(valueMember, column, ResourceExpression.Body);
                yield return new MappedPropertyInfo(resourceType.GetProperty(nameof(MultiValueResource<string>.Type))!, null, ResourceExpression.Body);
                yield return new MappedPropertyInfo(resourceType.GetProperty(nameof(MultiValueResource<string>.Primary))!, null, ResourceExpression.Body);
            }
        }
    }
}
