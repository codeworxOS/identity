using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class ProfileLinkRequest
    {
        public ProfileLinkRequest(ClaimsIdentity identity, string providerId, LinkDirection direction)
        {
            Identity = identity;
            ProviderId = providerId;
            Direction = direction;
        }

        public ClaimsIdentity Identity { get; }

        public string ProviderId { get; }

        public LinkDirection Direction { get; }
    }
}