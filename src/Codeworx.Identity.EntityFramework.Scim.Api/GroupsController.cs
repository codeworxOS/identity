using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Binding;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("scim/Groups")]
    [AllowAnonymous]
    public class GroupsController : Controller
    {
        private readonly DbContext _db;
        private readonly IEnumerable<IGroupSchemaProperty> _mappedProperties;

        public GroupsController(IContextWrapper contextWrapper, IEnumerable<IGroupSchemaProperty> mappedProperties)
        {
            _db = contextWrapper.Context;
            _mappedProperties = mappedProperties;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ListResponse> GetGroupsAsync([FromQuery] int startIndex, [FromQuery] int count)
        {
            ConfigHelper.ValidateDefaultPagination(ref startIndex, ref count);

            var query = _db.Set<Group>().AsNoTracking().OrderBy(c => c.Id).AsQueryable();
            int totalResults = await query.CountAsync();

            if (startIndex > 1)
            {
                query = query.Skip(startIndex - 1);
            }

            if (count > 0)
            {
                query = query.Take(count);
            }

            var groups = await query.ToListAsync();
            var result = new List<GroupResponse>();

            foreach (var item in groups)
            {
                var response = GenerateGroupResponse(item);

                result.Add(response);
            }

            return new ListResponse(startIndex, totalResults, count, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GroupResponse>> GetGroupAsync(Guid id)
        {
            var item = await _db.Set<Group>().FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var response = GenerateGroupResponse(item);

            return response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<GroupResponse>> AddGroupAsync([RequestResourceBinder] GroupRequest group)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var item = new Group
                {
                    Id = Guid.NewGuid(),
                };

                _db.Add(item);

                ApplyEntityChanges(group, item);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                var response = await GetGroupAsync(item.Id);
                return CreatedAtAction(nameof(AddGroupAsync), response.Value);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GroupResponse>> UpdateGroupAsync(Guid id, [RequestResourceBinder] GroupRequest group)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var item = await _db.Set<Group>().Where(p => p.Id == id).FirstOrDefaultAsync();

                if (item == null)
                {
                    return NotFound();
                }

                ApplyEntityChanges(group, item);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(await GetGroupAsync(item.Id));
            }
        }

        private GroupResponse GenerateGroupResponse(Group item)
        {
            var groupUrl = this.Url.ActionLink(controller: "Groups", action: "GetGroup", values: new { id = item.Id.ToString("N") })!;

            var info = new ScimResponseInfo(item.Id.ToString("N"), groupUrl, DateTime.Today, DateTime.Today);

            var entityEntry = _db.Entry(item);
            var resource = new GroupResource();
            var list = new List<ISchemaResource>();
            foreach (var resourceType in _mappedProperties.GroupBy(d => d.ResourceType))
            {
                object data;
                if (resourceType.Key == typeof(GroupResource))
                {
                    data = resource;
                }
                else
                {
                    if (Activator.CreateInstance(resourceType.Key) is ISchemaResource d)
                    {
                        data = d;
                        list.Add(d);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                foreach (var property in resourceType)
                {
                    property.SetResourceValue(data, entityEntry.Property(property.EntityPropertyName).CurrentValue);
                }
            }

            var response = new GroupResponse(info, resource, list.ToArray());
            return response;
        }

        private void ApplyEntityChanges(GroupRequest groupRequest, Group item)
        {
            var entityEntry = _db.Entry(item);

            foreach (var resourceType in _mappedProperties.GroupBy(d => d.ResourceType))
            {
                object data;
                if (resourceType.Key == typeof(GroupResource))
                {
                    data = groupRequest.Resource;
                }
                else
                {
                    data = groupRequest.Extensions.FirstOrDefault(d => d.GetType() == resourceType.Key) ?? Activator.CreateInstance(resourceType.Key) ?? throw new InvalidOperationException();
                }

                foreach (var property in resourceType)
                {
                    entityEntry.Property(property.EntityPropertyName).CurrentValue = property.GetResourceValue(data);
                }
            }
        }
    }
}
