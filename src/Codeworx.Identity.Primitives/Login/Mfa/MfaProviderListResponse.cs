using System.Collections.Generic;
using System.Collections.Immutable;
using Codeworx.Identity.Model;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaProviderListResponse : IViewData
    {
        public MfaProviderListResponse(IUser user, IEnumerable<ILoginRegistrationGroup> groups)
        {
            Groups = groups.ToImmutableList();
            User = user;
        }

        public IReadOnlyCollection<ILoginRegistrationGroup> Groups { get; }

        public IUser User { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(Groups), Groups);
            target.Add(nameof(User), User);
        }
    }
}