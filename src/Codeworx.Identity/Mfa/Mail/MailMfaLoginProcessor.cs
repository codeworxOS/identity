using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Mfa.Mail
{
    public class MailMfaLoginProcessor : ILoginProcessor
    {
        private readonly IdentityOptions _options;
        private readonly IUserService _userService;
        private readonly IStringResources _stringResources;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public MailMfaLoginProcessor(
            IUserService userService,
            IStringResources stringResources,
            IBaseUriAccessor baseUriAccessor,
            IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
            _userService = userService;
            _stringResources = stringResources;
            _baseUriAccessor = baseUriAccessor;
        }

        public Type RequestParameterType { get; } = typeof(MailLoginRequest);

        public async Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration registration)
        {
            string error = null;
            request.ProviderErrors.TryGetValue(registration.Id, out error);

            switch (request.Type)
            {
                case ProviderRequestType.MfaLogin:
                    var email = await _userService.GetProviderValueAsync(request.User.Identity, registration.Id).ConfigureAwait(false);
                    if (email == null)
                    {
                        return null;
                    }

                    return new MailRegistrationInfo(registration.Id, email, error);
                case ProviderRequestType.MfaRegister:
                    return new RegisterMailRegistrationInfo(registration.Id, error);
                case ProviderRequestType.MfaList:
                    if (request.User.LinkedProviders.Contains(registration.Id))
                    {
                        var address = await _userService.GetProviderValueAsync(request.User.Identity, registration.Id).ConfigureAwait(false);
                        if (address == null)
                        {
                            return null;
                        }

                        return GetMfaListRegistrationInfo(request, registration, address);
                    }

                    return null;
                case ProviderRequestType.Login:
                case ProviderRequestType.Invitation:
                case ProviderRequestType.Profile:
                default:
                    return null;
            }
        }

        public Task<SignInResponse> ProcessAsync(ILoginRegistration configuration, object request)
        {
            throw new NotImplementedException();
        }

        private ILoginRegistrationInfo GetMfaListRegistrationInfo(ProviderRequest request, ILoginRegistration registration, string email)
        {
            request.ProviderErrors.TryGetValue(registration.Id, out var error);

            var masked = MailRegistrationInfo.Mask(email);
            var description = string.Format(_stringResources.GetResource(StringResource.OneTimeCodeViaEmail), masked);

            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
            uriBuilder.AppendPath(_options.AccountEndpoint);
            uriBuilder.AppendPath("login/mfa");
            uriBuilder.AppendPath(registration.Id);

            if (!string.IsNullOrWhiteSpace(request.ReturnUrl))
            {
                uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, request.ReturnUrl);
            }

            return new MfaProviderListInfo(registration.Id, uriBuilder.ToString(), description, "fa-solid fa-at", error);
        }
    }
}
