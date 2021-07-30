using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Api.Model;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Api
{
    [Route("api/identity/groups")]
    public class GroupController
    {
        private readonly IContextWrapper _db;

        public GroupController(IContextWrapper db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<GroupData> InsertUsersAsync([FromBody] GroupInsertData group)
        {
            var entity = new Group
            {
                Id = Guid.NewGuid(),
                Name = group.Name,
            };

            _db.Context.Add(entity);
            var entry = _db.Context.Entry(entity);

            entry.UpdateAdditionalProperties(group);

            await _db.Context.SaveChangesAsync();

            return await GetGroupByIdAsync(entity.Id);
        }

        [HttpGet("{id}/member")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<MemberInfoData>> GetMembersAsync(Guid id)
        {
            var groupExists = await _db.Context.Set<Group>().Where(p => p.Id == id).AnyAsync();

            if (!groupExists)
            {
                // TODO return 404;
            }

            var result = await _db.Context.Set<RightHolderGroup>()
                                .Where(p => p.GroupId == id)
                                .Select(p => new MemberInfoData
                                {
                                    Id = p.RightHolder.Id,
                                    Name = p.RightHolder.Name,
                                    MemberType = p.RightHolder is User ? MemberType.User : MemberType.Group,
                                })
                                .ToListAsync();

            return result;
        }

        [HttpPut("{id}/member/{memberId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task AddMemberAsync(Guid id, Guid memberId)
        {
            using (var transaction = await _db.Context.Database.BeginTransactionAsync())
            {
                var groupExists = await _db.Context.Set<Group>().Where(p => p.Id == id).AnyAsync();
                var memberExists = await _db.Context.Set<RightHolder>().Where(p => p.Id == memberId).AnyAsync();

                if (!groupExists || !memberExists)
                {
                    // TODO return 404;
                }

                var entity = new RightHolderGroup { GroupId = id, RightHolderId = memberId };

                _db.Context.Add(entity);

                await _db.Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }

        [HttpDelete("{id}/member/{memberId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task RemoveMemberAsync(Guid id, Guid memberId)
        {
            using (var transaction = await _db.Context.Database.BeginTransactionAsync())
            {
                var assignment = await _db.Context.Set<RightHolderGroup>().Where(p => p.GroupId == id && p.RightHolderId == memberId).FirstOrDefaultAsync();

                if (assignment == null)
                {
                    // TODO return 404;
                }

                _db.Context.Remove(assignment);

                await _db.Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<GroupData> UpdateUsersAsync([FromBody] GroupListData group)
        {
            var entity = await _db.Context.Set<Group>().Where(p => p.Id == group.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                // TODO return 404;
            }

            entity.Name = group.Name;

            var entry = _db.Context.Entry(entity);

            entry.UpdateAdditionalProperties(group);

            await _db.Context.SaveChangesAsync();

            return await GetGroupByIdAsync(entity.Id);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task DeleteGroupAsync(Guid id)
        {
            var entity = await _db.Context.Set<Group>().Where(p => p.Id == id).FirstOrDefaultAsync();

            if (entity == null)
            {
                // TODO return 404;
            }

            var entry = _db.Context.Remove(entity);

            await _db.Context.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<IEnumerable<GroupListData>> GetGroupsAsync()
        {
            IQueryable<Group> query = _db.Context.Set<Group>();

            var groups = await query.ToListAsync();
            var result = new List<GroupListData>();

            foreach (var group in groups)
            {
                var data = new GroupListData
                {
                    Id = group.Id,
                    Name = group.Name,
                };

                _db.Context.Entry(group).MapAdditionalProperties(data);

                result.Add(data);
            }

            return result;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<GroupData> GetGroupByIdAsync(Guid id, [FromQuery] string expand = null)
        {
            var set = _db.Context.Set<Group>();
            var group = await set.Where(p => p.Id == id).FirstOrDefaultAsync();

            var expands = expand?.Split(",")?.Select(p => p.Trim())?.ToList() ?? new List<string>();

            if (group == null)
            {
                // TODO return 404;
            }

            var result = new List<GroupListData>();

            var entry = _db.Context.Entry(group);
            var data = new GroupData
            {
                Id = group.Id,
                Name = group.Name,
            };

            if (expands.Contains(nameof(GroupData.MemberOf).ToLower()))
            {
                var groupInfos = await _db.Context.Set<RightHolderGroup>()
                    .Where(p => p.RightHolderId == group.Id)
                    .Select(p => new GroupInfoData
                    {
                        Id = p.Group.Id,
                        Name = p.Group.Name,
                    }).ToListAsync();

                foreach (var parent in groupInfos)
                {
                    data.MemberOf.Add(parent);
                }
            }

            entry.MapAdditionalProperties(data);

            return data;
        }
    }
}
