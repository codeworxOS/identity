using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
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
        private readonly ITokenCache _tokenCache;
        private readonly IDefaultSigningDataProvider _defaultSigningDataProvider;
        private string _key;

        private DateTimeOffset _validFrom;

        public Jwt(TokenType tokenType, ITokenCache tokenCache, IDefaultSigningDataProvider defaultSigningDataProvider, JwtConfiguration configuration)
        {
            TokenType = tokenType;
            _tokenCache = tokenCache;
            _defaultSigningDataProvider = defaultSigningDataProvider;
            _configuration = configuration;

            _handler = new JsonWebTokenHandler();
        }

        public IdentityData IdentityData { get; private set; }

        public TokenType TokenType { get; }

        public DateTimeOffset ValidUntil { get; private set; }

        public async Task ParseAsync(string value, CancellationToken token = default)
        {
            if (!_handler.CanReadToken(value))
            {
                throw new SecurityTokenException($"Parameter {nameof(value)} is not a valid token.");
            }

            var jwtToken = _handler.ReadJsonWebToken(value);

            if (TokenType != TokenType.IdToken)
            {
                if (jwtToken.TryGetClaim(Constants.Claims.Trk, out var claim))
                {
                    _key = claim.Value;
                    var entry = await _tokenCache.GetAsync(TokenType, _key, token).ConfigureAwait(false);
                    IdentityData = entry.IdentityData;
                    ValidUntil = entry.ValidUntil;
                }
            }
        }

        public async Task<string> SerializeAsync(CancellationToken token = default)
        {
            var data = IdentityData ?? throw new ArgumentNullException(nameof(IdentityData));
            var payload = IdentityData.GetTokenClaims(GetClaimTarget());

            if (TokenType != TokenType.IdToken)
            {
                if (_key == null)
                {
                    _key = await _tokenCache.SetAsync(TokenType, data, ValidUntil, token).ConfigureAwait(false);
                }

                payload.Add(Constants.Claims.Trk, _key);
            }

            payload.TryGetValue(Constants.Claims.Issuer, out var issuer);

            var signingData = await _defaultSigningDataProvider.GetSigningDataAsync(token);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer?.ToString(),
                Audience = data.ClientId,
                Claims = payload,
                Expires = ValidUntil.UtcDateTime,
                IssuedAt = _validFrom.UtcDateTime,
                NotBefore = _validFrom.UtcDateTime,
                SigningCredentials = signingData.Credentials,
            };

            return _handler.CreateToken(descriptor);
        }

        public Task SetPayloadAsync(IdentityData identityData, DateTimeOffset validUntil, CancellationToken token = default)
        {
            IdentityData = identityData;
            _validFrom = DateTimeOffset.UtcNow;
            ValidUntil = validUntil;
            return Task.CompletedTask;
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

        private ClaimTarget GetClaimTarget()
        {
            switch (TokenType)
            {
                case TokenType.AccessToken:
                    return ClaimTarget.AccessToken;
                case TokenType.IdToken:
                    return ClaimTarget.IdToken;
                case TokenType.RefreshToken:
                default:
                    throw new NotSupportedException();
            }
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