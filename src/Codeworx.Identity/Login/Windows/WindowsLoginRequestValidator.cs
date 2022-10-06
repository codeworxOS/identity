using System.Threading.Tasks;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.Login.Windows
{
    public class WindowsLoginRequestValidator : IRequestValidator<WindowsLoginRequest>
    {
        private readonly IBaseUriAccessor _baseUriAccessor;

        public WindowsLoginRequestValidator(IBaseUriAccessor baseUriAccessor)
        {
            _baseUriAccessor = baseUriAccessor;
        }

        public Task ValidateAsync(WindowsLoginRequest request)
        {
            if (request.ReturnUrl != null && !_baseUriAccessor.IsRelative(request.ReturnUrl))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse(Constants.InvalidReturnUrlError));
            }

            return Task.CompletedTask;
        }
    }
}
