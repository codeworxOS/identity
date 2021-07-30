using System;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class AuthorizationCodeBindingSelector : TokenRequestBindingSelector<AuthorizationCodeTokenRequest>
    {
        public AuthorizationCodeBindingSelector(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override string GrantType => Constants.OAuth.GrantType.AuthorizationCode;
    }
}
