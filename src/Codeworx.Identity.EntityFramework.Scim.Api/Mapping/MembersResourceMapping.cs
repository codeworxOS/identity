using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class MembersResourceMapping : ResourceMapping<Group, GroupResource, List<GroupMemberResource>>
    {
        public MembersResourceMapping()
            : base(CreateEntityExpression(), p => p.Members!)
        {
        }

        public override Task CopyValueAsync(DbContext db, Group entity, GroupResource resource, Guid providerId)
        {
            // TODO implememt;
            return Task.CompletedTask;
        }

        public override Expression<Func<ScimEntity<Group>, bool>>? GetFilter(OperationFilterNode operationFilterNode)
        {
            var path = operationFilterNode.Path.Members;

            if (path.StartsWith(ResourcePath + ".", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotImplementedException();
            }

            return null;
        }

        protected override IEnumerable<MappedPropertyInfo> GetMappedProperties(DbContext db)
        {
            var resourceType = typeof(GroupMemberResource);

            var entity = db.Model.FindEntityType(typeof(RightHolder));
            if (entity != null)
            {
                ////yield return new MappedPropertyInfo(resourceType.GetProperty(nameof(GroupMemberResource.Type))!, null, ResourceExpression.Body);
                yield return new MappedPropertyInfo(resourceType.GetProperty(nameof(GroupMemberResource.Ref))!, null, ResourceExpression.Body);
                yield return new MappedPropertyInfo(resourceType.GetProperty(nameof(GroupMemberResource.Value))!, entity.GetProperty(nameof(RightHolder.Id)), ResourceExpression.Body);
                yield return new MappedPropertyInfo(resourceType.GetProperty(nameof(GroupMemberResource.Display))!, entity.GetProperty(nameof(RightHolder.Name)), ResourceExpression.Body);
                yield return new MappedPropertyInfo(resourceType.GetProperty(nameof(GroupMemberResource.ExternalId))!, null, ResourceExpression.Body);
            }
        }

        private static Expression<Func<ScimEntity<Group>, List<GroupMemberResource>>> CreateEntityExpression()
        {
            return p => (from rhg in Query.Set<RightHolderGroup>()
                         from provider in rhg.RightHolder.Providers
                         where provider.ProviderId == Query.ProviderId
                         select new
                         {
                             GroupId = rhg.GroupId,
                             Id = rhg.RightHolderId,
                             Name = rhg.RightHolder.Name,
                             Type = rhg.RightHolder is User ? "User" : "Group",
                             ExternalId = provider.ExternalIdentifier,
                         })
                        .Where(x => x.GroupId == p.Entity.Id)
                        .Select(p => new GroupMemberResource
                        {
                            Type = p.Type,
                            Display = p.Name,
                            ExternalId = p.ExternalId,
                            Value = p.Id.ToString("N"),
                        })
                        .ToList();
        }
    }
}
