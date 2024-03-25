using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.Login
{
    public class LoginRequestValidator : IRequestValidator<LoginRequest>
    {
        private readonly IBaseUriAccessor _baseUriAccessor;

        public LoginRequestValidator(IBaseUriAccessor baseUriAccessor)
        {
            _baseUriAccessor = baseUriAccessor;
        }

        public Task ValidateAsync(LoginRequest request)
        {
            if (request.ReturnUrl != null && !_baseUriAccessor.IsValidRelative(request.ReturnUrl))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse(Constants.InvalidReturnUrlError));
            }

            return Task.CompletedTask;
        }
    }
}
