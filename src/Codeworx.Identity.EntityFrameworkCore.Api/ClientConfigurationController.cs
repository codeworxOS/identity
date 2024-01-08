using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Api.Model;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
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
            var configurations = await _db.Context.Set<ClientConfiguration>().Select(p => new ClientConfigurationInfoData
            {
                Id = p.Id,
                AccessTokenType = p.AccessTokenType,
                AccessTokenTypeConfiguration = p.AccessTokenTypeConfiguration,
                AuthenticationMode = p.AuthenticationMode,
                ClientType = p.ClientType,
                TokenExpiration = p.TokenExpiration,
                User = p.UserId != null ? new UserInfoData { DisplayName = p.User.Name, Id = p.User.Id } : null,
                Scopes = p.ScopeAssignments.Select(t => new ScopeInfoData { Id = t.ScopeId, DisplayName = t.Scope.ScopeKey }).ToList(),
                ValidRedirectUrls = p.ValidRedirectUrls.Select(t => t.Url).ToList(),
                HasClientSecret = p.ClientSecretHash != null,
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
                User = p.UserId != null ? new UserInfoData { DisplayName = p.User.Name, Id = p.User.Id } : null,
                ClientType = p.ClientType,
                Scopes = p.ScopeAssignments.Select(t => new ScopeInfoData { Id = t.ScopeId, DisplayName = t.Scope.ScopeKey }).ToList(),
                ValidRedirectUrls = p.ValidRedirectUrls.Select(t => t.Url).ToList(),
                HasClientSecret = p.ClientSecretHash != null,
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

            var configurations = await _db.Context.Set<ClientConfiguration>().Where(p => p.UserId == userId).Select(p => new ClientConfigurationInfoData
            {
                Id = p.Id,
                AccessTokenType = p.AccessTokenType,
                AccessTokenTypeConfiguration = p.AccessTokenTypeConfiguration,
                AuthenticationMode = p.AuthenticationMode,
                ClientType = p.ClientType,
                TokenExpiration = p.TokenExpiration,
                User = p.UserId != null ? new UserInfoData { DisplayName = p.User.Name, Id = p.User.Id } : null,
                Scopes = p.ScopeAssignments.Select(t => new ScopeInfoData { Id = t.ScopeId, DisplayName = t.Scope.ScopeKey }).ToList(),
                ValidRedirectUrls = p.ValidRedirectUrls.Select(t => t.Url).ToList(),
                HasClientSecret = p.ClientSecretHash != null,
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
                AccessTokenType = configuration.AccessTokenType,
                AccessTokenTypeConfiguration = configuration.AccessTokenTypeConfiguration,
                AuthenticationMode = configuration.AuthenticationMode,
                ClientType = configuration.ClientType,
                TokenExpiration = configuration.TokenExpiration,
                UserId = configuration.User?.Id,
            };

            string secret = null;
            if (configuration.ClientType == ClientType.Web || configuration.ClientType == ClientType.ApiKey || configuration.ClientType == ClientType.Backend)
            {
                secret = GenerateClientSecret();
                entity.ClientSecretHash = _hashingProvider.Create(secret);
            }

            _db.Context.Add(entity);
            await _db.Context.SaveChangesAsync().ConfigureAwait(false);

            var result = await GetClientConfigurationByIdAsync(entity.Id);
            result.ClientSecret = secret;
            return result;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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
            entity.UserId = configuration.User.Id;

            await _db.Context.SaveChangesAsync().ConfigureAwait(false);

            return await GetClientConfigurationByIdAsync(configuration.Id);
        }

        [HttpPut("{id:guid}/reset-secret")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ClientConfigurationInfoData> ResetClientSecretAsync(Guid id)
        {
            var entity = await _db.Context.Set<ClientConfiguration>().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                // TODO return 404
                throw new NotImplementedException();
            }

            if (entity.ClientType != ClientType.Web && entity.ClientType == ClientType.ApiKey && entity.ClientType == ClientType.Backend)
            {
                // TODO return 409
                throw new NotImplementedException();
            }

            string secret = GenerateClientSecret();
            entity.ClientSecretHash = _hashingProvider.Create(secret);
            await _db.Context.SaveChangesAsync().ConfigureAwait(false);

            var result = await GetClientConfigurationByIdAsync(entity.Id);
            result.ClientSecret = secret;
            return result;
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
        public async Task<ValidRedirectUrlInfoData> InsertValidRedirectUrlsAsync(Guid id, [FromBody] string url)
        {
            if (!await _db.Context.Set<ClientConfiguration>().AnyAsync(x => x.Id == id))
            {
                // TODO return 404;
                throw new NotSupportedException();
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                // TODO return 400
                throw new NotSupportedException();
            }

            var existingEntries = await _db.Context.Set<ValidRedirectUrl>().Where(t => t.ClientId == id).AsNoTracking().ToListAsync();

            var entity = existingEntries.FirstOrDefault(d => d.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase));
            if (entity == null)
            {
                entity = new ValidRedirectUrl
                {
                    Id = Guid.NewGuid(),
                    ClientId = id,
                    Url = url,
                };

                _db.Context.Add(entity);
                await _db.Context.SaveChangesAsync().ConfigureAwait(false);
            }

            return await GetValidRedirectUrlByIdAsync(id, entity.Id);
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

        private string GenerateClientSecret()
        {
            string uniq = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace('+', '-').Replace('/', '_')[..^1];
            return uniq;
        }
    }
}
