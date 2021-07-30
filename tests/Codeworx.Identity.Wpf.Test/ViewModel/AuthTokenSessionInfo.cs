using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Codeworx.Identity.Wpf.Test.Common;

namespace Codeworx.Identity.Wpf.Test.ViewModel
{
    public class AuthTokenSessionInfo : ISessionInfo
    {
        private string _accessToken;
        private JwtPayload _payload;

        public AuthTokenSessionInfo(string accessToken, JwtPayload payload)
        {
            _accessToken = accessToken;
            _payload = payload;
        }

        public string AccessToken => _accessToken;

        public IDictionary<string, object> Claims => _payload;
    }
}