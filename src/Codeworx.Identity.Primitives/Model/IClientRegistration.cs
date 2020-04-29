using System;
using System.Collections.Generic;

namespace Codeworx.Identity.Model
{
    public interface IClientRegistration
    {
        string ClientId { get; }

        byte[] ClientSecretHash { get; }

        byte[] ClientSecretSalt { get; }

        ClientType ClientType { get; }

        TimeSpan TokenExpiration { get; }

        IReadOnlyList<Uri> ValidRedirectUrls { get; }
    }
}