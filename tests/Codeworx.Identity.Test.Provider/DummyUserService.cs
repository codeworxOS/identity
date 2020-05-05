using Codeworx.Identity.Model;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.Test
{
    public class DummyUserService : IUserService, IDefaultTenantService
    {
        private static string _defaultTenantMultiTenantCache;

        public Task<IUser> GetUserByExternalIdAsync(string provider, string nameIdentifier)
        {
            if (provider == Constants.ExternalWindowsProviderId)
            {
                return Task.FromResult<IUser>(new MultiTenantDummyUser(_defaultTenantMultiTenantCache));
            }

            return Task.FromResult<IUser>(null);
        }

        public Task<IUser> GetUserByIdentifierAsync(ClaimsIdentity identity)
        {
            var id = Guid.Parse(identity.GetUserId());
            if (id == Guid.Parse(Constants.DefaultAdminUserId))
            {
                return Task.FromResult<IUser>(new DummyUser());
            }
            else if (id == Guid.Parse(Constants.MultiTenantUserId))
            {
                return Task.FromResult<IUser>(new MultiTenantDummyUser(_defaultTenantMultiTenantCache));
            }

            return Task.FromResult<IUser>(null);
        }

        public Task<IUser> GetUserByNameAsync(string userName)
        {
            if (userName.Equals(Constants.DefaultAdminUserName, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult<IUser>(new DummyUser());
            }
            else if (userName.Equals(Constants.MultiTenantUserName, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult<IUser>(new MultiTenantDummyUser(_defaultTenantMultiTenantCache));
            }

            return Task.FromResult<IUser>(null);
        }

        public Task SetDefaultTenantAsync(string identifier, string tenantKey)
        {
            var id = Guid.Parse(identifier);
            if (id == Guid.Parse(Constants.DefaultAdminUserId))
            {
                throw new KeyNotFoundException();
            }
            else if (id == Guid.Parse(Constants.MultiTenantUserId))
            {
                _defaultTenantMultiTenantCache = tenantKey;
            }

            return Task.CompletedTask;
        }

        private class DummyUser : IUser
        {
            public string DefaultTenantKey => null;

            public string Identity => Constants.DefaultAdminUserId;

            public string Name => Constants.DefaultAdminUserName;

            public byte[] PasswordHash => null;

            public byte[] PasswordSalt => null;
        }

        private class MultiTenantDummyUser : IUser
        {
            public MultiTenantDummyUser(string defaultTenantKey = null)
            {
                this.DefaultTenantKey = defaultTenantKey;
            }

            public string DefaultTenantKey { get; }

            public string Identity => Constants.MultiTenantUserId;

            public string Name => Constants.MultiTenantUserName;

            public byte[] PasswordHash => null;

            public byte[] PasswordSalt => null;
        }
    }
}