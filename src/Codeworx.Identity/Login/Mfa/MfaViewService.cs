﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Login.Mfa
{
    public class MfaViewService : IMfaViewService
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IdentityOptions _options;

        public MfaViewService(
            IUserService userService,
            ILoginService loginService,
            IServiceProvider serviceProvider,
            IBaseUriAccessor baseUriAccessor,
            IOptionsSnapshot<IdentityOptions> options)
        {
            _userService = userService;
            _loginService = loginService;
            _serviceProvider = serviceProvider;
            _baseUriAccessor = baseUriAccessor;
            _options = options.Value;
        }

        public async Task<SignInResponse> ProcessLoginAsync(MfaProcessLoginRequest request)
        {
            var response = await _loginService.SignInAsync(request.ProviderId, request.ProviderRequestParameter).ConfigureAwait(false);

            return response;
        }

        public async Task<MfaLoginResponse> ShowLoginAsync(MfaLoginRequest request, string errorMessage = null)
        {
            var user = await _userService.GetUserByIdentityAsync(request.Identity);

            if (user == null)
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("user missing!"));
            }

            var requestType = user.LinkedProviders.Contains(request.ProviderId) ? ProviderRequestType.MfaLogin : ProviderRequestType.MfaRegister;

            var providerRequest = new ProviderRequest(requestType, request.ReturnUrl, user: user);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                providerRequest.ProviderErrors.Add(request.ProviderId, errorMessage);
            }

            var registration = await _loginService.GetLoginRegistrationInfoAsync(request.ProviderId, LoginProviderType.MultiFactor).ConfigureAwait(false);
            var processor = (ILoginProcessor)_serviceProvider.GetService(registration.ProcessorType);
            var info = await processor.GetRegistrationInfoAsync(providerRequest, registration).ConfigureAwait(false);

            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
            uriBuilder.AppendPath(_options.AccountEndpoint);
            uriBuilder.AppendPath("login/mfa");

            if (!string.IsNullOrWhiteSpace(request.ReturnUrl))
            {
                uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, request.ReturnUrl);
            }

            var result = new MfaLoginResponse(info, uriBuilder.ToString(), request.ReturnUrl);

            return result;
        }

        public async Task<MfaProviderListResponse> ShowProviderListAsync(MfaProviderListRequest request)
        {
            var user = await _userService.GetUserByIdentityAsync(request.Identity).ConfigureAwait(false);

            if (user == null)
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("user missing!"));
            }

            var providerRequest = new ProviderRequest(ProviderRequestType.MfaList, request.ReturnUrl, user: user);

            var response = await _loginService.GetRegistrationInfosAsync(providerRequest).ConfigureAwait(false);
            return new MfaProviderListResponse(user, response.Groups);
        }
    }
}