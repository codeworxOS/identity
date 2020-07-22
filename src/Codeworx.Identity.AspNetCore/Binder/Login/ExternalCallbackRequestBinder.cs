using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Login
{
    public class ExternalCallbackRequestBinder : IRequestBinder<ExternalCallbackRequest>
    {
        private static readonly ConcurrentDictionary<Type, Func<IServiceProvider, HttpRequest, Task<ILoginRequest>>> _parameterTypeCache;
        private static readonly MethodInfo _bindMethod;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILoginService _loginService;
        private readonly IdentityOptions _identityOptions;

        static ExternalCallbackRequestBinder()
        {
            _parameterTypeCache = new ConcurrentDictionary<Type, Func<IServiceProvider, HttpRequest, Task<ILoginRequest>>>();

            Expression<Func<IServiceProvider, HttpRequest, Task<ILoginRequest>>> exp = (sp, req) => BindLoginRequest<ILoginRequest>(sp, req);
            _bindMethod = ((MethodCallExpression)exp.Body).Method.GetGenericMethodDefinition();
        }

        public ExternalCallbackRequestBinder(IServiceProvider serviceProvider, ILoginService loginService, IOptionsSnapshot<IdentityOptions> options)
        {
            _serviceProvider = serviceProvider;
            _loginService = loginService;
            _identityOptions = options.Value;
        }

        public async Task<ExternalCallbackRequest> BindAsync(HttpRequest request)
        {
            if (request.Path.StartsWithSegments($"{_identityOptions.AccountEndpoint}/callback", out var remaining))
            {
                var providerId = remaining.Value.Trim('/');

                if (providerId.Contains("/"))
                {
                    throw new NotSupportedException($"Invalid uri {request.Path}.");
                }

                var parameterType = await _loginService.GetParameterTypeAsync(providerId);

                if (parameterType == null)
                {
                    throw new InvalidOperationException($"Provider {providerId} not found!");
                }

                var factory = _parameterTypeCache.GetOrAdd(parameterType, CreateFactory);
                var loginRequest = await factory(_serviceProvider, request).ConfigureAwait(false);

                return new ExternalCallbackRequest(loginRequest, providerId);
            }

            throw new NotSupportedException("Invalid Uri.");
        }

        private static async Task<ILoginRequest> BindLoginRequest<T>(IServiceProvider sp, HttpRequest request)
            where T : ILoginRequest
        {
            var binder = sp.GetRequiredService<IRequestBinder<T>>();
            return await binder.BindAsync(request).ConfigureAwait(false);
        }

        private static Func<IServiceProvider, HttpRequest, Task<ILoginRequest>> CreateFactory(Type key)
        {
            var serviceProviderParam = Expression.Parameter(typeof(IServiceProvider), "sp");
            var requestParam = Expression.Parameter(typeof(HttpRequest), "req");

            var expression = Expression.Lambda<Func<IServiceProvider, HttpRequest, Task<ILoginRequest>>>(
                                    Expression.Call(
                                            _bindMethod.MakeGenericMethod(key),
                                            serviceProviderParam,
                                            requestParam),
                                    serviceProviderParam,
                                    requestParam);

            return expression.Compile();
        }
    }
}
