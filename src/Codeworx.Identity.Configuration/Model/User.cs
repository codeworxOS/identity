using System.Collections.Generic;
using System.Collections.Immutable;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Configuration.Model
{
    public class User : IUser
    {
        public User(string id, string name, string password, IEnumerable<string> linkedMfaProviders, IEnumerable<string> linkedLoginProviders)
        {
            Identity = id;
            Name = name;
            PasswordHash = password;
            LinkedMfaProviders = linkedMfaProviders.ToImmutableList();
            LinkedLoginProviders = linkedLoginProviders.ToImmutableList();
        }

        public AuthenticationMode AuthenticationMode { get; }

        public bool ConfirmationPending => false;

        public string DefaultTenantKey => null;

        public int FailedLoginCount => 0;

        public bool ForceChangePassword => false;

        public bool HasMfaRegistration => false;

        public string Identity { get; }

        public IReadOnlyList<string> LinkedLoginProviders { get; }

        public IReadOnlyList<string> LinkedMfaProviders { get; }

        public string Name { get; }

        public string PasswordHash { get; }
    }
}