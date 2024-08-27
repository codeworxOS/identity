﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Invitation;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Resources;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Login.OAuth
{
    public class OAuthRedirectRequestBinder : IRequestBinder<OAuthRedirectRequest>
    {
        private readonly IdentityServerOptions _identityOptions;
        private readonly IInvitationService _invitationService;
        private readonly IStringResources _stringResources;

        public OAuthRedirectRequestBinder(
            IdentityServerOptions options,
            IInvitationService invitationService,
            IStringResources stringResources)
        {
            _identityOptions = options;
            _invitationService = invitationService;
            _stringResources = stringResources;
        }

        public async Task<OAuthRedirectRequest> BindAsync(HttpRequest request)
        {
            if (request.Path.StartsWithSegments($"{_identityOptions.AccountEndpoint}/oauth", out var remaining))
            {
                var providerId = remaining.Value.Trim('/');

                string prompt = null;
                string returnUrl = null;
                string invitationCode = null;
                InvitationItem invitation = null;

                if (providerId.Contains("/"))
                {
                    throw new NotSupportedException($"Invalid uri {request.Path}.");
                }

                if (request.Query.TryGetValue(Constants.InvitationParameter, out var invitationValues))
                {
                    invitationCode = invitationValues;

                    var supported = await _invitationService.IsSupportedAsync().ConfigureAwait(false);

                    if (!supported)
                    {
                        var message = _stringResources.GetResource(StringResource.InvitationNotSupportedError);
                        throw new NotSupportedException(message);
                    }

                    invitation = await _invitationService.GetInvitationAsync(invitationCode).ConfigureAwait(false);
                    returnUrl = invitation.RedirectUri;
                }
                else
                {
                    if (!request.Query.TryGetValue(Constants.ReturnUrlParameter, out var values) || values.Count != 1 || string.IsNullOrWhiteSpace(values[0]))
                    {
                        throw new NotSupportedException("ReturnUrl Parameter is missing from the request.");
                    }

                    returnUrl = values;
                }

                if (request.Query.TryGetValue(Constants.OAuth.PromptName, out var promptValues))
                {
                    prompt = promptValues.FirstOrDefault();
                }

                var result = new OAuthRedirectRequest(providerId, returnUrl, prompt, invitationCode, invitation);

                return result;
            }

            throw new NotSupportedException("Invalid Uri.");
        }
    }
}
