using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Token;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Codeworx.Identity.Cryptography.Json
{
    public class Jwt : IToken
    {
        private readonly JwtConfiguration _configuration;
        private readonly IDefaultSigningKeyProvider _defaultSigningKeyProvider;
        private readonly JsonWebTokenHandler _handler;
        private readonly SecurityKey _signingKey;
        private TimeSpan _expiration;
        private IDictionary<string, object> _payload;

        public Jwt(IDefaultSigningKeyProvider defaultSigningKeyProvider, JwtConfiguration configuration)
        {
            _defaultSigningKeyProvider = defaultSigningKeyProvider;
            _configuration = configuration;
            _signingKey = _defaultSigningKeyProvider.GetKey();

            _handler = new JsonWebTokenHandler();
        }

        public Task<IDictionary<string, object>> GetPayloadAsync()
        {
            return Task.FromResult(_payload);
        }

        public async Task ParseAsync(string value)
        {
            if (!_handler.CanReadToken(value))
            {
                throw new ArgumentException($"Parameter {nameof(value)} is not a valid token.");
            }

            var token = _handler.ReadJsonWebToken(value);

            var decode = Base64UrlEncoder.Decode(token.EncodedPayload);

            _payload = JsonConvert.DeserializeObject<ConcurrentDictionary<string, object>>(decode);

            await Task.CompletedTask;
        }

        public Task<string> SerializeAsync()
        {
            var descriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = GetSigningCredentials(),
                Claims = _payload,
                Expires = DateTime.Now + _expiration,
                IssuedAt = DateTime.Now
            };

            return Task.FromResult(_handler.CreateToken(descriptor));
        }

        public async Task SetPayloadAsync(IDictionary<string, object> data, TimeSpan expiration)
        {
            await Task.CompletedTask;

            _payload = data;
            _expiration = expiration;
        }

        public Task<bool> ValidateAsync()
        {
            throw new NotImplementedException();
        }

        private static async Task WriterObjectAsync(IDictionary<string, object> data, JsonTextWriter writer)
        {
            await writer.WriteStartObjectAsync().ConfigureAwait(false);

            foreach (var item in data)
            {
                await WriteValueAsync(writer, item).ConfigureAwait(false);
            }

            await writer.WriteEndObjectAsync().ConfigureAwait(false);
        }

        private static async Task WriteValueAsync(JsonTextWriter writer, KeyValuePair<string, object> item)
        {
            await writer.WritePropertyNameAsync(item.Key).ConfigureAwait(false);

            switch (item.Value)
            {
                case string[] arrayValue:
                    if (arrayValue.Length > 1)
                    {
                        await writer.WriteStartArrayAsync().ConfigureAwait(false);
                    }

                    foreach (var value in arrayValue)
                    {
                        await writer.WriteValueAsync(value).ConfigureAwait(false);
                    }

                    if (arrayValue.Length > 1)
                    {
                        await writer.WriteEndArrayAsync().ConfigureAwait(false);
                    }

                    break;

                case string stringValue:
                    await writer.WriteValueAsync(stringValue).ConfigureAwait(false);
                    break;

                case IDictionary<string, object> subObject:
                    await WriterObjectAsync(subObject, writer);
                    break;

                default:
                    throw new NotSupportedException($"Type {item.Value?.GetType()} not supported as Payload Value. Allowed Value Types are (string, string[], IDictionary<string,object>).");
            }
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