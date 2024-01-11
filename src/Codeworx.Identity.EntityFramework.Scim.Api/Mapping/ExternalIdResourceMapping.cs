using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class ExternalIdResourceMapping<TEntity, TResource> : ResourceMapping<TEntity, TResource, string>, IReadOnly
        where TEntity : RightHolder
        where TResource : IScimResource
    {
        public ExternalIdResourceMapping(Expression<Func<TResource, string>> resourceExpression)
            : base(p => p.ExternalId, resourceExpression)
        {
        }

        public bool ReadOnly { get; } = true;

        public override async Task CopyValueAsync(DbContext db, TEntity entity, TResource resource, Guid providerId)
        {
            var rightHolderId = entity.Id;

            var current = db.ChangeTracker.Entries<AuthenticationProviderRightHolder>().Where(p => p.Entity.RightHolderId == rightHolderId).Select(p => p.Entity).FirstOrDefault();

            if (current == null)
            {
                current = await db.Set<AuthenticationProviderRightHolder>().Where(p => p.ProviderId == providerId && p.RightHolderId == rightHolderId).FirstOrDefaultAsync();

                if (current == null)
                {
                    current = new AuthenticationProviderRightHolder { ProviderId = providerId, RightHolderId = rightHolderId };
                }
            }

            current.ExternalIdentifier = GetResourceValue(resource);
        }

        public override Expression<Func<ScimEntity<TEntity>, bool>>? GetFilter(OperationFilterNode operationFilterNode)
        {
            var path = string.Join(".", operationFilterNode.Paths);

            if (path.Equals(ResourcePath, StringComparison.OrdinalIgnoreCase))
            {
                var value = Expression.Constant(operationFilterNode.Value);
                var body = Expression.Equal(Entity.Body, value);

                return Expression.Lambda<Func<ScimEntity<TEntity>, bool>>(body, Entity.Parameters[0]);
            }

            return null;
        }

        protected override IEnumerable<MappedPropertyInfo> GetMappedProperties(DbContext db)
        {
            if (Resource.Body is MemberExpression member)
            {
                IProperty? column = null;

                if (EntityExpression.Body is MemberExpression entityMember)
                {
                    column = db.Model.FindEntityType(entityMember.Expression!.Type)?.FindProperty(entityMember.Member);
                }

                yield return new MappedPropertyInfo(member.Member, column, member.Expression);
            }
        }
    }
}
