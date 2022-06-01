using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity
{
    // EventIds 146xx
    public class ExternalTokenClaimsProvider : ISystemClaimsProvider
    {
        private static readonly Action<ILogger, Exception> _externalTokenKeyClaimMissingMessage;
        private static readonly Action<ILogger, Exception> _externalTokenCacheNotRegisterdMessage;
        private static readonly Action<ILogger, Exception> _noRefreshTokenMessage;
        private static readonly Action<ILogger, string, Exception> _invalidRegistrationIdMessage;
        private static readonly Action<ILogger, Exception> _unableToGetExternalTokenDataMessage;
        private static readonly Action<ILogger, Exception> _unableToRefreshExternalTokenMessage;
        private static readonly Action<ILogger, Exception> _unableToUpdateExternalTokenMessage;
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly ILogger<ExternalTokenClaimsProvider> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IdentityOptions _options;

        static ExternalTokenClaimsProvider()
        {
            _externalTokenKeyClaimMissingMessage = LoggerMessage.Define(LogLevel.Error, new EventId(14601), $"The {Constants.Claims.ExternalTokenKey} claim is not present for the current user.");
            _externalTokenCacheNotRegisterdMessage = LoggerMessage.Define(LogLevel.Error, new EventId(14602), "The external token cache is not registerd!");
            _noRefreshTokenMessage = LoggerMessage.Define(LogLevel.Error, new EventId(14603), "The external login did not provide a refresh token.");
            _invalidRegistrationIdMessage = LoggerMessage.Define<string>(LogLevel.Error, new EventId(14604), "Invalid login registration id: {registrationId}.");
            _unableToGetExternalTokenDataMessage = LoggerMessage.Define(LogLevel.Error, new EventId(14605), "Unable to retrive the external token data.");
            _unableToRefreshExternalTokenMessage = LoggerMessage.Define(LogLevel.Error, new EventId(14606), "Unable to refresh the external token data.");
            _unableToUpdateExternalTokenMessage = LoggerMessage.Define(LogLevel.Error, new EventId(14607), "Unable to update the external token data.");
        }

        public ExternalTokenClaimsProvider(
            IBaseUriAccessor baseUriAccessor,
            ILogger<ExternalTokenClaimsProvider> logger,
            IOptionsSnapshot<IdentityOptions> options,
            IServiceProvider serviceProvider)
        {
            _baseUriAccessor = baseUriAccessor;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        public async Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IIdentityDataParameters parameters)
        {
            LogoutResponse logoutResponse = null;
            bool interactive = false;

            if (parameters is IAuthorizationParameters authorization)
            {
                interactive = true;

                var builder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
                authorization.Request.Append(builder);

                var loginBuilder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
                loginBuilder.AppendPath(_options.AccountEndpoint);
                loginBuilder.AppendPath("login");
                logoutResponse = new LogoutResponse(builder.ToString());
            }

            var result = new List<AssignedClaim>();
            var scopes = parameters.Scopes.ToList();

            if (scopes.Contains(Constants.Scopes.ExternalToken.All) || scopes.Contains(Constants.Scopes.ExternalToken.IdToken) || scopes.Contains(Constants.Scopes.ExternalToken.AccessToken))
            {
                var claim = parameters.User.FindFirst(Constants.Claims.ExternalTokenKey);
                if (claim == null)
                {
                    _externalTokenKeyClaimMissingMessage(_logger, null);
                    parameters.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.ExternalToken.All);
                }

                var externalTokenCache = _serviceProvider.GetService<IExternalTokenCache>();

                if (externalTokenCache == null)
                {
                    _externalTokenCacheNotRegisterdMessage(_logger, null);
                    parameters.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.ExternalToken.All);
                }

                ExternalTokenData data = null;
                try
                {
                    data = await externalTokenCache.GetAsync(claim.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _unableToGetExternalTokenDataMessage(_logger, ex);

                    if (interactive)
                    {
                        throw new ErrorResponseException<LogoutResponse>(logoutResponse);
                    }

                    parameters.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.ExternalToken.All);
                }

                var loginService = _serviceProvider.GetRequiredService<ILoginService>();

                var registration = await loginService.GetLoginRegistrationInfoAsync(data.RegistrationId).ConfigureAwait(false);

                if (registration == null)
                {
                    _invalidRegistrationIdMessage(_logger, data.RegistrationId, null);
                    parameters.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.ExternalToken.All);
                }

                if (registration.ProcessorConfiguration is OAuthLoginConfiguration oauthConfig)
                {
                    if (oauthConfig.TokenHandling == ExternalTokenHandling.Refresh)
                    {
                        if (string.IsNullOrWhiteSpace(data.RefreshToken))
                        {
                            _noRefreshTokenMessage(_logger, null);
                            parameters.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.ExternalToken.All);
                        }

                        ClaimsIdentity response = null;

                        var tokenService = _serviceProvider.GetRequiredService<IExternalOAuthTokenService>();

                        try
                        {
                            response = await tokenService.RefreshAsync(oauthConfig, data.RefreshToken).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _unableToRefreshExternalTokenMessage(_logger, ex);

                            if (interactive)
                            {
                                throw new ErrorResponseException<LogoutResponse>(logoutResponse);
                            }

                            parameters.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.ExternalToken.All);
                        }

                        var current = new ExternalTokenData
                        {
                            RegistrationId = registration.Id,
                            AccessToken = response.FindFirst(Constants.OAuth.AccessTokenName)?.Value,
                            RefreshToken = response.FindFirst(Constants.OAuth.RefreshTokenName)?.Value,
                            IdToken = response.FindFirst(Constants.OpenId.IdTokenName)?.Value,
                        };

                        if (!string.IsNullOrWhiteSpace(current.RefreshToken) && data.RefreshToken != current.RefreshToken)
                        {
                            try
                            {
                                await externalTokenCache.UpdateAsync(claim.Value, current).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                _unableToUpdateExternalTokenMessage(_logger, ex);
                                parameters.Throw(Constants.OAuth.Error.InvalidScope, Constants.Scopes.ExternalToken.All);
                            }
                        }

                        data = current;
                    }

                    if (scopes.Contains(Constants.Scopes.ExternalToken.All) || scopes.Contains(Constants.Scopes.ExternalToken.IdToken))
                    {
                        result.Add(new AssignedClaim(new[] { Constants.Claims.ExternalToken, Constants.OpenId.IdTokenName }, new[] { data.IdToken }, ClaimTarget.AccessToken));
                    }

                    if (scopes.Contains(Constants.Scopes.ExternalToken.All) || scopes.Contains(Constants.Scopes.ExternalToken.AccessToken))
                    {
                        result.Add(new AssignedClaim(new[] { Constants.Claims.ExternalToken, Constants.OAuth.AccessTokenName }, new[] { data.AccessToken }, ClaimTarget.AccessToken));
                    }
                }
            }

            return result;
        }
    }
}
