using System.Linq;
using System.Text.Json;
using Codeworx.Identity.EntityFrameworkCore.Api.Model;
#if !NET6_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Extensions
{
    public static class CodeworxIdentityEntityFrameworkCoreApiEfExtensions
    {
        public static void MapAdditionalProperties<TEntity>(this EntityEntry<TEntity> entry, IExtendableObject target)
            where TEntity : class
        {
            foreach (var item in entry.Metadata.GetProperties().Where(p => p.IsShadowProperty()))
            {
#if NET6_0_OR_GREATER
                if (item == entry.Metadata.FindDiscriminatorProperty())
#else
                if (item == entry.Metadata.GetDiscriminatorProperty())
#endif
                {
                    continue;
                }

                var value = entry.CurrentValues[item];
                target.AdditionalProperties.Add(item.Name.ToLower(), value);
            }
        }

        public static void UpdateAdditionalProperties<TEntity>(this EntityEntry<TEntity> entry, IExtendableObject target)
           where TEntity : class
        {
            foreach (var item in entry.Metadata.GetProperties().Where(p => p.IsShadowProperty()))
            {
#if NET6_0_OR_GREATER
                if (item == entry.Metadata.FindDiscriminatorProperty())
#else
                if (item == entry.Metadata.GetDiscriminatorProperty())
#endif
                {
                    continue;
                }

                if (target.AdditionalProperties.TryGetValue(item.Name.ToLower(), out var value))
                {
                    if (value is JsonElement json)
                    {
                        value = System.Text.Json.JsonSerializer.Deserialize(json.GetRawText(), item.ClrType);
                    }

                    entry.CurrentValues[item] = value;
                }
            }
        }
    }
}
