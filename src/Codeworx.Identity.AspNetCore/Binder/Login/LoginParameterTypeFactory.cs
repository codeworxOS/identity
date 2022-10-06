using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.AspNetCore.Binder.Login
{
    public static class LoginParameterTypeFactory
    {
        private static readonly MethodInfo _bindMethod;
        private static readonly ConcurrentDictionary<Type, Func<IServiceProvider, HttpRequest, Task<object>>> _parameterTypeCache;

        static LoginParameterTypeFactory()
        {
            _parameterTypeCache = new ConcurrentDictionary<Type, Func<IServiceProvider, HttpRequest, Task<object>>>();

            Expression<Func<IServiceProvider, HttpRequest, Task<object>>> exp = (sp, req) => BindLoginRequest<object>(sp, req);
            _bindMethod = ((MethodCallExpression)exp.Body).Method.GetGenericMethodDefinition();
        }

        public static Func<IServiceProvider, HttpRequest, Task<object>> GetFactory(Type parameterType)
        {
            return _parameterTypeCache.GetOrAdd(parameterType, CreateFactory);
        }

        private static async Task<object> BindLoginRequest<T>(IServiceProvider sp, HttpRequest request)
        {
            var binder = sp.GetRequiredService<IRequestBinder<T>>();
            var validator = sp.GetService<IRequestValidator<T>>();

            var result = await binder.BindAsync(request).ConfigureAwait(false);

            if (validator != null)
            {
                await validator.ValidateAsync(result).ConfigureAwait(false);
            }

            return result;
        }

        private static Func<IServiceProvider, HttpRequest, Task<object>> CreateFactory(Type key)
        {
            var serviceProviderParam = Expression.Parameter(typeof(IServiceProvider), "sp");
            var requestParam = Expression.Parameter(typeof(HttpRequest), "req");

            var expression = Expression.Lambda<Func<IServiceProvider, HttpRequest, Task<object>>>(
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
