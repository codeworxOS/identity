using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public abstract class TokenRequestBindingSelector<TTokenRequest> : ITokenRequestBindingSelector
        where TTokenRequest : TokenRequest
    {
        private readonly IServiceProvider _serviceProvider;

        public TokenRequestBindingSelector(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public abstract string GrantType { get; }

        public async Task<TokenRequest> BindAsync(HttpRequest request)
        {
            var binder = _serviceProvider.GetRequiredService<IRequestBinder<TTokenRequest>>();

            return await binder.BindAsync(request);
        }
    }
}