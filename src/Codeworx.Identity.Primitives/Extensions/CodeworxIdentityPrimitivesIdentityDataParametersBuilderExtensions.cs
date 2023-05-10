﻿using System;
using System.Linq.Expressions;
using System.Security.Claims;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth.Token;
using Codeworx.Identity.Token;

namespace Codeworx.Identity
{
    public static class CodeworxIdentityPrimitivesIdentityDataParametersBuilderExtensions
    {
        public static void SetValue<TParameters, TValue>(this IIdentityDataParametersBuilder<TParameters> builder, Expression<Func<TParameters, TValue>> propertySelector, TValue value)
            where TParameters : IIdentityDataParameters
        {
            builder.SetValue(((MemberExpression)propertySelector.Body).Member.Name, value);
        }

        public static TBuilder WithActorToken<TBuilder>(this TBuilder builder, string actorToken)
            where TBuilder : IIdentityDataParametersBuilder<ITokenExchangeParameters>
        {
            builder.SetValue(p => p.ActorToken, actorToken);
            return builder;
        }

        public static TBuilder WithActorTokenType<TBuilder>(this TBuilder builder, string actorTokenType)
            where TBuilder : IIdentityDataParametersBuilder<ITokenExchangeParameters>
        {
            builder.SetValue(p => p.ActorTokenType, actorTokenType);
            return builder;
        }

        public static TBuilder WithRequestedTokenTypes<TBuilder>(this TBuilder builder, string[] requestedTokenTypes)
    where TBuilder : IIdentityDataParametersBuilder<ITokenExchangeParameters>
        {
            builder.SetValue(p => p.RequestedTokenTypes, requestedTokenTypes);
            return builder;
        }

        public static TBuilder WithAudience<TBuilder>(this TBuilder builder, string audience)
            where TBuilder : IIdentityDataParametersBuilder<ITokenExchangeParameters>
        {
            builder.SetValue(p => p.Audience, audience);
            return builder;
        }

        public static TBuilder WithParsedRefreshToken<TBuilder>(this TBuilder builder, IToken parsedRefreshToken)
          where TBuilder : IIdentityDataParametersBuilder<IRefreshTokenParameters>
        {
            builder.SetValue(p => p.ParsedRefreshToken, parsedRefreshToken);
            return builder;
        }

        public static TBuilder WithClient<TBuilder>(this TBuilder builder, IClientRegistration client)
                                            where TBuilder : IIdentityDataParametersBuilder<IIdentityDataParameters>
        {
            builder.SetValue(p => p.Client, client);
            return builder;
        }

        public static TBuilder WithNonce<TBuilder>(this TBuilder builder, string nonce)
            where TBuilder : IIdentityDataParametersBuilder<IAuthorizationParameters>
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

        public static TBuilder WithRefreshTokenUser<TBuilder>(this TBuilder builder, ClaimsIdentity user, IUser identityUser)
where TBuilder : IIdentityDataParametersBuilder<IRefreshTokenParameters>
        {
            builder.SetValue(p => p.User, user);
            builder.SetValue(p => p.IdentityUser, identityUser);
            return builder;
        }

        public static TBuilder WithTokenExchangeUser<TBuilder>(this TBuilder builder, ClaimsIdentity user, IUser identityUser)
    where TBuilder : IIdentityDataParametersBuilder<ITokenExchangeParameters>
        {
            builder.SetValue(p => p.User, user);
            builder.SetValue(p => p.IdentityUser, identityUser);
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
                                                                                    where TBuilder : IIdentityDataParametersBuilder<IAuthorizationParameters>
        {
            builder.SetValue(p => p.State, state);
            return builder;
        }

        public static TBuilder WithSubjectToken<TBuilder>(this TBuilder builder, string subjectToken)
    where TBuilder : IIdentityDataParametersBuilder<ITokenExchangeParameters>
        {
            builder.SetValue(p => p.SubjectToken, subjectToken);
            return builder;
        }

        public static TBuilder WithSubjectTokenType<TBuilder>(this TBuilder builder, string subjectTokenType)
            where TBuilder : IIdentityDataParametersBuilder<ITokenExchangeParameters>
        {
            builder.SetValue(p => p.SubjectTokenType, subjectTokenType);
            return builder;
        }

        public static TBuilder WithUser<TBuilder>(this TBuilder builder, ClaimsIdentity user)
            where TBuilder : IIdentityDataParametersBuilder<IIdentityDataParameters>
        {
            builder.SetValue(p => p.User, user);
            return builder;
        }

        public static TBuilder WithUser<TBuilder>(this TBuilder builder, ClaimsIdentity user, IUser identityUser)
            where TBuilder : IIdentityDataParametersBuilder<IIdentityDataParameters>
        {
            builder.SetValue(p => p.User, user);
            builder.SetValue(p => p.IdentityUser, identityUser);
            return builder;
        }
    }
}
