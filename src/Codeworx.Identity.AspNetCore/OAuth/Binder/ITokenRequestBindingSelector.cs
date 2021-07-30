using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public interface ITokenRequestBindingSelector : IRequestBinder<TokenRequest>
    {
        string GrantType { get; }
    }
}
