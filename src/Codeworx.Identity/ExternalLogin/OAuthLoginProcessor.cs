using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.ExternalLogin
{
    public class OAuthLoginProcessor : IExternalLoginProcessor
    {
        private readonly IBaseUriAccessor _baseUriAccessor;

        public OAuthLoginProcessor(IBaseUriAccessor baseUriAccessor)
        {
            _baseUriAccessor = baseUriAccessor;
        }

        public Type RequestParameterType { get; } = typeof(OAuthLoginRequest);

        public Type ConfigurationType { get; } = typeof(OAuthLoginConfiguration);

        public Task<string> GetProcessorUrlAsync(ProviderRequest request, object configuration)
        {
            throw new NotImplementedException();
        }

        public Task<ExternalLoginResponse> ProcessAsync(object request, object configuration)
        {
            throw new NotImplementedException();
        }
    }
}