using System.Collections.Generic;
using System.Collections.Immutable;
using Codeworx.Identity.Login;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class InvitationViewResponse : IViewData
    {
        public InvitationViewResponse(IEnumerable<ILoginRegistrationGroup> groups, string loginUrl, Terms terms, string error = null)
        {
            Terms = terms;
            LoginUrl = loginUrl;
            Error = error;
            Groups = groups.ToImmutableList();
        }

        public Terms Terms { get; }

        public string LoginUrl { get; }

        public string Error { get; }

        public IReadOnlyCollection<ILoginRegistrationGroup> Groups { get; }

        public bool HasError => !string.IsNullOrWhiteSpace(Error);

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(Error), Error);
            target.Add(nameof(HasError), HasError);
            target.Add(nameof(Groups), Groups);
            target.Add(nameof(Terms), Terms);
            target.Add(nameof(LoginUrl), LoginUrl);
        }
    }
}
