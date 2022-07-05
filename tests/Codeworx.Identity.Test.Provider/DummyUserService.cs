using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Test.Provider;

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
            _users.Add(new DummyEmailUser());
            _users.Add(new DummyUserWithoutPassword());
            _users.Add(new MultiTenantDummyUser(_defaultTenantMultiTenantCache));
            _users.Add(new ForceChangePasswordUser());
            _users.Add(new MfaRequiredUser());
            _users.Add(new MfaRequiredOnTenantUser());
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
            if (id == Guid.Parse(TestConstants.Users.DefaultAdmin.UserId))
            {
                throw new KeyNotFoundException();
            }
            else if (id == Guid.Parse(TestConstants.Users.MultiTenant.UserId))
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

        public Task<string> GetProviderValueAsync(ClaimsIdentity user, string providerId)
        {
            var id = Guid.Parse(user.GetUserId());
            var result = _users.Where(p => Guid.Parse(p.Identity) == id)
                .SelectMany(p => p.ExternalIdentifiers)
                .Where(p => p.Key == providerId)
                .Select(p => p.Value)
                .FirstOrDefault();

            return Task.FromResult(result);
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
            private string _password = TestConstants.Users.DefaultAdmin.Password;
            public bool ConfirmationPending => false;

            public DummyUser()
            {
                FailedLoginCount = 0;
            }

            public string DefaultTenantKey => null;

            public string Identity => TestConstants.Users.DefaultAdmin.UserId;

            public string Name => TestConstants.Users.DefaultAdmin.UserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public int FailedLoginCount { get; set; }

            public bool HasMfaRegistration => false;

            public AuthenticationMode AuthenticationMode => AuthenticationMode.Login;

            public void ResetPassword(string password)
            {
                _forceChangePassword = false;
                _password = password;
            }
        }

        public class DummyEmailUser : IDummyUser
        {
            private bool _forceChangePassword;
            private string _password = TestConstants.Users.DefaultEmail.Password;
            public bool ConfirmationPending => false;

            public DummyEmailUser()
            {
                FailedLoginCount = 0;
            }

            public string DefaultTenantKey => null;

            public string Identity => TestConstants.Users.DefaultEmail.UserId;

            public string Name => TestConstants.Users.DefaultEmail.UserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public int FailedLoginCount { get; set; }

            public bool HasMfaRegistration => false;

            public AuthenticationMode AuthenticationMode => AuthenticationMode.Login;

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
            public bool ConfirmationPending => false;
            public string DefaultTenantKey => null;

            public string Identity => TestConstants.Users.NoPassword.UserId;

            public string Name => TestConstants.Users.NoPassword.UserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public int FailedLoginCount { get; set; }

            public bool HasMfaRegistration => false;

            public AuthenticationMode AuthenticationMode => AuthenticationMode.Login;

            public void ResetPassword(string password)
            {
                _forceChangePassword = false;
                _password = password;
            }
        }

        public class MultiTenantDummyUser : IDummyUser
        {
            private bool _forceChangePassword;
            private string _password = TestConstants.Users.MultiTenant.Password;
            public MultiTenantDummyUser(string defaultTenantKey = null)
            {
                FailedLoginCount = 0;
                this.DefaultTenantKey = defaultTenantKey;
            }

            public bool ConfirmationPending => false;

            public string DefaultTenantKey { get; }

            public string Identity => TestConstants.Users.MultiTenant.UserId;

            public string Name => TestConstants.Users.MultiTenant.UserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public int FailedLoginCount { get; set; }

            public bool ForceChangePassword => _forceChangePassword;

            public bool HasMfaRegistration => false;

            public AuthenticationMode AuthenticationMode => AuthenticationMode.Login;

            public void ResetPassword(string password)
            {
                _password = password;
                _forceChangePassword = false;
            }
        }

        public class ForceChangePasswordUser : IDummyUser
        {
            private bool _forceChangePassword = true;
            private string _password = TestConstants.Users.ForceChangePassword.Password;

            public ForceChangePasswordUser()
            {
                FailedLoginCount = 0;
            }
            public bool ConfirmationPending => false;
            public string DefaultTenantKey => null;

            public string Identity => TestConstants.Users.ForceChangePassword.UserId;

            public string Name => TestConstants.Users.ForceChangePassword.UserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedProviders { get; } = ImmutableList<string>.Empty;

            public int FailedLoginCount { get; set; }

            public bool HasMfaRegistration => false;

            public AuthenticationMode AuthenticationMode => AuthenticationMode.Login;

            public void ResetPassword(string password)
            {
                _password = password;
                _forceChangePassword = false;
            }
        }


        public class MfaRequiredUser : IDummyUser
        {
            private bool _forceChangePassword;
            private string _password = TestConstants.Users.MfaRequired.Password;
            public bool ConfirmationPending => false;

            public MfaRequiredUser()
            {
                FailedLoginCount = 0;
            }

            public string DefaultTenantKey => null;

            public string Identity => TestConstants.Users.MfaRequired.UserId;

            public string Name => TestConstants.Users.MfaRequired.UserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public int FailedLoginCount { get; set; }

            public bool HasMfaRegistration => false;

            public AuthenticationMode AuthenticationMode => AuthenticationMode.Mfa;

            public void ResetPassword(string password)
            {
                _forceChangePassword = false;
                _password = password;
            }
        }

        public class MfaRequiredOnTenantUser : IDummyUser
        {
            private bool _forceChangePassword;
            private string _password = TestConstants.Users.MfaRequired.Password;
            public bool ConfirmationPending => false;

            public MfaRequiredOnTenantUser()
            {
                FailedLoginCount = 0;
            }

            public string DefaultTenantKey => TestConstants.Tenants.MfaTenant.Id;

            public string Identity => TestConstants.Users.MfaRequired.UserId;

            public string Name => TestConstants.Users.MfaRequired.UserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public int FailedLoginCount { get; set; }

            public bool HasMfaRegistration => false;

            public AuthenticationMode AuthenticationMode => AuthenticationMode.Login;

            public void ResetPassword(string password)
            {
                _forceChangePassword = false;
                _password = password;
            }
        }
    }
}