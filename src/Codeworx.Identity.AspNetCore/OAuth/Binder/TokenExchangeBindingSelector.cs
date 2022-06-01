using System;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class TokenExchangeBindingSelector : TokenRequestBindingSelector<TokenExchangeRequest>
    {
        public TokenExchangeBindingSelector(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override string GrantType => Constants.OAuth.GrantType.TokenExchange;
    }
}
