using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.Login
{
    public class LogoutRequestValidator : IRequestValidator<LogoutRequest>
    {
        private readonly IBaseUriAccessor _baseUriAccessor;

        public LogoutRequestValidator(IBaseUriAccessor baseUriAccessor)
        {
            _baseUriAccessor = baseUriAccessor;
        }

        public Task ValidateAsync(LogoutRequest request)
        {
            if (request.ReturnUrl != null && !_baseUriAccessor.IsRelative(request.ReturnUrl))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse(Constants.InvalidReturnUrlError));
            }

            return Task.CompletedTask;
        }
    }
}
