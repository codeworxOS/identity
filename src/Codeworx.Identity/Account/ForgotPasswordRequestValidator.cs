using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.Account
{
    public class ForgotPasswordRequestValidator : IRequestValidator<ForgotPasswordRequest>
    {
        private readonly IBaseUriAccessor _baseUriAccessor;

        public ForgotPasswordRequestValidator(IBaseUriAccessor baseUriAccessor)
        {
            _baseUriAccessor = baseUriAccessor;
        }

        public Task ValidateAsync(ForgotPasswordRequest request)
        {
            if (request.ReturnUrl != null && !_baseUriAccessor.IsRelative(request.ReturnUrl))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse(Constants.InvalidReturnUrlError));
            }

            return Task.CompletedTask;
        }
    }
}
