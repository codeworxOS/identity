using System.Collections.Generic;
using System.Collections.Immutable;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Configuration.Model
{
    public class User : IUser
    {
        public User(string id, string name, string password, IEnumerable<string> linkedProviders)
        {
            Identity = id;
            Name = name;
            PasswordHash = password;
            LinkedProviders = linkedProviders.ToImmutableList();
        }

        public string DefaultTenantKey => null;

        public string Identity { get; }

        public string Name { get; }

        public string PasswordHash { get; }

        public bool ForceChangePassword => false;

        public IReadOnlyList<string> LinkedProviders { get; }
    }
}
