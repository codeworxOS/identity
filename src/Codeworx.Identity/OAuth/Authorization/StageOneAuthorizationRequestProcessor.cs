using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class StageOneAuthorizationRequestProcessor : IAuthorizationRequestProcessor
    {
        private readonly IClientService _clientService;

        public StageOneAuthorizationRequestProcessor(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<IAuthorizationParametersBuilder> ProcessAsync(IAuthorizationParametersBuilder builder, AuthorizationRequest request)
        {
            if (!Validator.TryValidateProperty(request.State, new ValidationContext(request) { MemberName = nameof(request.State) }, new List<ValidationResult>()))
            {
                AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.StateName, request.State);
            }

            builder = builder.WithState(request.State);

            if (!Validator.TryValidateProperty(request.ClientId, new ValidationContext(request) { MemberName = nameof(request.ClientId) }, new List<ValidationResult>()))
            {
                AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ClientIdName, request.State);
            }

            var client = await _clientService.GetById(request.ClientId).ConfigureAwait(false);

            if (client == null)
            {
                AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ClientIdName, request.State);
            }

            builder.WithClientId(request.ClientId);

            if (request.RedirectUri != null)
            {
                if (!Validator.TryValidateProperty(request.RedirectUri, new ValidationContext(request) { MemberName = nameof(request.RedirectUri) }, new List<ValidationResult>()))
                {
                    AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.RedirectUriName, request.State);
                }
            }

            var redirectUrl = request.RedirectUri;

            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                if (client.ValidRedirectUrls?.Count == 1)
                {
                    redirectUrl = client.ValidRedirectUrls[0].ToString();
                }
                else
                {
                    AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.RedirectUriName, request.State);
                }
            }

            if (!client.ValidRedirectUrls.Any(p => CheckRedirectUrl(redirectUrl, p)))
            {
                AuthorizationErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.RedirectUriName, request.State);
            }

            builder = builder.WithRedirectUri(redirectUrl);

            return builder;
        }

        private bool CheckRedirectUrl(string redirectUrl, Uri p)
        {
            var target = new Uri(redirectUrl);
            if (target.Host.Equals(p.Host, StringComparison.OrdinalIgnoreCase) &&
                p.Host.Equals(Constants.Localhost, StringComparison.OrdinalIgnoreCase) &&
                p.PathAndQuery == "/")
            {
                return true;
            }

            return redirectUrl.StartsWith(p.ToString(), System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
