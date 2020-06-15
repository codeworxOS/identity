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
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.StateName);
            }

            builder = builder.WithState(request.State);

            if (!Validator.TryValidateProperty(request.ClientId, new ValidationContext(request) { MemberName = nameof(request.ClientId) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ClientIdName);
            }

            var client = await _clientService.GetById(request.ClientId).ConfigureAwait(false);

            if (client == null)
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ClientIdName);
            }

            builder.WithClientId(request.ClientId);

            if (request.RedirectUri != null)
            {
                if (!Validator.TryValidateProperty(request.RedirectUri, new ValidationContext(request) { MemberName = nameof(request.RedirectUri) }, new List<ValidationResult>()))
                {
                    builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.RedirectUriName);
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
                    builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.RedirectUriName);
                }
            }

            if (!client.ValidRedirectUrls.Any(p => CheckRedirectUrl(redirectUrl, p)))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.RedirectUriName);
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
