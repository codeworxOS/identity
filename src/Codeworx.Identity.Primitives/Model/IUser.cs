using System.Collections.Generic;

namespace Codeworx.Identity.Model
{
    public interface IUser
    {
        string DefaultTenantKey { get; }

        string Identity { get; }

        string Name { get; }

        ICollection<IOAuthClientRegistration> OAuthClientRegistrations { get; }
    }
}