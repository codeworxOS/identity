using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.Account
{
    public class PasswordChangeRequestValidator : IRequestValidator<PasswordChangeRequest>
    {
        private readonly IBaseUriAccessor _baseUriAccessor;

        public PasswordChangeRequestValidator(IBaseUriAccessor baseUriAccessor)
        {
            _baseUriAccessor = baseUriAccessor;
        }

        public Task ValidateAsync(PasswordChangeRequest request)
        {
            if (request.ReturnUrl != null && !_baseUriAccessor.IsValidRelative(request.ReturnUrl))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse(Constants.InvalidReturnUrlError));
            }

            return Task.CompletedTask;
        }
    }
}
