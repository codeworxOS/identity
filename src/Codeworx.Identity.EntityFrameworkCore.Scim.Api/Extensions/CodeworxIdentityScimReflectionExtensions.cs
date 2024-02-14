using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace System.Reflection
{
    public static class CodeworxIdentityScimReflectionExtensions
    {
        public static string GetJsonName(this MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<JsonPropertyNameAttribute>();

            if (attribute != null)
            {
                return attribute.Name;
            }

            return $"{member.Name.Substring(0, 1).ToLower()}{member.Name.Substring(1)}";
        }

        public static IEnumerable<Type> Flatten(this Type type)
        {
            yield return type;
            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }
        }

        public static bool IsEnumerable(this Type parent)
        {
            return parent.IsEnumerable(out var elementType);
        }

        public static bool IsEnumerable(this Type parent, out Type? elementType)
        {
            if (parent == typeof(string))
            {
                elementType = null;
                return false;
            }

            foreach (var type in Flatten(parent))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    elementType = type.GetGenericArguments()[0];
                    return true;
                }
            }

            elementType = null;
            return false;
        }
    }
}
