using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface ITokenFlowService
    {
        string SupportedGrantType { get; }

#pragma warning disable SA1009 //Justification: ValueTuple
#pragma warning disable SA1008 //Justification: ValueTuple
        Task<ITokenResult> AuthorizeRequest(TokenRequest request, (string ClientId, string ClientSecret)? authorizationHeader);
#pragma warning restore SA1009
#pragma warning restore SA1008
    }
}
