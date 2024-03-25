using System.Threading.Tasks;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaLoginRequestValidator : IRequestValidator<MfaLoginRequest>
    {
        private readonly IBaseUriAccessor _baseUriAccessor;

        public MfaLoginRequestValidator(IBaseUriAccessor baseUriAccessor)
        {
            _baseUriAccessor = baseUriAccessor;
        }

        public Task ValidateAsync(MfaLoginRequest request)
        {
            if (request.ReturnUrl != null && !_baseUriAccessor.IsValidRelative(request.ReturnUrl))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse(Constants.InvalidReturnUrlError));
            }

            return Task.CompletedTask;
        }
    }
}
