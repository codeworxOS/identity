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
    }
}
