using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.ExternalLogin
{
    public interface IExternalLoginProcessor
    {
        Type RequestParameterType { get; }

        Type ConfigurationType { get; }

        Task<string> GetProcessorUrlAsync(ProviderRequest request, object configuration);

        Task<ExternalLoginResponse> ProcessAsync(object request, object configuration);
    }
}