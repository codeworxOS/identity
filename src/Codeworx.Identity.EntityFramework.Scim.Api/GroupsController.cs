using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("authProvider/{authProviderId}/scim/Groups")]
    [AllowAnonymous]
    public class GroupsController : Controller
    {
        private readonly DbContext _db;
        private readonly IEnumerable<ISchemaParameterDescription<Group>> _parameters;

        public GroupsController(IContextWrapper contextWrapper, IEnumerable<ISchemaParameterDescription<Group>> parameters)
        {
            _db = contextWrapper.Context;
            _parameters = parameters;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ListResponse<GroupResponse>> GetGroupsAsync([FromQuery] int startIndex, [FromQuery] int count)
        {
            if (startIndex < 1)
            {
                startIndex = 1;
            }

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
                var data = new GroupResponse
                {
                    Id = item.Id,
                };

                data.ApplyParameters(item, _parameters);

                result.Add(data);
            }

            return new ListResponse<GroupResponse>(startIndex, totalResults, count, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GroupResponse>> GetGroupASync(Guid id)
        {
            var group = await _db.Set<Group>().FirstOrDefaultAsync(x => x.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            var result = new GroupResponse { Id = group.Id };

            result.ApplyParameters(group, _parameters);

            return result;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<GroupResponse>> AddGroupAsync([FromBody] GroupResponse group)
        {
            var entity = new Group
            {
                Id = Guid.NewGuid(),
            };

            entity.ApplyParameters(group, _parameters);

            _db.Add(entity);

            await _db.SaveChangesAsync().ConfigureAwait(false);

            var response = await GetGroupASync(group.Id);
            return CreatedAtAction(nameof(AddGroupAsync), response.Value);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GroupResponse>> UpdateGroupAsync(Guid id, [FromBody] GroupResponse group)
        {
            var entity = await _db.Set<Group>().Where(p => p.Id == id).FirstOrDefaultAsync();

            if (entity == null)
            {
                return NotFound();
            }

            entity.ApplyParameters(group, _parameters);

            await _db.SaveChangesAsync();

            return Ok(await GetGroupASync(entity.Id));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GroupResponse>> PatchGroupAsync(Guid id, [FromBody] PatchOperation patch)
        {
            var entity = await _db.Set<Group>().Where(p => p.Id == id).FirstOrDefaultAsync();

            if (entity == null)
            {
                return NotFound();
            }

            foreach (var operation in patch.Operations)
            {
                // todo patch opertaion
            }

            ////entity.ApplyParameters(group, _parameters);

            await _db.SaveChangesAsync();

            return Ok(await GetGroupASync(entity.Id));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUsersAsync(Guid id)
        {
            var entity = await _db.Set<Group>().Where(p => p.Id == id).FirstOrDefaultAsync();

            if (entity == null)
            {
                return NotFound();
            }

            var entry = _db.Remove(entity);

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
