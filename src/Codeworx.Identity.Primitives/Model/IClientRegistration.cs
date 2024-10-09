using System;
using System.Collections.Generic;
using Codeworx.Identity.Login;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.Model
{
    public interface IClientRegistration
    {
        string ClientId { get; }

        string ClientSecretHash { get; }

        string AccessTokenType { get; }

        string AccessTokenTypeConfiguration { get; }

        bool AllowScim { get; }

        ClientType ClientType { get; }

        TimeSpan TokenExpiration { get; }

        RefreshTokenLifetime? RefreshTokenLifetime { get; }

        TimeSpan? RefreshTokenExpiration { get; }

        IReadOnlyList<Uri> ValidRedirectUrls { get; }

        IUser User { get; }

        AuthenticationMode AuthenticationMode { get; }
    }
}