using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Api.Model;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Api
{
    [Route("api/identity/clients")]
    [Authorize(Policy = Policies.Admin)]
    public class ClientConfigurationController
    {
        private readonly IContextWrapper _db;
        private readonly IHashingProvider _hashingProvider;

        public ClientConfigurationController(IContextWrapper db, IHashingProvider hashingProvider)
        {
            _db = db;
            _hashingProvider = hashingProvider;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<ClientConfigurationInfoData>> GetClientConfigurationsAsync()
        {
            var configurations = await _db.Context.Set<ClientConfiguration>().Select(t => new ClientConfigurationInfoData
            {
                Id = t.Id,
                AccessTokenType = t.AccessTokenType,
                AccessTokenTypeConfiguration = t.AccessTokenTypeConfiguration,
                AuthenticationMode = t.AuthenticationMode,
                ClientType = t.ClientType,
                TokenExpiration = t.TokenExpiration,
                UserId = t.UserId,
                Scopes = t.ScopeAssignments.Select(t => t.ScopeId).ToList(),
                ValidRedirectUrls = t.ValidRedirectUrls.Select(t => t.Url).ToList(),
            }).ToListAsync();

            return configurations;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ClientConfigurationInfoData> GetClientConfigurationByIdAsync(Guid id)
        {
            var configuration = await _db.Context.Set<ClientConfiguration>().Where(p => p.Id == id).Select(p => new ClientConfigurationInfoData
            {
                Id = p.Id,
                AccessTokenType = p.AccessTokenType,
                AccessTokenTypeConfiguration = p.AccessTokenTypeConfiguration,
                AuthenticationMode = p.AuthenticationMode,
                TokenExpiration = p.TokenExpiration,
                UserId = p.UserId,
                ClientType = p.ClientType,
                Scopes = p.ScopeAssignments.Select(t => t.ScopeId).ToList(),
                ValidRedirectUrls = p.ValidRedirectUrls.Select(t => t.Url).ToList(),
            }).FirstOrDefaultAsync();

            if (configuration == null)
            {
                // TODO return 404;
                throw new NotImplementedException();
            }

            return configuration;
        }

        [HttpGet("byuser/{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<ClientConfigurationInfoData>> GetClientConfigurationByUserIdAsync(Guid userId)
        {
            if (!await _db.Context.Set<User>().AnyAsync(p => p.Id == userId))
            {
                // TODO return 404;
                throw new NotImplementedException();
            }

            var configurations = await _db.Context.Set<ClientConfiguration>().Where(p => p.UserId == userId).Select(t => new ClientConfigurationInfoData
            {
                Id = t.Id,
                AccessTokenType = t.AccessTokenType,
                AccessTokenTypeConfiguration = t.AccessTokenTypeConfiguration,
                AuthenticationMode = t.AuthenticationMode,
                ClientType = t.ClientType,
                TokenExpiration = t.TokenExpiration,
                UserId = t.UserId,
                Scopes = t.ScopeAssignments.Select(t => t.ScopeId).ToList(),
                ValidRedirectUrls = t.ValidRedirectUrls.Select(t => t.Url).ToList(),
            }).ToListAsync();

            return configurations;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ClientConfigurationInfoData> InsertClientConfigurationAsync([FromBody] ClientConfigurationInsertData configuration)
        {
            var entity = new ClientConfiguration
            {
                Id = Guid.NewGuid(),
                ClientSecretHash = _hashingProvider.Create(configuration.Secret),
                AccessTokenType = configuration.AccessTokenType,
                AccessTokenTypeConfiguration = configuration.AccessTokenTypeConfiguration,
                AuthenticationMode = configuration.AuthenticationMode,
                ClientType = configuration.ClientType,
                TokenExpiration = configuration.TokenExpiration,
                UserId = configuration.UserId,
            };

            _db.Context.Add(entity);
            await _db.Context.SaveChangesAsync().ConfigureAwait(false);

            return await GetClientConfigurationByIdAsync(entity.Id);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ClientConfigurationInfoData> UpdateClientConfigurationAsync([FromBody] ClientConfigurationUpdateData configuration)
        {
            var entity = await _db.Context.Set<ClientConfiguration>().FirstOrDefaultAsync(x => x.Id == configuration.Id);

            if (entity == null)
            {
                // TODO return 404
                throw new NotImplementedException();
            }

            entity.AccessTokenType = configuration.AccessTokenType;
            entity.AccessTokenTypeConfiguration = configuration.AccessTokenTypeConfiguration;
            entity.AuthenticationMode = configuration.AuthenticationMode;
            entity.ClientType = configuration.ClientType;
            entity.TokenExpiration = configuration.TokenExpiration;
            entity.UserId = configuration.UserId;

            await _db.Context.SaveChangesAsync().ConfigureAwait(false);

            return await GetClientConfigurationByIdAsync(configuration.Id);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task DeleteClientConfigurationAsync(Guid id)
        {
            var entity = await _db.Context.Set<ClientConfiguration>().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                // TODO return 404
                throw new NotImplementedException();
            }

            _db.Context.Remove(entity);
            await _db.Context.SaveChangesAsync().ConfigureAwait(false);
        }

        [HttpGet("{id}/redirect-urls")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<ValidRedirectUrlInfoData>> GetValidRedirectUrlsAsync(Guid id)
        {
            if (!await _db.Context.Set<ClientConfiguration>().AnyAsync(x => x.Id == id))
            {
                // TODO return 404;
                throw new NotSupportedException();
            }

            var redirectUrls = await _db.Context.Set<ValidRedirectUrl>().Where(t => t.ClientId == id).Select(t => new ValidRedirectUrlInfoData
            {
                Id = t.Id,
                ClientId = t.ClientId,
                Url = t.Url,
            }).ToListAsync();

            return redirectUrls;
        }

        [HttpGet("{id}/redirect-urls/{urlId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ValidRedirectUrlInfoData> GetValidRedirectUrlByIdAsync(Guid id, Guid urlId)
        {
            if (!await _db.Context.Set<ClientConfiguration>().AnyAsync(x => x.Id == id))
            {
                // TODO return 404;
                throw new NotSupportedException();
            }

            var redirectUrl = await _db.Context.Set<ValidRedirectUrl>().Where(t => t.Id == urlId).Select(t => new ValidRedirectUrlInfoData
            {
                Id = t.Id,
                ClientId = t.ClientId,
                Url = t.Url,
            }).FirstOrDefaultAsync();

            if (redirectUrl == null)
            {
                // TODO return 404;
                throw new NotSupportedException();
            }

            return redirectUrl;
        }

        [HttpPost("{id}/redirect-urls")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<ValidRedirectUrlInfoData>> InsertValidRedirectUrlsAsync(Guid id, [FromBody] IEnumerable<string> urls)
        {
            if (!await _db.Context.Set<ClientConfiguration>().AnyAsync(x => x.Id == id))
            {
                // TODO return 404;
                throw new NotSupportedException();
            }

            var invalid = urls.Where(t => !Uri.IsWellFormedUriString(t, UriKind.Absolute)).ToList();
            if (invalid.Any())
            {
                // TODO return 400
                throw new NotSupportedException();
            }

            var existing = await _db.Context.Set<ValidRedirectUrl>().Where(t => t.ClientId == id).Select(t => t.Url).ToListAsync();

            var entities = urls.Where(t => !existing.Contains(t, StringComparer.InvariantCultureIgnoreCase)).Select(t => new ValidRedirectUrl
            {
                Id = Guid.NewGuid(),
                ClientId = id,
                Url = t,
            }).ToList();

            _db.Context.AddRange(entities);
            await _db.Context.SaveChangesAsync().ConfigureAwait(false);

            return await GetValidRedirectUrlsAsync(id);
        }

        [HttpDelete("{id}/redirect-urls/{urlId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task DeleteValidRedirectUrlAsync(Guid id, Guid urlId)
        {
            if (!await _db.Context.Set<ClientConfiguration>().AnyAsync(x => x.Id == id))
            {
                // TODO return 404;
                throw new NotSupportedException();
            }

            var entity = await _db.Context.Set<ValidRedirectUrl>().FirstOrDefaultAsync(x => x.Id == urlId);

            if (entity == null)
            {
                // TODO return 404;
                throw new NotSupportedException();
            }

            _db.Context.Remove(entity);
            await _db.Context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
