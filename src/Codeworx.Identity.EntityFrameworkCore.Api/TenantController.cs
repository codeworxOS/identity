using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Api.Model;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Api
{
    [Route("api/identity/tenants")]
    [Authorize(Policy = Policies.Admin)]
    public class TenantController
    {
        private readonly IContextWrapper _db;

        public TenantController(IContextWrapper db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<TenantListData> InsertTenantAsync([FromBody] TenantInsertData tenant)
        {
            var entity = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = tenant.Name,
            };
            _db.Context.Add(entity);

            var entry = _db.Context.Entry(entity);

            entry.UpdateAdditionalProperties(tenant);

            await _db.Context.SaveChangesAsync();

            var result = new TenantListData { Id = entity.Id, Name = entity.Name };
            entry.MapAdditionalProperties(result);

            return result;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<TenantListData> UpdateTenantAsync([FromBody] TenantListData tenant)
        {
            var entity = await _db.Context.Set<Tenant>().Where(p => p.Id == tenant.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                // TODO return 404;
            }

            entity.Name = tenant.Name;

            var entry = _db.Context.Entry(entity);
            entry.UpdateAdditionalProperties(tenant);

            await _db.Context.SaveChangesAsync();

            var result = new TenantListData
            {
                Id = entity.Id,
                Name = entity.Name,
            };

            entry.MapAdditionalProperties(result);

            return result;
        }

        [HttpGet]
        public async Task<IEnumerable<TenantListData>> GetTenantsAsync()
        {
            IQueryable<Tenant> query = _db.Context.Set<Tenant>();

            var tenants = await query.ToListAsync();
            var result = new List<TenantListData>();

            foreach (var tenant in tenants)
            {
                var data = new TenantListData
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                };

                _db.Context.Entry(tenant).MapAdditionalProperties(data);

                result.Add(data);
            }

            return result;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<TenantListData> GetTenantById(Guid id)
        {
            IQueryable<Tenant> query = _db.Context.Set<Tenant>();

            var tenant = await query.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (tenant == null)
            {
                // TODO return 404
            }

            var data = new TenantListData
            {
                Id = tenant.Id,
                Name = tenant.Name,
            };

            _db.Context.Entry(tenant).MapAdditionalProperties(data);

            return data;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task DeleteTenantById(Guid id)
        {
            IQueryable<Tenant> query = _db.Context.Set<Tenant>();

            var tenant = await query.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (tenant == null)
            {
                // TODO return 404
            }

            _db.Context.Remove(tenant);

            await _db.Context.SaveChangesAsync();
        }
    }
}
