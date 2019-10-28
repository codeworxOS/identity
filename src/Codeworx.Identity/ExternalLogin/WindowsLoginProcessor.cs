using System;
using System.Threading.Tasks;

namespace Codeworx.Identity.ExternalLogin
{
    public class WindowsLoginProcessor : IExternalLoginProcessor
    {
        public string ProcessorType => Constants.ExternalWindowsProviderName;

        public Type RequestParameterType { get; } = typeof(WindowsLoginRequest);

        public Task<ExternalLoginResponse> ProcessAsync(object request)
        {
            throw new NotImplementedException();
        }
    }
}