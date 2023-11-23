using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions
{
    public static class EntityExtensions
    {
        public static void ApplyParameters<TEntity>(this TEntity entity, IResource resource, IEnumerable<ISchemaParameterDescription<TEntity>> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (resource.AdditionalProperties.ContainsKey(parameter.Display))
                {
                    var value = GetValue(resource.AdditionalProperties[parameter.Display], parameter.Display, GetPropertyType(parameter.Property));
                    entity.SetPropertyValue(parameter.Property, value);
                }
            }
        }

        private static void SetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberLamda, TValue value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(target, value, null);
                }
            }
        }

        private static Type GetPropertyType<T, TValue>(Expression<Func<T, TValue>> memberLamda)
        {
            return ((memberLamda.Body as MemberExpression)?.Member as PropertyInfo)?.PropertyType;
        }

        private static object GetValue(object value, string parameter, Type targetType)
        {
            if (value is JsonElement element)
            {
                if (element.ValueKind == JsonValueKind.Object)
                {
                    string[] split = parameter.Split(".");
                    foreach (var p in split)
                    {
                        element = element.GetProperty(p);
                    }
                }

                return JsonSerializer.Deserialize(element, targetType);
            }
            else
            {
                return value;
            }
        }
    }
}
