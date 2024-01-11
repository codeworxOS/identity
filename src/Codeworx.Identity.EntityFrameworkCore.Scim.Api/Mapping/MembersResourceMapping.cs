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

        public override async Task CopyValueAsync(DbContext db, Group entity, GroupResource resource, Guid providerId)
        {
            var currentMembers = await db.Set<RightHolderGroup>()
                                    .Where(p => p.GroupId == entity.Id && p.RightHolder.Providers.Any(x => x.ProviderId == providerId))
                                    .Select(p => p.RightHolderId)
                                    .ToListAsync();

            var newMembers = resource.Members?.Select(p => Guid.Parse(p.Value)).ToList() ?? new List<Guid>();

            foreach (var member in currentMembers)
            {
                if (!newMembers.Contains(member))
                {
                    var toRemove = new RightHolderGroup { GroupId = entity.Id, RightHolderId = member };
                    db.Entry(toRemove).State = EntityState.Deleted;
                }
            }

            foreach (var member in newMembers)
            {
                if (!currentMembers.Contains(member))
                {
                    var toAdd = new RightHolderGroup { GroupId = entity.Id, RightHolderId = member };
                    db.Add(toAdd);
                }
            }
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
                yield return new MappedPropertyInfo(resourceType.GetProperty(nameof(GroupMemberResource.Ref))!, null, ResourceExpression.Body);
                yield return new MappedPropertyInfo(resourceType.GetProperty(nameof(GroupMemberResource.Value))!, entity.GetProperty(nameof(RightHolder.Id)), ResourceExpression.Body);
                yield return new MappedPropertyInfo(resourceType.GetProperty(nameof(GroupMemberResource.Display))!, entity.GetProperty(nameof(RightHolder.Name)), ResourceExpression.Body);
            }
        }

        private static Expression<Func<ScimEntity<Group>, List<GroupMemberResource>>> CreateEntityExpression()
        {
            return p => p.Entity.Members
                        .Select(p => new GroupMemberResource
                        {
                            Ref = p.RightHolder is User ? "User" : "Group",
                            Display = p.RightHolder.Name,
                            Value = p.RightHolderId.ToString("N"),
                        })
                        .ToList();
        }
    }
}
