using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Cryptography.Argon2;
using Codeworx.Identity.EntityFrameworkCore.Api.Model;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Codeworx.Identity.EntityFrameworkCore.Api
{
    [Route("api/identity/clients")]
    [Authorize(Policy = Policies.Admin)]
    public class ClientConfigurationController
    {
        private readonly IContextWrapper _db;
        private readonly IHashingProvider _hashingProvider;
        private readonly ISecretGenerator _secretGenerator;
        private readonly ClientConfigurationOptions _options;

        public ClientConfigurationController(IContextWrapper db, IHashingProvider hashingProvider, ISecretGenerator secretGenerator, IOptionsSnapshot<ClientConfigurationOptions> options)
        {
            _db = db;
            _hashingProvider = hashingProvider;
            _secretGenerator = secretGenerator;
            _options = options.Value;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<ClientConfigurationInfoData>> GetClientConfigurationsAsync()
        {
            var data = await _db.Context.Set<ClientConfiguration>().Select(p => new
            {
                p.Id,
                p.AccessTokenType,
                p.AccessTokenTypeConfiguration,
                p.AuthenticationMode,
                p.ClientType,
                p.TokenExpiration,
                User = p.UserId != null ? new UserInfoData { DisplayName = p.User.Name, Id = p.User.Id } : null,
                HasClientSecret = p.ClientSecretHash != null,
            }).ToListAsync();

            var scopes = await _db.Context.Set<ScopeAssignment>().Select(d => new { d.ClientId, Data = new ScopeInfoData { Id = d.ScopeId, DisplayName = d.Scope.ScopeKey } }).ToListAsync();
            var redirectUrls = await _db.Context.Set<ValidRedirectUrl>().Select(d => new { d.ClientId, Data = new ValidRedirectUrlInfoData { Id = d.Id, Url = d.Url } }).ToListAsync();

            var configurations = data.Select(d => new ClientConfigurationInfoData
            {
                Id = d.Id,
                AccessTokenType = d.AccessTokenType,
                AuthenticationMode = d.AuthenticationMode,
                ClientType = d.ClientType,
                TokenExpiration = d.TokenExpiration,
                Scopes = scopes.Where(c => c.ClientId == d.Id).Select(c => c.Data).ToList(),
                ValidRedirectUrls = redirectUrls.Where(c => c.ClientId == d.Id).Select(c => c.Data).ToList(),
                User = d.User,
                HasClientSecret = d.HasClientSecret,
                AccessTokenTypeConfiguration = d.AccessTokenTypeConfiguration != null ? JsonConvert.DeserializeObject<Dictionary<string, object>>(d.AccessTokenTypeConfiguration) : null,
            });

            return configurations;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ClientConfigurationInfoData> GetClientConfigurationByIdAsync(Guid id)
        {
            var data = await _db.Context.Set<ClientConfiguration>().Where(p => p.Id == id).Select(p => new
            {
                p.Id,
                p.AccessTokenType,
                p.AccessTokenTypeConfiguration,
                p.AuthenticationMode,
                p.ClientType,
                p.TokenExpiration,
                User = p.UserId != null ? new UserInfoData { DisplayName = p.User.Name, Id = p.User.Id } : null,
                HasClientSecret = p.ClientSecretHash != null,
            }).FirstOrDefaultAsync();

            if (data == null)
            {
                // TODO return 404;
                throw new NotImplementedException();
            }

            var scopes = await _db.Context.Set<ScopeAssignment>().Where(d => d.ClientId == id).Select(d => new ScopeInfoData { Id = d.ScopeId, DisplayName = d.Scope.ScopeKey }).ToListAsync();
            var redirectUrls = await _db.Context.Set<ValidRedirectUrl>().Where(d => d.ClientId == id).Select(d => new ValidRedirectUrlInfoData { Id = d.Id, Url = d.Url }).ToListAsync();

            var configuration = new ClientConfigurationInfoData
            {
                Id = data.Id,
                AccessTokenType = data.AccessTokenType,
                AuthenticationMode = data.AuthenticationMode,
                ClientType = data.ClientType,
                Scopes = scopes,
                TokenExpiration = data.TokenExpiration,
                ValidRedirectUrls = redirectUrls,
                User = data.User,
                HasClientSecret = data.HasClientSecret,
                AccessTokenTypeConfiguration = data.AccessTokenTypeConfiguration != null ? JsonConvert.DeserializeObject<Dictionary<string, object>>(data.AccessTokenTypeConfiguration) : null,
            };

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

            var data = await _db.Context.Set<ClientConfiguration>().Where(p => p.UserId == userId).Select(p => new
            {
                p.Id,
                p.AccessTokenType,
                p.AccessTokenTypeConfiguration,
                p.AuthenticationMode,
                p.ClientType,
                p.TokenExpiration,
                User = p.UserId != null ? new UserInfoData { DisplayName = p.User.Name, Id = p.User.Id } : null,
                HasClientSecret = p.ClientSecretHash != null,
            }).ToListAsync();

            var scopes = await _db.Context.Set<ScopeAssignment>().Where(p => p.Client.UserId == userId).Select(d => new { d.ClientId, Data = new ScopeInfoData { Id = d.ScopeId, DisplayName = d.Scope.ScopeKey } }).ToListAsync();
            var redirectUrls = await _db.Context.Set<ValidRedirectUrl>().Where(p => p.Client.UserId == userId).Select(d => new { d.ClientId, Data = new ValidRedirectUrlInfoData { Id = d.Id, Url = d.Url } }).ToListAsync();

            var configurations = data.Select(d => new ClientConfigurationInfoData
            {
                Id = d.Id,
                AccessTokenType = d.AccessTokenType,
                AuthenticationMode = d.AuthenticationMode,
                ClientType = d.ClientType,
                TokenExpiration = d.TokenExpiration,
                Scopes = scopes.Where(c => c.ClientId == d.Id).Select(c => c.Data).ToList(),
                ValidRedirectUrls = redirectUrls.Where(c => c.ClientId == d.Id).Select(c => c.Data).ToList(),
                User = d.User,
                HasClientSecret = d.HasClientSecret,
                AccessTokenTypeConfiguration = d.AccessTokenTypeConfiguration != null ? JsonConvert.DeserializeObject<Dictionary<string, object>>(d.AccessTokenTypeConfiguration) : null,
            });

            return configurations;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ClientConfigurationInfoData> InsertClientConfigurationAsync([FromBody] ClientConfigurationInsertData configuration)
        {
            var entity = new ClientConfiguration
            {
                Id = Guid.NewGuid(),
                AuthenticationMode = configuration.AuthenticationMode,
                ClientType = configuration.ClientType,
                TokenExpiration = configuration.TokenExpiration,
                UserId = configuration.User?.Id,
            };

            if (!string.IsNullOrEmpty(configuration.AccessTokenType))
            {
                string config = null;
                ////if (_configurationTypeLookup.TryGetValue(configuration.AccessTokenType, out var configType))
                ////{
                ////    config = GetSerializedConfig(configuration.AccessTokenTypeConfiguration, configType);
                ////}
                ////else
                ////{
                ////    // TODO return 404;
                ////    throw new NotImplementedException();
                ////}

                entity.AccessTokenType = configuration.AccessTokenType;
                entity.AccessTokenTypeConfiguration = config;
            }

            string secret = null;
            if (configuration.ClientType == ClientType.Web || configuration.ClientType == ClientType.ApiKey || configuration.ClientType == ClientType.Backend)
            {
                secret = _secretGenerator.Create(_options.SecretLength);
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ClientConfigurationInfoData> UpdateClientConfigurationAsync([FromBody] ClientConfigurationUpdateData configuration)
        {
            var entity = await _db.Context.Set<ClientConfiguration>().FirstOrDefaultAsync(x => x.Id == configuration.Id);

            if (entity == null)
            {
                // TODO return 404
                throw new NotImplementedException();
            }

            if (!string.IsNullOrEmpty(configuration.AccessTokenType))
            {
                string config = null;
                ////if (_configurationTypeLookup.TryGetValue(configuration.AccessTokenType, out var configType))
                ////{
                ////    config = GetSerializedConfig(configuration.AccessTokenTypeConfiguration, configType);
                ////}
                ////else
                ////{
                ////    // TODO return 404;
                ////    throw new NotImplementedException();
                ////}

                entity.AccessTokenType = configuration.AccessTokenType;
                entity.AccessTokenTypeConfiguration = config;
            }
            else
            {
                entity.AccessTokenType = null;
                entity.AccessTokenTypeConfiguration = null;
            }

            entity.AuthenticationMode = configuration.AuthenticationMode;
            entity.ClientType = configuration.ClientType;
            entity.TokenExpiration = configuration.TokenExpiration;
            entity.UserId = configuration.User?.Id;

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

            string secret = _secretGenerator.Create(_options.SecretLength);
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
    }
}
