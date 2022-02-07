using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Token;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Codeworx.Identity.Cryptography.Json
{
    public class Jwt : IToken
    {
        private readonly JwtConfiguration _configuration;
        private readonly JsonWebTokenHandler _handler;
        private readonly SecurityKey _signingKey;
        private TimeSpan _expiration;
        private IDictionary<string, object> _payload;

        public Jwt(IDefaultSigningKeyProvider defaultSigningKeyProvider, JwtConfiguration configuration)
        {
            _configuration = configuration;
            _signingKey = defaultSigningKeyProvider.GetKey();

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
                throw new SecurityTokenException($"Parameter {nameof(value)} is not a valid token.");
            }

            var token = _handler.ReadJsonWebToken(value);
            var decode = Base64UrlEncoder.Decode(token.EncodedPayload);

            _payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(decode, new ArraySubObjectConverter());

            await Task.CompletedTask;
        }

        public Task<string> SerializeAsync()
        {
            _payload.TryGetValue(Constants.Claims.Issuer, out var issuer);
            _payload.TryGetValue(Constants.Claims.Audience, out var audience);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer?.ToString(),
                Audience = audience?.ToString(),
                Claims = _payload,
                Expires = DateTime.UtcNow + _expiration,
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                SigningCredentials = GetSigningCredentials(),
            };

            return Task.FromResult(_handler.CreateToken(descriptor));
        }

        public Task SetPayloadAsync(IDictionary<string, object> data, TimeSpan expiration)
        {
            _payload = data;
            _expiration = expiration;

            return Task.CompletedTask;
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

        private class ArraySubObjectConverter : JsonConverter
        {
            public override bool CanWrite => false;

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(object);
            }

            public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
#pragma warning disable IDE0010 // Add missing cases
                switch (reader.TokenType)
#pragma warning restore IDE0010 // Add missing cases
                {
                    case Newtonsoft.Json.JsonToken.StartArray:
                        return JToken.Load(reader).ToObject<List<object>>();
                    case Newtonsoft.Json.JsonToken.StartObject:
                        return JToken.Load(reader).ToObject<Dictionary<string, object>>();
                    default:
                        if (reader.ValueType == null && reader.TokenType != Newtonsoft.Json.JsonToken.Null)
                        {
                            throw new NotImplementedException("Token not supported!");
                        }

                        return reader.Value;
                }
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotSupportedException("Read Only Converter.");
            }
        }
    }
}