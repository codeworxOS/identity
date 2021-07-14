using System;
using System.Linq.Expressions;
using System.Security.Claims;
using Codeworx.Identity.Cache;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity
{
    public static class CodeworxIdentityPrimitivesIdentityDataParametersBuilderExtensions
    {
        public static void SetValue<TParameters, TValue>(this IIdentityDataParametersBuilder<TParameters> builder, Expression<Func<TParameters, TValue>> propertySelector, TValue value)
            where TParameters : IIdentityDataParameters
        {
            builder.SetValue(((MemberExpression)propertySelector.Body).Member.Name, value);
        }

        public static TBuilder WithClientId<TBuilder>(this TBuilder builder, string clientId)
            where TBuilder : IIdentityDataParametersBuilder<IIdentityDataParameters>
        {
            builder.SetValue(p => p.ClientId, clientId);
            return builder;
        }

        public static TBuilder WithClientSecret<TBuilder>(this TBuilder builder, string clientSecret)
            where TBuilder : IIdentityDataParametersBuilder<IClientCredentialsParameters>
        {
            builder.SetValue(p => p.ClientSecret, clientSecret);
            return builder;
        }

        public static TBuilder WithRefreshTokenClientSecret<TBuilder>(this TBuilder builder, string clientSecret)
          where TBuilder : IIdentityDataParametersBuilder<IRefreshTokenParameters>
        {
            builder.SetValue(p => p.ClientSecret, clientSecret);
            return builder;
        }

        public static TBuilder WithCacheItem<TBuilder>(this TBuilder builder, IRefreshTokenCacheItem cacheItem)
          where TBuilder : IIdentityDataParametersBuilder<IRefreshTokenParameters>
        {
            builder.SetValue(p => p.CacheItem, cacheItem);
            return builder;
        }

        public static TBuilder WithNonce<TBuilder>(this TBuilder builder, string nonce)
            where TBuilder : IIdentityDataParametersBuilder<IIdentityDataParameters>
        {
            builder.SetValue(p => p.Nonce, nonce);
            return builder;
        }

        public static TBuilder WithPrompts<TBuilder>(this TBuilder builder, params string[] prompts)
            where TBuilder : IIdentityDataParametersBuilder<IAuthorizationParameters>
        {
            builder.SetValue(p => p.Prompts, prompts);
            return builder;
        }

        public static TBuilder WithRedirectUri<TBuilder>(this TBuilder builder, string redirectUri)
            where TBuilder : IIdentityDataParametersBuilder<IAuthorizationParameters>
        {
            builder.SetValue(p => p.RedirectUri, redirectUri);
            return builder;
        }

        public static TBuilder WithRefreshToken<TBuilder>(this TBuilder builder, string refreshToken)
            where TBuilder : IIdentityDataParametersBuilder<IRefreshTokenParameters>
        {
            builder.SetValue(p => p.RefreshToken, refreshToken);
            return builder;
        }

        public static TBuilder WithResponseMode<TBuilder>(this TBuilder builder, string responseMode)
                    where TBuilder : IIdentityDataParametersBuilder<IAuthorizationParameters>
        {
            builder.SetValue(p => p.ResponseMode, responseMode);
            return builder;
        }

        public static TBuilder WithResponseTypes<TBuilder>(this TBuilder builder, params string[] responseTypes)
            where TBuilder : IIdentityDataParametersBuilder<IAuthorizationParameters>
        {
            builder.SetValue(p => p.ResponseTypes, responseTypes);
            return builder;
        }

        public static TBuilder WithScopes<TBuilder>(this TBuilder builder, params string[] scopes)
            where TBuilder : IIdentityDataParametersBuilder<IIdentityDataParameters>
        {
            builder.SetValue(p => p.Scopes, scopes);
            return builder;
        }

        public static TBuilder WithState<TBuilder>(this TBuilder builder, string state)
                                                                                    where TBuilder : IIdentityDataParametersBuilder<IIdentityDataParameters>
        {
            builder.SetValue(p => p.State, state);
            return builder;
        }

        public static TBuilder WithTokenExpiration<TBuilder>(this TBuilder builder, TimeSpan tokenExpiration)
             where TBuilder : IIdentityDataParametersBuilder<IClientCredentialsParameters>
        {
            builder.SetValue(p => p.TokenExpiration, tokenExpiration);
            return builder;
        }

        public static TBuilder WithUser<TBuilder>(this TBuilder builder, ClaimsIdentity user)
            where TBuilder : IIdentityDataParametersBuilder<IClientCredentialsParameters>
        {
            builder.SetValue(p => p.User, user);
            return builder;
        }

        public static TBuilder WithRefreshTokenUser<TBuilder>(this TBuilder builder, ClaimsIdentity user)
    where TBuilder : IIdentityDataParametersBuilder<IRefreshTokenParameters>
        {
            builder.SetValue(p => p.User, user);
            return builder;
        }
    }
}
