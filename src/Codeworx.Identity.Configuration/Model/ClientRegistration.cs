﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Configuration.Model
{
    public class ClientRegistration : IClientRegistration
    {
        public ClientRegistration(string clientId, string clientSecretHash, ClientType clientType, TimeSpan tokenExpiration, string accessTokenType, string accessTokenTypeConfiguration, IEnumerable<string> validRedirectUrls = null, IUser user = null, bool allowScim = false)
        {
            ClientId = clientId;
            ClientSecretHash = clientSecretHash;
            ClientType = clientType;
            TokenExpiration = tokenExpiration;
            AccessTokenType = accessTokenType;
            AccessTokenTypeConfiguration = accessTokenTypeConfiguration;
            AllowScim = allowScim;

            if (validRedirectUrls != null)
            {
                ValidRedirectUrls = validRedirectUrls.Select(p => new Uri(p)).ToImmutableList();
            }
            else
            {
                ValidRedirectUrls = ImmutableList.Create<Uri>();
            }

            User = user;
        }

        public string ClientId { get; }

        public string ClientSecretHash { get; }

        public TimeSpan TokenExpiration { get; }

        public IReadOnlyList<Uri> ValidRedirectUrls { get; }

        public ClientType ClientType { get; }

        public IUser User { get; }

        public AuthenticationMode AuthenticationMode { get; }

        public string AccessTokenType { get; }

        public string AccessTokenTypeConfiguration { get; }

        public bool AllowScim { get; }
    }
}