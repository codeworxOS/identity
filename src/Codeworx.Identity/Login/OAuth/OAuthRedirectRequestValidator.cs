using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthRedirectRequestValidator : IRequestValidator<OAuthRedirectRequest>
    {
        private readonly IBaseUriAccessor _baseUriAccessor;

        public OAuthRedirectRequestValidator(IBaseUriAccessor baseUriAccessor)
        {
            _baseUriAccessor = baseUriAccessor;
        }

        public Task ValidateAsync(OAuthRedirectRequest request)
        {
            if (request.ReturnUrl != null && !_baseUriAccessor.IsRelative(request.ReturnUrl))
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse(Constants.InvalidReturnUrlError));
            }

            return Task.CompletedTask;
        }
    }
}
