using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Response;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class StageOneAuthorizationRequestProcessor : IIdentityRequestProcessor<IAuthorizationParameters, AuthorizationRequest>
    {
        private readonly IClientService _clientService;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public StageOneAuthorizationRequestProcessor(IClientService clientService, IBaseUriAccessor baseUriAccessor)
        {
            _clientService = clientService;
            _baseUriAccessor = baseUriAccessor;
        }

        public int SortOrder => 100;

        public async Task ProcessAsync(IIdentityDataParametersBuilder<IAuthorizationParameters> builder, AuthorizationRequest request)
        {
            var client = await _clientService.GetById(request.ClientId).ConfigureAwait(false);

            if (client == null)
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ClientIdName);
            }

            builder.WithClient(client);

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

            var redirect = new Uri(redirectUrl, UriKind.RelativeOrAbsolute);

            if (!redirect.IsAbsoluteUri)
            {
                redirect = new Uri(_baseUriAccessor.BaseUri, redirect);
            }

            if (!client.ValidRedirectUrls.Any(p => CheckRedirectUrl(redirect, p)))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.RedirectUriName);
            }

            builder = builder.WithRedirectUri(redirectUrl);

            if (!Validator.TryValidateProperty(request.Prompt, new ValidationContext(request) { MemberName = nameof(request.Prompt) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.PromptName);
            }

            builder = builder.WithPrompts(request.Prompt?.Split(' ') ?? new string[] { });

            var parameters = builder.Parameters;

            if (parameters.Prompts.Contains(Constants.OAuth.Prompt.None))
            {
                if (parameters.Prompts.Count > 1)
                {
                    builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.PromptName);
                }
                else if (parameters.User == null)
                {
                    builder.Throw(Constants.OpenId.Error.LoginRequired, null);
                }
            }

            if (parameters.User == null)
            {
                throw new ErrorResponseException<LoginChallengeResponse>(new LoginChallengeResponse(request.Prompt));
            }

            builder = builder.WithState(request.State);

            if (!Validator.TryValidateProperty(request.State, new ValidationContext(request) { MemberName = nameof(request.State) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.StateName);
            }

            if (!Validator.TryValidateProperty(request.ClientId, new ValidationContext(request) { MemberName = nameof(request.ClientId) }, new List<ValidationResult>()))
            {
                builder.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.ClientIdName);
            }
        }

        private bool CheckRedirectUrl(Uri redirect, Uri compare)
        {
            if (!compare.IsAbsoluteUri)
            {
                compare = new Uri(_baseUriAccessor.BaseUri, compare);
            }

            if (redirect.Host.Equals(compare.Host, StringComparison.OrdinalIgnoreCase) &&
                compare.Host.Equals(Constants.Localhost, StringComparison.OrdinalIgnoreCase) &&
                compare.PathAndQuery == "/")
            {
                return true;
            }

            return redirect.Equals(compare);
        }
    }
}
