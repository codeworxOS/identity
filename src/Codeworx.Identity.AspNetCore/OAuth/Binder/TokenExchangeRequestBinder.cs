using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class TokenExchangeRequestBinder : IRequestBinder<TokenExchangeRequest>
    {
        public Task<TokenExchangeRequest> BindAsync(HttpRequest request)
        {
            string clientId = null;
            string clientSecret = null;
            string scope = null;
            string audience = null;
            string subjectToken = null;
            string subjectTokenType = null;
            string actorToken = null;
            string actorTokenType = null;
            string requestedTokenType = null;

            if (AuthenticationHeaderValue.TryParse(request.Headers[HeaderNames.Authorization], out var authenticationHeaderValue))
            {
                if (authenticationHeaderValue.Scheme.Equals(Constants.BasicHeader, StringComparison.OrdinalIgnoreCase))
                {
                    var credentialBytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                    clientId = credentials[0];
                    clientSecret = credentials[1];
                }
            }

            request.Form.TryGetValue(Constants.OAuth.ClientIdName, out var clientIdValues);
            request.Form.TryGetValue(Constants.OAuth.ClientSecretName, out var clientSecretValues);
            request.Form.TryGetValue(Constants.OAuth.ScopeName, out var scopeValues);
            request.Form.TryGetValue(Constants.OAuth.AudienceName, out var audienceValues);
            request.Form.TryGetValue(Constants.OAuth.SubjectTokenName, out var subjectTokenValues);
            request.Form.TryGetValue(Constants.OAuth.SubjectTokenTypeName, out var subjectTokenTypeValues);
            request.Form.TryGetValue(Constants.OAuth.ActorTokenName, out var actorTokenValues);
            request.Form.TryGetValue(Constants.OAuth.ActorTokenTypeName, out var actorTokenTypeValues);
            request.Form.TryGetValue(Constants.OAuth.RequestedTokenTypeName, out var requesteTokenTypeValues);

            if (clientIdValues.Count > 1 ||
                clientSecretValues.Count > 1 ||
                scopeValues.Count > 1 ||
                audienceValues.Count > 1 ||
                subjectTokenValues.Count > 1 ||
                subjectTokenTypeValues.Count > 1 ||
                actorTokenValues.Count > 1 ||
                actorTokenTypeValues.Count > 1 ||
                requesteTokenTypeValues.Count > 1)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest);
            }

            if (clientIdValues.Any() && clientId != null && clientIdValues.First() != clientId)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest);
            }

            if (clientSecretValues.Any() && clientSecret != null && clientSecretValues.First() != clientSecret)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest);
            }

            clientId = clientId ?? clientIdValues.FirstOrDefault();
            clientSecret = clientSecret ?? clientSecretValues.FirstOrDefault();
            audience = audienceValues.FirstOrDefault();
            scope = scopeValues.FirstOrDefault();
            subjectToken = subjectTokenValues.FirstOrDefault();
            subjectTokenType = subjectTokenTypeValues.FirstOrDefault();
            actorToken = actorTokenValues.FirstOrDefault();
            actorTokenType = actorTokenTypeValues.FirstOrDefault();
            requestedTokenType = requesteTokenTypeValues.FirstOrDefault();

            return Task.FromResult(new TokenExchangeRequest(
                                                clientId,
                                                clientSecret,
                                                audience,
                                                scope,
                                                subjectToken,
                                                subjectTokenType,
                                                actorToken,
                                                actorTokenType,
                                                requestedTokenType));
        }
    }
}