using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
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

        public GroupsController(IContextWrapper contextWrapper)
        {
            _db = contextWrapper.Context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ListResponse<GroupResponse>> GetGroupsAsync([FromQuery] int startIndex, [FromQuery] int count)
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
                var info = new ScimResponseInfo(item.Id.ToString("N"), this.Url.ActionLink(controller: "Groups")!, DateTime.Today, DateTime.Today);

                var response = new GroupResponse(info, new GroupResource { }, new ISchemaResource[] { });

                result.Add(response);
            }

            return new ListResponse<GroupResponse>(startIndex, totalResults, count, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GroupResponse>> GetGroupAsync(Guid id)
        {
            var group = await _db.Set<Group>().FirstOrDefaultAsync(x => x.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            var info = new ScimResponseInfo(group.Id.ToString("N"), this.Url.ActionLink(controller: "Groups")!, DateTime.Today, DateTime.Today);

            var response = new GroupResponse(info, new GroupResource(), new ISchemaResource[] { });

            return response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<GroupResponse>> AddGroupAsync([RequestResourceBinder] GroupResponse group)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var entity = new Group
                {
                    Id = Guid.NewGuid(),
                };

                _db.Add(entity);

                // ToDo DI?
                ////entity.ApplyParameters(group, _parameter);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                var response = await GetGroupAsync(entity.Id);
                return CreatedAtAction(nameof(AddGroupAsync), response.Value);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GroupResponse>> UpdateGroupAsync(Guid id, [RequestResourceBinder] GroupResponse group)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var entity = await _db.Set<Group>().Where(p => p.Id == id).FirstOrDefaultAsync();

                if (entity == null)
                {
                    return NotFound();
                }

                // ToDo DI?
                //// entity.ApplyParameters(group, _parameters);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(await GetGroupAsync(entity.Id));
            }
        }
    }
}
