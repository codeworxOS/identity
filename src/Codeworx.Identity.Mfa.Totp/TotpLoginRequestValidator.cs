using System.Threading.Tasks;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.Mfa.Totp
{
    public class TotpLoginRequestValidator : IRequestValidator<TotpLoginRequest>
    {
        private readonly IBaseUriAccessor _baseUriAccessor;

        public TotpLoginRequestValidator(IBaseUriAccessor baseUriAccessor)
        {
            _baseUriAccessor = baseUriAccessor;
        }

        public Task ValidateAsync(TotpLoginRequest request)
        {
            if (request.ReturnUrl != null && !_baseUriAccessor.IsRelative(request.ReturnUrl))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse(Constants.InvalidReturnUrlError));
            }

            return Task.CompletedTask;
        }
    }
}
