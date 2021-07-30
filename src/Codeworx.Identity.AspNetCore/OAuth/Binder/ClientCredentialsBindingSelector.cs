using System;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class ClientCredentialsBindingSelector : TokenRequestBindingSelector<ClientCredentialsTokenRequest>
    {
        public ClientCredentialsBindingSelector(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override string GrantType => Constants.OAuth.GrantType.ClientCredentials;
    }
}
