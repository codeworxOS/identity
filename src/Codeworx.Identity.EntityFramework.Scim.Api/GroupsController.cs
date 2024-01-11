using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Binding;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("{providerId}/scim/Groups")]
    [Produces("application/scim+json", "application/json")]
    [Consumes("application/scim+json", "application/json")]
    [Authorize(Policy = ScimConstants.Policies.ScimInterop)]
    public class GroupsController : Controller
    {
        private readonly DbContext _db;
        private readonly IResourceMapper<Group> _mapper;
        private readonly IFilterParser _filterParser;

        public GroupsController(IContextWrapper contextWrapper, IResourceMapper<Group> mapper, IFilterParser filterParser)
        {
            _db = contextWrapper.Context;
            _mapper = mapper;
            _filterParser = filterParser;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ListResponse>> GetGroupsAsync([FromQuery] int startIndex, [FromQuery] int count, [FromQuery] string? filter, [FromQuery] string? excludedAttributes, Guid providerId)
        {
            ConfigHelper.ValidateDefaultPagination(ref startIndex, ref count);
            try
            {
                var query = from g in _db.Set<Group>().AsNoTracking()
                            from p in g.Providers.Where(p => p.ProviderId == providerId)
                            select new ScimEntity<Group> { Entity = g, ExternalId = p.ExternalIdentifier, ProviderId = p.ProviderId };

                BooleanFilterNode? filterNode = null;

                if (filter != null)
                {
                    filterNode = (BooleanFilterNode)_filterParser.Parse(filter);
                }

                var parameter = new QueryParameter(filterNode, excludedAttributes, providerId);

                var mapped = _mapper.GetResourceQuery(_db, query, parameter);

                int totalResults = await mapped.CountAsync();

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
            catch (Exception)
            {
                return BadRequest();
            }
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
        public async Task<ActionResult<GroupResponse>> GetGroupAsync(Guid id, [FromQuery] string? excludedAttributes, Guid providerId)
        {
            var query = from g in _db.Set<Group>().AsNoTracking()
                        from p in g.Providers.Where(p => p.ProviderId == providerId)
                        where g.Id == id
                        select new ScimEntity<Group> { Entity = g, ExternalId = p.ExternalIdentifier, ProviderId = p.ProviderId };

            var parameters = new QueryParameter(null, excludedAttributes, providerId);

            var mapped = _mapper.GetResourceQuery(_db, query, parameters);

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

                await _mapper.ToDatabaseAsync(_db, item, group.Flatten(), providerId);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                var response = await GetGroupAsync(item.Id, null, providerId);
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
                await _db.Set<AuthenticationProviderRightHolder>().Where(p => p.ProviderId == providerId && p.RightHolderId == id).LoadAsync();

                if (item == null)
                {
                    return NotFound();
                }

                await _mapper.ToDatabaseAsync(_db, item, group.Flatten(), providerId);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return await GetGroupAsync(item.Id, null, providerId);
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
