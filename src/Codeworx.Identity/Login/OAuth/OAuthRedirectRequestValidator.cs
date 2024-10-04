using System;
using System.Threading.Tasks;
using Codeworx.Identity.Invitation;
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
            if (request.ReturnUrl != null)
            {
                if (_baseUriAccessor.IsValidRelative(request.ReturnUrl))
                {
                    return Task.CompletedTask;
                }
                else if (request.Invitation?.RedirectUri != null)
                {
                    var actual = new Uri(request.ReturnUrl, UriKind.RelativeOrAbsolute);
                    var expected = new Uri(request.Invitation.RedirectUri, UriKind.RelativeOrAbsolute);
                    if (actual.Equals(expected))
                    {
                        return Task.CompletedTask;
                    }
                }

                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse(Constants.InvalidReturnUrlError));
            }

            return Task.CompletedTask;
        }
    }
}
