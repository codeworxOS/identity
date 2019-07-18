using System;
using Codeworx.Identity.Token;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography.Json
{
    public class Jwt : IToken
    {
        private readonly JwtConfiguration _configuration;
        private readonly IDefaultSigningKeyProvider _defaultSigningKeyProvider;
        private readonly JsonWebTokenHandler _handler;
        private readonly SecurityKey _signingKey;
        private string _payload;

        public Jwt(IDefaultSigningKeyProvider defaultSigningKeyProvider, JwtConfiguration configuration)
        {
            _defaultSigningKeyProvider = defaultSigningKeyProvider;
            _configuration = configuration;
            _signingKey = _defaultSigningKeyProvider.GetKey();

            _handler = new JsonWebTokenHandler();
        }

        public IdentityData GetPayload()
        {
            throw new NotImplementedException();
        }

        public void Parse(string value)
        {
            throw new NotImplementedException();
        }

        public string Serialize()
        {
            return _handler.CreateToken(_payload, GetSigningCredentials());
        }

        public void SetPayload(IdentityData data, TokenType tokenType)
        {
            _payload = "{ \"sub\": \"whatever\"}";
        }

        public bool Validate()
        {
            throw new NotImplementedException();
        }

        private SigningCredentials GetSigningCredentials()
        {
            string algorithm = null;

            switch (_signingKey)
            {
                case ECDsaSecurityKey ecd:
                    algorithm = $"ES{ecd.KeySize}";
                    break;

                case RsaSecurityKey rsa:
                    algorithm = $"RS256";
                    break;
                default:
                    throw new NotSupportedException("provided Signing Key is not supported!");
            }

            return new SigningCredentials(_signingKey, algorithm);
        }
    }
}
