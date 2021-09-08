using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Test
{
    public class DummyUserService : IUserService, IDefaultTenantService
    {
        private static string _defaultTenantMultiTenantCache;

        private List<IDummyUser> _users;

        public List<IDummyUser> Users => _users;

        public DummyUserService()
        {
            _users = new List<IDummyUser>();
            _users.Add(new DummyUser());
            _users.Add(new MultiTenantDummyUser(_defaultTenantMultiTenantCache));
            _users.Add(new ForceChangePasswordUser());
        }

        public Task<Model.IUser> GetUserByExternalIdAsync(string provider, string nameIdentifier)
        {
            return Task.FromResult<Model.IUser>(_users.FirstOrDefault(p => p.ExternalIdentifiers.TryGetValue(provider, out var identifier) && identifier == nameIdentifier));
        }

        public Task<Model.IUser> GetUserByIdentifierAsync(ClaimsIdentity identity)
        {
            var id = Guid.Parse(identity.GetUserId());
            return Task.FromResult<IUser>(_users.FirstOrDefault(p => Guid.Parse(p.Identity) == id));
        }

        public Task<Model.IUser> GetUserByNameAsync(string userName)
        {
            return Task.FromResult<IUser>(_users.FirstOrDefault(p => p.Name == userName));
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

        public Task<IUser> GetUserByIdAsync(string userId)
        {
            var id = Guid.Parse(userId);
            return Task.FromResult<IUser>(_users.FirstOrDefault(p => Guid.Parse(p.Identity) == id));
        }

        public interface IDummyUser : IUser
        {
            IDictionary<string, string> ExternalIdentifiers { get; }
        }

        public class DummyUser : IDummyUser
        {
            public string DefaultTenantKey => null;

            public string Identity => Constants.DefaultAdminUserId;

            public string Name => Constants.DefaultAdminUserName;

            public string PasswordHash => null;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public bool ForceChangePassword => false;

            public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();
        }

        public class MultiTenantDummyUser : IDummyUser
        {
            public MultiTenantDummyUser(string defaultTenantKey = null)
            {
                this.DefaultTenantKey = defaultTenantKey;
            }

            public string DefaultTenantKey { get; }

            public string Identity => Constants.MultiTenantUserId;

            public string Name => Constants.MultiTenantUserName;

            public string PasswordHash => null;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public bool ForceChangePassword => false;
        }

        public class ForceChangePasswordUser : IDummyUser
        {
            public string DefaultTenantKey => null;

            public string Identity => Constants.ForcePasswordUserId;

            public string Name => Constants.ForcePasswordUserName;

            public string PasswordHash => null;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public bool ForceChangePassword => true;
        }
    }
}