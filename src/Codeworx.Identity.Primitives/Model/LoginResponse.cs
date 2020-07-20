using System.Collections.Generic;
using System.Collections.Immutable;
using Codeworx.Identity.Login;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class LoginResponse : IViewData
    {
        public LoginResponse(IEnumerable<ILoginRegistrationGroup> groups, string returnUrl = null, string username = null, string error = null)
        {
            Groups = groups.ToImmutableList();
            ReturnUrl = returnUrl;
            Username = username;
            Error = error;
        }

        public string Error { get; }

        public IEnumerable<ILoginRegistrationGroup> Groups { get; }

        public string ReturnUrl { get; }

        public string Username { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(Error), Error);
            target.Add(nameof(Groups), Groups);
            target.Add(nameof(ReturnUrl), ReturnUrl);
            target.Add(nameof(Username), Username);
        }
    }
}