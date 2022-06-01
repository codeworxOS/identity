using System.Collections.Generic;

namespace Codeworx.Identity.Model
{
    public interface IUser
    {
        string DefaultTenantKey { get; }

        string Identity { get; }

        string Name { get; }

        string PasswordHash { get; }

        bool ForceChangePassword { get; }

        bool ConfirmationPending { get; }

        IReadOnlyList<string> LinkedProviders { get; }

        int FailedLoginCount { get; }
    }
}