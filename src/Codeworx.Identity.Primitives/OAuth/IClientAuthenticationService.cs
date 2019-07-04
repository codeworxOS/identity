using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth
{
    public interface IClientAuthenticationService
    {
#pragma warning disable SA1009 //Justification: ValueTuple
#pragma warning disable SA1008 //Justification: ValueTuple
        Task<(ITokenResult TokenResult, IClientRegistration ClientRegistration)> AuthenticateClient(TokenRequest request, (string ClientId, string ClientSecret)? authorizationHeader);
#pragma warning restore SA1009
#pragma warning restore SA1008
    }
}
