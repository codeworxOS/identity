using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Binding;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("{providerId}/scim/Groups")]
    [AllowAnonymous]
    public class GroupsController : Controller
    {
        private readonly DbContext _db;
        private readonly IResourceMapper<Group> _mapper;

        public GroupsController(IContextWrapper contextWrapper, IResourceMapper<Group> mapper)
        {
            _db = contextWrapper.Context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ListResponse> GetGroupsAsync([FromQuery] int startIndex, [FromQuery] int count, Guid providerId)
        {
            ConfigHelper.ValidateDefaultPagination(ref startIndex, ref count);

            var query = _db.Set<Group>().AsNoTracking().OrderBy(c => c.Id).AsQueryable();
            var mapped = _mapper.GetResourceQuery(query);

            int totalResults = await query.CountAsync();

            if (startIndex > 1)
            {
                mapped = mapped.Skip(startIndex - 1);
            }

            if (count > 0)
            {
                mapped = mapped.Take(count);
            }

            var groups = await mapped.ToListAsync();
            var result = new List<GroupResponse>();

            foreach (var item in groups)
            {
                var response = GenerateGroupResponse(item.Values);

                result.Add(response);
            }

            return new ListResponse(startIndex, totalResults, count, result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteGroupAsync(Guid id)
        {
            var item = await _db.Set<Group>().Where(p => p.Id == id).FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            var entry = _db.Remove(item);

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GroupResponse>> GetGroupAsync(Guid id, Guid providerId)
        {
            var query = _db.Set<Group>().Where(x => x.Id == id);
            var mapped = _mapper.GetResourceQuery(query);

            var item = await mapped.FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            var response = GenerateGroupResponse(item.Values);

            return response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<GroupResponse>> AddGroupAsync([RequestResourceBinder] GroupRequest group, Guid providerId)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                if (group.ExternalId == null)
                {
                    return Conflict();
                }

                var existing = await _db.Set<AuthenticationProviderRightHolder>().Where(p => p.ProviderId == providerId && p.ExternalIdentifier == group.ExternalId)
                                    .AnyAsync();

                if (existing)
                {
                    return Conflict();
                }

                var item = new Group
                {
                    Id = Guid.NewGuid(),
                };

                var mapping = new AuthenticationProviderRightHolder
                {
                    ExternalIdentifier = group.ExternalId,
                    ProviderId = providerId,
                    RightHolderId = item.Id,
                };

                _db.Add(item);
                _db.Add(mapping);

                await _mapper.ToDatabaseAsync(_db, item, group.Flatten());

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                var response = await GetGroupAsync(item.Id, providerId);
                return CreatedAtAction("AddGroup", "Groups", new { providerId = providerId }, response.Value);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GroupResponse>> UpdateGroupAsync(Guid id, [RequestResourceBinder] GroupRequest group, Guid providerId)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var item = await _db.Set<Group>().Where(p => p.Id == id).FirstOrDefaultAsync();

                if (item == null)
                {
                    return NotFound();
                }

                await _mapper.ToDatabaseAsync(_db, item, group.Flatten());

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return await GetGroupAsync(item.Id, providerId);
            }
        }

        private GroupResponse GenerateGroupResponse(IEnumerable<IScimResource> resources)
        {
            var info = resources.OfType<ScimResponseInfo>().FirstOrDefault();

            if (info == null)
            {
                throw new NotSupportedException("ScimResponseInfo must be mapped for entity!");
            }

            var groupUrl = this.Url.ActionLink(controller: "Groups", action: "GetGroup", values: new { id = info.Id })!;

            info.Location = groupUrl;
            ////var info = new ScimResponseInfo(item.Id.ToString("N"), userUrl, item.Created, item.Created);

            var group = resources.OfType<GroupResource>().FirstOrDefault();

            if (group == null)
            {
                throw new NotSupportedException("At least one property of UserResource must be mapped!");
            }

            return new GroupResponse(info, group, resources.OfType<ISchemaResource>().Except(new[] { group }).ToArray());

            ////}

            ////private GroupResponse GenerateGroupResponse(Group item)
            ////{

            ////    var info = new ScimResponseInfo(item.Id.ToString("N"), groupUrl, DateTime.Today, DateTime.Today);

            ////    var entityEntry = _db.Entry(item);
            ////    var resource = new GroupResource();
            ////    var list = new List<ISchemaResource>();
            ////    foreach (var resourceType in _mappedProperties.GroupBy(d => d.ResourceType))
            ////    {
            ////        object data;
            ////        if (resourceType.Key == typeof(GroupResource))
            ////        {
            ////            data = resource;
            ////        }
            ////        else
            ////        {
            ////            if (Activator.CreateInstance(resourceType.Key) is ISchemaResource d)
            ////            {
            ////                data = d;
            ////                list.Add(d);
            ////            }
            ////            else
            ////            {
            ////                throw new NotSupportedException();
            ////            }
            ////        }

            ////        foreach (var property in resourceType)
            ////        {
            ////            property.SetResourceValue(data, entityEntry.Property(property.EntityPropertyName).CurrentValue);
            ////        }
            ////    }

            ////    var response = new GroupResponse(info, resource, list.ToArray());
            ////    return response;
            ////}

            ////private void ApplyEntityChanges(GroupRequest groupRequest, Group item)
            ////{
            ////    var entityEntry = _db.Entry(item);

            ////    foreach (var resourceType in _mappedProperties.GroupBy(d => d.ResourceType))
            ////    {
            ////        object data;
            ////        if (resourceType.Key == typeof(GroupResource))
            ////        {
            ////            data = groupRequest.Resource;
            ////        }
            ////        else
            ////        {
            ////            data = groupRequest.Extensions.FirstOrDefault(d => d.GetType() == resourceType.Key) ?? Activator.CreateInstance(resourceType.Key) ?? throw new InvalidOperationException();
            ////        }

            ////        foreach (var property in resourceType)
            ////        {
            ////            entityEntry.Property(property.EntityPropertyName).CurrentValue = property.GetResourceValue(data);
            ////        }
            ////    }
            ////}
        }
    }
}
