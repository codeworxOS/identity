using System;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class RefreshTokenBindingSelector : TokenRequestBindingSelector<RefreshTokenRequest>
    {
        public RefreshTokenBindingSelector(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override string GrantType => Constants.OAuth.GrantType.RefreshToken;
    }
}
