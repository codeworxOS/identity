using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Api.Model;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Api
{
    [Route("api/identity/login")]
    [Authorize(Policy = Policies.Admin)]
    public class LoginController
    {
        private readonly IContextWrapper _db;
        private readonly Dictionary<string, Type> _configurationTypeLookup;

        public LoginController(IContextWrapper db, IEnumerable<IProcessorTypeLookup> processorTypeLookups)
        {
            _db = db;
            _configurationTypeLookup = processorTypeLookups.ToDictionary(p => p.Key, p => p.ConfigurationType);
        }

        [HttpGet]
        public async Task<IEnumerable<LoginRegistrationListData>> GetLoginRegistrationsAsync(bool includeDisabled = false)
        {
            IQueryable<AuthenticationProvider> query = _db.Context.Set<AuthenticationProvider>();

            if (!includeDisabled)
            {
                query = query.Where(p => !p.IsDisabled);
            }

            var result = await query.OrderBy(p => p.SortOrder).Select(p => new LoginRegistrationListData
            {
                Id = p.Id,
                DisplayName = p.Name,
                Processor = p.EndpointType,
                Disabled = p.IsDisabled,
            }).ToListAsync();

            return result;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(LoginRegistrationData), StatusCodes.Status200OK)]
        public async Task<LoginRegistrationData> GetLoginRegistrationAsync(Guid id)
        {
            IQueryable<AuthenticationProvider> query = _db.Context.Set<AuthenticationProvider>();
            query = query.Where(p => p.Id == id);

            var data = await query.Select(p => new
            {
                Id = p.Id,
                DisplayName = p.Name,
                SortOrder = p.SortOrder,
                Processor = p.EndpointType,
                ProcessorConfiguration = p.EndpointConfiguration,
                Disabled = p.IsDisabled,
            }).FirstOrDefaultAsync();

            if (data == null)
            {
                // TODO return 404;
                return null;
            }

            Dictionary<string, object> config = null;

            if (data.ProcessorConfiguration != null)
            {
                config = JsonConvert.DeserializeObject<Dictionary<string, object>>(data.ProcessorConfiguration);
            }

            var result = new LoginRegistrationData
            {
                Id = data.Id,
                Disabled = data.Disabled,
                DisplayName = data.DisplayName,
                Processor = data.Processor,
                Configuration = config,
                SortOrder = data.SortOrder,
            };

            return result;
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task DeleteLoginRegistrationAsync(Guid id, [FromQuery] bool unlinkUsers = false)
        {
            IQueryable<AuthenticationProvider> query = _db.Context.Set<AuthenticationProvider>();
            query = query.Where(p => p.Id == id);

            var entity = await query.FirstOrDefaultAsync();

            if (entity == null)
            {
                // TODO return 404;
                return;
            }

            if (unlinkUsers)
            {
                var data = await _db.Context.Set<AuthenticationProviderRightHolder>().Where(p => p.ProviderId == id).ToListAsync().ConfigureAwait(false);
                _db.Context.RemoveRange(data);
            }
            else
            {
                if (await _db.Context.Set<AuthenticationProviderRightHolder>().Where(p => p.ProviderId == id).AnyAsync().ConfigureAwait(false))
                {
                    // TODO return 409
                    return;
                }
            }

            _db.Context.Remove(entity);

            try
            {
                await _db.Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // TODO return 409;
            }
        }

        [HttpGet("/api/identity/users/{userId:guid}/login")]
        public async Task<IEnumerable<LoginProviderAssignmentData>> GetLoginProviderAssignmentsAsync(Guid userId)
        {
            var data = await _db.Context.Set<AuthenticationProviderRightHolder>()
                .Where(p => p.RightHolderId == userId)
                .OrderBy(p => p.Provider.SortOrder)
                .Select(p => new LoginProviderAssignmentData
                {
                    ProviderId = p.ProviderId,
                    Identifier = p.ExternalIdentifier,
                }).ToListAsync()
                .ConfigureAwait(false);

            return data;
        }

        [HttpGet("/api/identity/users/{userId:guid}/login/{providerId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<string> GetLoginProviderAssignmentAsync(Guid userId, Guid providerId)
        {
            var data = await _db.Context.Set<AuthenticationProviderRightHolder>()
                .Where(p => p.RightHolderId == userId && p.ProviderId == providerId)
                .Select(p => p.ExternalIdentifier).ToListAsync()
                .ConfigureAwait(false);

            if (data.Count > 0)
            {
                return data[0];
            }
            else
            {
                // TODO return 404
                return null;
            }
        }

        [HttpPut("/api/identity/users/{userId:guid}/login/{providerId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task UpdateLoginProviderAssignmentAsync(Guid userId, Guid providerId, [FromBody] string identifier)
        {
            await using (var transaction = await _db.Context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var data = await _db.Context.Set<AuthenticationProviderRightHolder>()
                    .Where(p => p.RightHolderId == userId && p.ProviderId == providerId)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (data.Count > 0)
                {
                    data[0].ExternalIdentifier = identifier;
                }
                else
                {
                    var userExists = await _db.Context.Set<RightHolder>().AnyAsync(p => p.Id == userId);
                    var providerExists = await _db.Context.Set<AuthenticationProvider>().AnyAsync(p => p.Id == providerId);

                    if (!userExists || !providerExists)
                    {
                        // TODO return 404
                        return;
                    }

                    var entity = new AuthenticationProviderRightHolder { RightHolderId = userId, ProviderId = providerId, ExternalIdentifier = identifier };
                    _db.Context.Add(entity);
                }

                try
                {
                    await _db.Context.SaveChangesAsync().ConfigureAwait(false);
                    await transaction.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // TODO return 409
                    return;
                }
            }
        }

        [HttpDelete("/api/identity/users/{userId:guid}/login/{providerId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task DelteLoginProviderAssignmentAsync(Guid userId, Guid providerId)
        {
            await using (var transaction = await _db.Context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var data = await _db.Context.Set<AuthenticationProviderRightHolder>()
                    .Where(p => p.RightHolderId == userId && p.ProviderId == providerId)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (data.Count > 0)
                {
                    _db.Context.Remove(data[0]);
                }
                else
                {
                    // TODO return 404
                    return;
                }

                await _db.Context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<LoginRegistrationData> CreateLoginRegistrationAsync([FromBody] LoginRegistrationInsertData loginRegistration)
        {
            await using (var transaction = await _db.Context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                string config = null;

                int sortOrder = 0;

                if (loginRegistration.SortOrder.HasValue)
                {
                    sortOrder = loginRegistration.SortOrder.Value;
                }
                else
                {
                    var current = await _db.Context.Set<AuthenticationProvider>().OrderByDescending(p => p.SortOrder).Select(p => p.SortOrder).FirstOrDefaultAsync().ConfigureAwait(false);
                    sortOrder = current + 1;
                }

                if (_configurationTypeLookup.TryGetValue(loginRegistration.Processor, out var configType))
                {
                    config = GetSerializedConfig(loginRegistration.Configuration, configType);
                }
                else
                {
                    // TODO return 404;
                    return null;
                }

                var entity = new AuthenticationProvider
                {
                    Id = Guid.NewGuid(),
                    Name = loginRegistration.DisplayName,
                    EndpointType = loginRegistration.Processor,
                    IsDisabled = loginRegistration.Disabled,
                    SortOrder = sortOrder,
                    EndpointConfiguration = config,
                };

                _db.Context.Add(entity);

                try
                {
                    await _db.Context.SaveChangesAsync().ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);
                    return await GetLoginRegistrationAsync(entity.Id).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // TODO return 409
                    return null;
                }
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<LoginRegistrationData> UpdateLoginRegistrationAsync([FromBody] LoginRegistrationData loginRegistration)
        {
            await using (var transaction = await _db.Context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                string config = null;

                if (_configurationTypeLookup.TryGetValue(loginRegistration.Processor, out var configType))
                {
                    config = GetSerializedConfig(loginRegistration.Configuration, configType);
                }
                else
                {
                    // TODO return 404
                    return null;
                }

                var entity = await _db.Context.Set<AuthenticationProvider>().Where(p => p.Id == loginRegistration.Id).FirstOrDefaultAsync().ConfigureAwait(false);

                if (entity == null)
                {
                    // TODO return 404
                    return null;
                }

                entity.Name = loginRegistration.DisplayName;
                entity.EndpointType = loginRegistration.Processor;
                entity.IsDisabled = loginRegistration.Disabled;
                entity.SortOrder = loginRegistration.SortOrder;
                entity.EndpointConfiguration = config;

                try
                {
                    await _db.Context.SaveChangesAsync().ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);
                    return await GetLoginRegistrationAsync(entity.Id).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // TODO return 409
                    return null;
                }
            }
        }

        private static string GetSerializedConfig(Dictionary<string, object> configuration, Type configType)
        {
            string config = null;
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            if (configType != null && configuration != null)
            {
                config = JsonConvert.SerializeObject(configuration);
                var deserializedConfig = JsonConvert.DeserializeObject(config, configType);
                config = JsonConvert.SerializeObject(deserializedConfig, settings);
            }

            return config;
        }
    }
}
