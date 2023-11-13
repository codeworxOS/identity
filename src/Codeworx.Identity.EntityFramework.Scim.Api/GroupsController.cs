using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("authProvider/{authProviderId}/scim/Groups")]
    [Authorize(Policy = "api_none")]
    public class GroupsController
    {
        private DbContext _db;

        public GroupsController(IContextWrapper contextWrapper)
        {
            _db = contextWrapper.Context;
        }

        [HttpGet]
        public async Task<ListResponse<GroupResponse>> GetUsersAsync([FromRoute] Guid authProviderId)
        {
            var query = _db.Set<Group>().AsQueryable().AsNoTracking().OrderBy(c => c.Id);

            var users = await query.ToListAsync();
            var result = new List<GroupResponse>();

            foreach (var item in users)
            {
                var data = new GroupResponse
                {
                    Id = item.Id,
                };

                result.Add(data);
            }

            var startIndex = 0;
            var itemsPerPage = 0;
            var totalResults = 0;

            return new ListResponse<GroupResponse>(startIndex, totalResults, itemsPerPage, result);
        }
    }
}
