using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Codeworx.Identity.Wpf.Test.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Wpf.Test.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IDialogHandler _dialog;
        private readonly LoginOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenIdConnectProtocolValidator _validator;
        private OpenIdConnectConfiguration _config;
        private OpenIdConnectProtocolValidationContext _context;
        private TaskCompletionSource<ISessionInfo> _loginResponse;
        private Uri _loginUri;

        public LoginViewModel(IDialogHandler dialog, IOptionsSnapshot<LoginOptions> options, IHttpClientFactory httpClientFactory)
        {
            _validator = new OpenIdConnectProtocolValidator();
            NavigatingCommand = new DelegateCommand<NavigatingArgs>(BrowserNavigating);
            _dialog = dialog;
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        public Uri LoginUri
        {
            get
            {
                return _loginUri;
            }

            set
            {
                if (_loginUri != value)
                {
                    _loginUri = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand NavigatingCommand { get; }

        public async Task<ISessionInfo> LoginAsync(string tenant = null)
        {
            using (StartJob("loading login data..."))
            {
                var metadataUrl = new Uri(new Uri(_options.Authority), new Uri(".well-known/openid-configuration", UriKind.RelativeOrAbsolute));

                var documentRetriever = new HttpDocumentRetriever();

                if (metadataUrl.Host.Equals("localhost"))
                {
                    documentRetriever.RequireHttps = false;
                }

                var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataUrl.ToString(), new OpenIdConnectConfigurationRetriever(), documentRetriever);

                try
                {
                    _config = await configManager.GetConfigurationAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await _dialog.ShowMessageBoxAsync("Connection Error.", "The Login Service is unreachable.", MessageBoxKind.Error);
                    return null;
                }
            }

            _context = new OpenIdConnectProtocolValidationContext()
            {
                ClientId = _options.Audience,
                Nonce = _validator.GenerateNonce(),
            };

            _context.State = _context.Nonce;

            var requestScopes = _options.Scope;

            if (tenant != null)
            {
                requestScopes = $"{requestScopes} {tenant}";
            }

            var authorizationRequest = new OpenIdConnectMessage()
            {
                AuthorizationEndpoint = _config.AuthorizationEndpoint,
                Iss = _config.Issuer,
                IssuerAddress = _config.AuthorizationEndpoint,
                ClientId = _options.Audience,
                ResponseType = OpenIdConnectResponseType.Code,
                Nonce = _context.Nonce,
                RedirectUri = new Uri(new Uri(_config.Issuer), "/account/redirect").ToString(),
                Scope = requestScopes,
                State = _context.State,
                Prompt = "login",
            };

            _loginResponse = new TaskCompletionSource<ISessionInfo>();
            var url = new Uri(authorizationRequest.CreateAuthenticationRequestUrl());

            var loginUrl = $"{url}";
            if (tenant == null)
            {
                loginUrl = $"/account/logout?returnurl={Uri.EscapeDataString(url.ToString())}";
            }

            var logoutUrl = new Uri(
                new Uri(_config.Issuer),
                loginUrl);

            LoginUri = logoutUrl;
            return await _loginResponse.Task.ConfigureAwait(false);
        }

        private void BrowserNavigating(NavigatingArgs e)
        {
            var path = $"{e.Uri.Scheme}://{e.Uri.Authority}{e.Uri.AbsolutePath}";

            var redirectUri = new Uri(new Uri(_config.Issuer), "/account/redirect");

            if (new Uri(path) == redirectUri)
            {
                var data = from q in e.Uri.Query.TrimStart('?').Split('&')
                           let keyValue = q.Split('=')
                           let key = Uri.UnescapeDataString(keyValue[0])
                           where keyValue.Length == 2
                           select new { Key = key, Value = Uri.UnescapeDataString(keyValue[1]) };

                var query = data
                                .GroupBy(p => p.Key)
                                .ToDictionary(p => p.Key, p => p.Select(x => x.Value).ToArray());

                var message = new OpenIdConnectMessage(query);

                _context.ProtocolMessage = message;

                _validator.ValidateAuthenticationResponse(_context);

                RedeemAuthorizationCodeAsync(message);
            }
        }

        private async void RedeemAuthorizationCodeAsync(OpenIdConnectMessage authorizationResponse)
        {
            var tokenRequest = new OpenIdConnectMessage()
            {
                State = authorizationResponse.State,
                Code = authorizationResponse.Code,
                GrantType = OpenIdConnectGrantTypes.AuthorizationCode,
                TokenEndpoint = _config.TokenEndpoint,
                ClientId = _context.ClientId,
                Nonce = _context.Nonce,
                RedirectUri = new Uri(new Uri(_config.Issuer), "/account/redirect").ToString(),
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, _config.TokenEndpoint);
            requestMessage.Content = new FormUrlEncodedContent(tokenRequest.Parameters);

            OpenIdConnectMessage tokenResponse = null;

            var client = _httpClientFactory.CreateClient();

            var responseMessage = await client.SendAsync(requestMessage);
            var content = await responseMessage.Content.ReadAsStringAsync();

            tokenResponse = new OpenIdConnectMessage(content);

            _context.ProtocolMessage = tokenResponse;

            var handler = new JwtSecurityTokenHandler();
            var validationParameter = new TokenValidationParameters();
            validationParameter.ValidIssuer = _config.Issuer;
            validationParameter.ValidAudience = _context.ClientId;
            validationParameter.IssuerSigningKeys = _config.SigningKeys;

            handler.ValidateToken(tokenResponse.IdToken, validationParameter, out var token);
            _context.ValidatedIdToken = (JwtSecurityToken)token;

            _validator.ValidateTokenResponse(_context);

            var sessionInfo = new AuthTokenSessionInfo(tokenResponse.AccessToken, _context.ValidatedIdToken.Payload);
            _loginResponse.SetResult(sessionInfo);
        }
    }
}
