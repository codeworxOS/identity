using System.Collections.Generic;
using System.Collections.Immutable;
using Codeworx.Identity.Login;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class ProfileResponse : IViewData
    {
        public ProfileResponse(IUser user, IEnumerable<ILoginRegistrationGroup> registrations, string returnUrl = null)
        {
            Groups = registrations.ToImmutableList();
            ReturnUrl = returnUrl;
            User = user;
        }

        public IUser User { get; }

        public IEnumerable<ILoginRegistrationGroup> Groups { get; }

        public string ReturnUrl { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(User), User);
            target.Add(nameof(Groups), Groups);
            target.Add(nameof(ReturnUrl), ReturnUrl);
        }
    }
}