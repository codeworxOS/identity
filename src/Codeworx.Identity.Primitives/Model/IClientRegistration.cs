using System;
using System.Collections.Generic;

namespace Codeworx.Identity.Model
{
    public interface IClientRegistration
    {
        string ClientId { get; }

        byte[] ClientSecretHash { get; }

        byte[] ClientSecretSalt { get; }

        IReadOnlyList<string> SupportedFlow { get; }

        TimeSpan TokenExpiration { get; }

        IReadOnlyList<string> ValidRedirectUrls { get; }
    }
}