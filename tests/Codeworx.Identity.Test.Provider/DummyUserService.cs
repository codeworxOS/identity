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
            _users.Add(new DummyUserWithoutPassword());
            _users.Add(new MultiTenantDummyUser(_defaultTenantMultiTenantCache));
            _users.Add(new ForceChangePasswordUser());
        }

        public Task<Model.IUser> GetUserByExternalIdAsync(string provider, string nameIdentifier)
        {
            return Task.FromResult<Model.IUser>(_users.FirstOrDefault(p => p.ExternalIdentifiers.TryGetValue(provider, out var identifier) && identifier == nameIdentifier));
        }

        public Task<Model.IUser> GetUserByIdentityAsync(ClaimsIdentity identity)
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
            void ResetPassword(string password);

            IDictionary<string, string> ExternalIdentifiers { get; }

            new int FailedLoginCount { get; set; }
        }

        public class DummyUser : IDummyUser
        {
            private bool _forceChangePassword;
            private string _password = Constants.DefaultAdminUserName;

            public DummyUser()
            {
                FailedLoginCount = 0;
            }

            public string DefaultTenantKey => null;

            public string Identity => Constants.DefaultAdminUserId;

            public string Name => Constants.DefaultAdminUserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public int FailedLoginCount { get; set; }

            public void ResetPassword(string password)
            {
                _forceChangePassword = false;
                _password = password;
            }
        }

        public class DummyUserWithoutPassword : IDummyUser
        {
            private bool _forceChangePassword;
            private string _password = null;

            public string DefaultTenantKey => null;

            public string Identity => Constants.NoPasswordUserId;

            public string Name => Constants.NoPasswordUserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public void ResetPassword(string password)
            {
                _forceChangePassword = false;
                _password = password;
            }
        }

        public class MultiTenantDummyUser : IDummyUser
        {
            private bool _forceChangePassword;
            private string _password = Constants.MultiTenantUserName;
            public MultiTenantDummyUser(string defaultTenantKey = null)
            {
                FailedLoginCount = 0;
                this.DefaultTenantKey = defaultTenantKey;
            }

            public string DefaultTenantKey { get; }

            public string Identity => Constants.MultiTenantUserId;

            public string Name => Constants.MultiTenantUserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public int FailedLoginCount { get; set; }

            public bool ForceChangePassword => _forceChangePassword;

            public void ResetPassword(string password)
            {
                _password = password;
                _forceChangePassword = false;
            }
        }

        public class ForceChangePasswordUser : IDummyUser
        {
            private bool _forceChangePassword = true;
            private string _password = Constants.ForcePasswordUserName;

            public ForceChangePasswordUser()
            {
                FailedLoginCount = 0;
            }

            public string DefaultTenantKey => null;

            public string Identity => Constants.ForcePasswordUserId;

            public string Name => Constants.ForcePasswordUserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedProviders { get; } = ImmutableList<string>.Empty;

            public int FailedLoginCount { get; set; }

            public void ResetPassword(string password)
            {
                _password = password;
                _forceChangePassword = false;
            }
        }
    }
}