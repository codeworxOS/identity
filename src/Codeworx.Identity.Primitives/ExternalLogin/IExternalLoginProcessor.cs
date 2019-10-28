using System;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.ExternalLogin
{
    public interface IExternalLoginProcessor
    {
        string ProcessorType { get; }

        Type RequestParameterType { get; }

        Task<ExternalLoginResponse> ProcessAsync(object request);
    }
}
