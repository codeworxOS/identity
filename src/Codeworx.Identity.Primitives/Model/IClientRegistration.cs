using System;
using System.Collections.Generic;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Model
{
    public interface IClientRegistration
    {
        string ClientId { get; }

        string ClientSecretHash { get; }

        string AccessTokenType { get; }

        string AccessTokenTypeConfiguration { get; }

        ClientType ClientType { get; }

        TimeSpan TokenExpiration { get; }

        IReadOnlyList<Uri> ValidRedirectUrls { get; }

        IUser User { get; }

        AuthenticationMode AuthenticationMode { get; }
    }
}