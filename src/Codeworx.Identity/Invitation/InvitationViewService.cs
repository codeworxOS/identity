using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Invitation
{
    public class InvitationViewService : IInvitationViewService
    {
        private readonly IInvitationService _service;
        private readonly IStringResources _stringResources;
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private readonly IIdentityService _identityService;
        private readonly IdentityServerOptions _identityServerOptions;
        private readonly IdentityOptions _identityOptions;
        private readonly IPasswordPolicyProvider _passwordPolicyProvider;
        private readonly ILoginPolicyProvider _loginPolicyProvider;
        private readonly IChangePasswordService _changePasswordService;
        private readonly IChangeUsernameService _changeUsernameService;

        public InvitationViewService(
            IInvitationService service,
            IUserService userService,
            ILoginService loginService,
            IIdentityService identityService,
            IOptionsSnapshot<IdentityOptions> identityOptions,
            IdentityServerOptions identityServerOptions,
            IPasswordPolicyProvider passwordPolicyProvider,
            ILoginPolicyProvider loginPolicyProvider,
            IChangePasswordService changePasswordService,
            IStringResources stringResources,
            IBaseUriAccessor baseUriAccessor,
            IChangeUsernameService changeUsernameService = null)
        {
            _service = service;
            _stringResources = stringResources;
            _baseUriAccessor = baseUriAccessor;
            _changeUsernameService = changeUsernameService;
            _userService = userService;
            _loginService = loginService;
            _identityService = identityService;
            _identityServerOptions = identityServerOptions;
            _identityOptions = identityOptions.Value;
            _passwordPolicyProvider = passwordPolicyProvider;
            _loginPolicyProvider = loginPolicyProvider;
            _changePasswordService = changePasswordService;
        }

        public async Task<SignInResponse> ProcessAsync(ProcessInvitationViewRequest request)
        {
            var languageCode = _stringResources.GetResource(StringResource.LanguageCode);

            var urlBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
            urlBuilder.AppendPath(_identityServerOptions.AccountEndpoint);
            urlBuilder.AppendPath("login");

            try
            {
                var invitation = await _service.GetInvitationAsync(request.Code).ConfigureAwait(false);
                var user = await _userService.GetUserByIdAsync(invitation.UserId).ConfigureAwait(false);

                if (invitation.Action.HasFlag(InvitationAction.ChangePassword))
                {
                    var policy = await _passwordPolicyProvider.GetPolicyAsync();
                    string error = null;
                    bool hasError = false;

                    if (request.Password != request.ConfirmPassword)
                    {
                        error = _stringResources.GetResource(StringResource.PasswordChangeNotMatchingError);
                        hasError = true;
                    }
                    else if (!policy.IsValid(request.Password, languageCode, out error))
                    {
                        hasError = true;
                    }

                    if (hasError)
                    {
                        var errorResponse = await ShowAsync(new InvitationViewRequest(request.Code, request.HeaderOnly, request.ProviderId, error));
                        throw new ErrorResponseException<InvitationViewResponse>(errorResponse);
                    }

                    await _changePasswordService.SetPasswordAsync(user, request.Password).ConfigureAwait(false);
                }

                if (invitation.Action.HasFlag(InvitationAction.ChangeLogin) && user.Name != request.UserName)
                {
                    if (_changeUsernameService == null)
                    {
                        throw new NotSupportedException("Missing IChangeUsernameService");
                    }

                    var loginPolicy = await _loginPolicyProvider.GetPolicyAsync().ConfigureAwait(false);
                    if (!loginPolicy.IsValid(request.UserName, languageCode, out var error))
                    {
                        var errorResponse = await ShowAsync(new InvitationViewRequest(request.Code, request.HeaderOnly, request.ProviderId, error));
                        throw new ErrorResponseException<InvitationViewResponse>(errorResponse);
                    }

                    user = await _changeUsernameService.ChangeUsernameAsync(user, request.UserName).ConfigureAwait(false);
                }

                user = await _userService.GetUserByIdAsync(user.Identity).ConfigureAwait(false);

                await _service.RedeemInvitationAsync(request.Code).ConfigureAwait(false);
                var identity = await _identityService.GetClaimsIdentityFromUserAsync(user).ConfigureAwait(false);
                return new SignInResponse(identity, invitation.RedirectUri);
            }
            catch (UsernameAlreadyExistsException)
            {
                var errorMessage = _stringResources.GetResource(StringResource.UsernameAlreadyTaken);
                var viewRequest = new InvitationViewRequest(request.Code, request.HeaderOnly, request.ProviderId, errorMessage);
                var response = await ShowAsync(viewRequest).ConfigureAwait(false);
                throw new ErrorResponseException<InvitationViewResponse>(response);
            }
            catch (InvitationNotFoundException)
            {
                var errorMessage = _stringResources.GetResource(StringResource.InvitationCodeInvalidError);
                var response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), urlBuilder.ToString(), null, errorMessage);
                throw new ErrorResponseException<InvitationViewResponse>(response);
            }
            catch (InvitationExpiredException ex)
            {
                if (ex.Invitation.RedirectUri != null)
                {
                    urlBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, ex.Invitation.RedirectUri);
                }

                var errorMessage = _stringResources.GetResource(StringResource.InvitationCodeExpiredError);
                var response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), urlBuilder.ToString(), null, errorMessage);
                throw new ErrorResponseException<InvitationViewResponse>(response);
            }
            catch (InvitationAlreadyRedeemedException)
            {
                var errorMessage = _stringResources.GetResource(StringResource.InvitationCodeRedeemedError);
                var response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), urlBuilder.ToString(), null, errorMessage);
                throw new ErrorResponseException<InvitationViewResponse>(response);
            }
            catch (PasswordChangeException)
            {
                var errorMessage = _stringResources.GetResource(StringResource.PasswordChangeSamePasswordError);
                var response = await ShowAsync(new InvitationViewRequest(request.Code, request.HeaderOnly, request.ProviderId, errorMessage));
                throw new ErrorResponseException<InvitationViewResponse>(response);
            }
        }

        public async Task<InvitationViewResponse> ShowAsync(InvitationViewRequest request)
        {
            var urlBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
            urlBuilder.AppendPath(_identityServerOptions.AccountEndpoint);
            urlBuilder.AppendPath("login");

            InvitationViewResponse response = null;
            try
            {
                var invitation = await _service.GetInvitationAsync(request.Code);

                var user = await _userService.GetUserByIdAsync(invitation.UserId);
                var providerRequest = new ProviderRequest(ProviderRequestType.Invitation, request.HeaderOnly, invitation.RedirectUri, null, user.Name, invitationCode: request.Code, invitation: invitation);

                if (!string.IsNullOrWhiteSpace(request.Provider))
                {
                    providerRequest.ProviderErrors.Add(request.Provider, request.Error);
                }

                var registrationInfoResponse = await _loginService.GetRegistrationInfosAsync(providerRequest);

                response = new InvitationViewResponse(registrationInfoResponse.Groups, null, GetTerms());
            }
            catch (InvitationNotFoundException)
            {
                var errorMessage = _stringResources.GetResource(StringResource.InvitationCodeInvalidError);
                response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), urlBuilder.ToString(), null, errorMessage);
            }
            catch (InvitationExpiredException ex)
            {
                if (ex.Invitation.RedirectUri != null)
                {
                    urlBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, ex.Invitation.RedirectUri);
                }

                var errorMessage = _stringResources.GetResource(StringResource.InvitationCodeExpiredError);
                response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), urlBuilder.ToString(), null, errorMessage);
            }
            catch (InvitationAlreadyRedeemedException)
            {
                var errorMessage = _stringResources.GetResource(StringResource.InvitationCodeRedeemedError);
                response = new InvitationViewResponse(Enumerable.Empty<ILoginRegistrationGroup>(), urlBuilder.ToString(), null, errorMessage);
            }

            return response;
        }

        private Terms GetTerms()
        {
            if (this._identityOptions.Terms != null)
            {
                switch (this._identityOptions.Terms.Mode)
                {
                    case TermsOption.TermsMode.Show:
                        return new Terms(false, GetTermsText(this._identityOptions.Terms.Text));
                    case TermsOption.TermsMode.Confirm:
                        return new Terms(true, GetTermsText(this._identityOptions.Terms.Text));
                    case TermsOption.TermsMode.None:
                    default:
                        return null;
                }
            }

            return null;
        }

        private string GetTermsText(Dictionary<string, string> text)
        {
            var languageCode = _stringResources.GetResource(StringResource.LanguageCode);

            if (text.TryGetValue(languageCode, out var translation))
            {
                return translation;
            }
            else if (text.TryGetValue("en", out var translationEn))
            {
                return translationEn;
            }

            throw new ArgumentException($"No terms text found for language {languageCode} or fallback language en.");
        }
    }
}
