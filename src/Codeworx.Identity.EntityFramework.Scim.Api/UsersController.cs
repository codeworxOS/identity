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
    [Route("authProvider/{authProviderId}/scim/Users")]
    [Authorize(Policy = "api_none")]
    public class UsersController
    {
        private DbContext _db;

        public UsersController(IContextWrapper contextWrapper)
        {
            _db = contextWrapper.Context;
        }

        [HttpGet]
        public async Task<ListResponse<UserResponse>> GetUsersAsync([FromQuery] int startIndex, [FromQuery] int count)
        {
            if (startIndex < 1)
            {
                startIndex = 1;
            }

            var query = _db.Set<User>().AsQueryable().AsNoTracking().OrderBy(c => c.Id);
            var totalResults = await query.CountAsync();

            var users = await query.ToListAsync();

            var result = new List<UserResponse>();

            foreach (var item in users)
            {
                var data = new UserResponse
                {
                    Id = item.Id,
                };

                data.AdditionalProperties.Add("Test", 1);

                result.Add(data);
            }

            return new ListResponse<UserResponse>(startIndex, totalResults, count, result);
        }
    }
}
