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
    public class RefreshTokenRequestBinder : IRequestBinder<RefreshTokenRequest>
    {
        public Task<RefreshTokenRequest> BindAsync(HttpRequest request)
        {
            string clientId = null;
            string clientSecret = null;
            string refreshToken = null;

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
            request.Form.TryGetValue(Constants.OAuth.RefreshTokenName, out var refreshTokenValues);

            if (clientIdValues.Count > 1 ||
                clientSecretValues.Count > 1 ||
                scopeValues.Count > 1 ||
                refreshTokenValues.Count > 1)
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
            refreshToken = refreshTokenValues.FirstOrDefault();

            if (string.IsNullOrEmpty(clientId))
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidClient);
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidRequest, Constants.OAuth.RefreshTokenName);
            }

            return Task.FromResult(new RefreshTokenRequest(
                                                clientId,
                                                clientSecret,
                                                refreshToken,
                                                scopeValues.FirstOrDefault()));
        }
    }
}