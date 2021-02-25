using Codeworx.Identity.Model;

namespace Codeworx.Identity.Configuration.Model
{
    public class User : IUser
    {
        public User(string id, string name, string password)
        {
            Identity = id;
            Name = name;
            PasswordHash = password;
        }

        public string DefaultTenantKey => null;

        public string Identity { get; }

        public string Name { get; }

        public string PasswordHash { get; }

        public bool ForceChangePassword => false;
    }
}
