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
    public class DummyUserService : IUserService, IDefaultTenantService, ILinkUserService
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
            _users.Add(new MfaTestUser());
            _users.Add(new MfaTestUserWithMfaRequired());
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

        public Task<string> GetProviderValueAsync(string userId, string providerId)
        {
            var id = Guid.Parse(userId);
            var result = _users.Where(p => Guid.Parse(p.Identity) == id)
                .SelectMany(p => p.ExternalIdentifiers)
                .Where(p => p.Key == providerId)
                .Select(p => p.Value)
                .FirstOrDefault();

            return Task.FromResult(result);
        }

        public async Task LinkUserAsync(IUser user, IExternalLoginData loginData)
        {
            var sharedSecret = await loginData.GetExternalIdentifierAsync();

            var dummyUser = (IDummyUser)user;
            dummyUser.ExternalIdentifiers.Add(loginData.LoginRegistration.Id, sharedSecret);
        }

        public Task UnlinkUserAsync(IUser user, string providerId)
        {
            var dummyUser = (IDummyUser)user;
            dummyUser.ExternalIdentifiers.Remove(providerId);

            return Task.CompletedTask;
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

            public IReadOnlyList<string> LinkedMfaProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public IReadOnlyList<string> LinkedLoginProviders => ExternalIdentifiers.Keys.ToImmutableList();

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

            public IReadOnlyList<string> LinkedMfaProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public IReadOnlyList<string> LinkedLoginProviders => ExternalIdentifiers.Keys.ToImmutableList();

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

            public IReadOnlyList<string> LinkedMfaProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public IReadOnlyList<string> LinkedLoginProviders => ExternalIdentifiers.Keys.ToImmutableList();

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

            public IReadOnlyList<string> LinkedMfaProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public IReadOnlyList<string> LinkedLoginProviders => ExternalIdentifiers.Keys.ToImmutableList();
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

            public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>() {
                { TestConstants.LoginProviders.ExternalWindowsProvider.Id, TestConstants.Users.ForceChangePassword.WindowsSid }
            };

            public IReadOnlyList<string> LinkedLoginProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedMfaProviders { get; } = ImmutableList<string>.Empty;

            public int FailedLoginCount { get; set; }

            public bool HasMfaRegistration => false;

            public AuthenticationMode AuthenticationMode => AuthenticationMode.Login;

            public void ResetPassword(string password)
            {
                _password = password;
                _forceChangePassword = false;
            }
        }


        public class MfaTestUser : IDummyUser
        {
            private bool _forceChangePassword;
            private string _password = TestConstants.Users.MfaTestUser.Password;
            public bool ConfirmationPending => false;

            public MfaTestUser()
            {
                FailedLoginCount = 0;
                AuthenticationMode = AuthenticationMode.Login;
                ExternalIdentifiers = new Dictionary<string, string>()
                {
                    { TestConstants.LoginProviders.TotpProvider.Id, TestConstants.Users.MfaTestUser.MfaSharedSecret }
                };
            }

            public string DefaultTenantKey => null;

            public string Identity => TestConstants.Users.MfaTestUser.UserId;

            public string Name => TestConstants.Users.MfaTestUser.UserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; }

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedMfaProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public IReadOnlyList<string> LinkedLoginProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public int FailedLoginCount { get; set; }

            public bool HasMfaRegistration => ExternalIdentifiers.ContainsKey(TestConstants.LoginProviders.TotpProvider.Id);

            public AuthenticationMode AuthenticationMode { get; private set; }

            public void RequireMfa()
            {
                AuthenticationMode = AuthenticationMode.Mfa;
            }

            public void ResetPassword(string password)
            {
                _forceChangePassword = false;
                _password = password;
            }
        }

        public class MfaTestUserWithMfaRequired : IDummyUser
        {
            private bool _forceChangePassword;
            private string _password = TestConstants.Users.MfaTestUserWithMfaRequired.Password;
            public bool ConfirmationPending => false;

            public MfaTestUserWithMfaRequired()
            {
                FailedLoginCount = 0;
                AuthenticationMode = AuthenticationMode.Mfa;
                ExternalIdentifiers = new Dictionary<string, string>()
                {
                    { TestConstants.LoginProviders.TotpProvider.Id, TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret }
                };
            }

            public string DefaultTenantKey => null;

            public string Identity => TestConstants.Users.MfaTestUserWithMfaRequired.UserId;

            public string Name => TestConstants.Users.MfaTestUserWithMfaRequired.UserName;

            public string PasswordHash => _password;

            public IDictionary<string, string> ExternalIdentifiers { get; }

            public bool ForceChangePassword => _forceChangePassword;

            public IReadOnlyList<string> LinkedMfaProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public IReadOnlyList<string> LinkedLoginProviders => ExternalIdentifiers.Keys.ToImmutableList();

            public int FailedLoginCount { get; set; }

            public bool HasMfaRegistration => ExternalIdentifiers.ContainsKey(TestConstants.LoginProviders.TotpProvider.Id);

            public AuthenticationMode AuthenticationMode { get; private set; }

            public void ResetMfaRequired()
            {
                AuthenticationMode = AuthenticationMode.Login;
            }

            public void RemoveMfaRegistration()
            {
                ExternalIdentifiers.Remove(TestConstants.LoginProviders.TotpProvider.Id);
            }

            public void ResetPassword(string password)
            {
                _forceChangePassword = false;
                _password = password;
            }
        }
    }
}