using System.Collections.Generic;
using System.Collections.Immutable;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaLoginResponse : IViewData
    {
        public MfaLoginResponse(
            IEnumerable<ILoginRegistrationGroup> groups,
            string returnUrl = null)
        {
            Groups = groups.ToImmutableList();
            ReturnUrl = returnUrl;
        }

        public IEnumerable<ILoginRegistrationGroup> Groups { get; }

        public string ReturnUrl { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(ReturnUrl), ReturnUrl);
            target.Add(nameof(Groups), Groups);
        }
    }
}