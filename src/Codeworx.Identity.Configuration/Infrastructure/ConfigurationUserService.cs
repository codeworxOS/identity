using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration.Model;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Configuration.Infrastructure
{
    public class ConfigurationUserService : IUserService, IDisposable
    {
        private readonly IDisposable _changeToken;
        private bool _disposedValue;
        private UserConfigOptions _options;

        public ConfigurationUserService(IOptionsMonitor<UserConfigOptions> options)
        {
            _options = options.CurrentValue;
            _changeToken = options.OnChange(p => _options = p);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task<IUser> GetUserByExternalIdAsync(string provider, string nameIdentifier)
        {
            return Task.FromResult<IUser>(null);
        }

        public Task<IUser> GetUserByIdAsync(string userId)
        {
            User result = null;

            if (_options.TryGetValue(userId, out var userConfig))
            {
                result = new User(userId, userId, userConfig.Password, new string[] { });
            }

            return Task.FromResult<IUser>(result);
        }

        public async Task<IUser> GetUserByIdentifierAsync(ClaimsIdentity user)
        {
            var id = user.GetUserId();

            return await GetUserByIdAsync(id);
        }

        public Task<IUser> GetUserByNameAsync(string userName)
        {
            User result = null;

            if (_options.TryGetValue(userName, out var userConfig))
            {
                result = new User(userName, userName, userConfig.Password, new string[] { });
            }

            return Task.FromResult<IUser>(result);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _changeToken.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
