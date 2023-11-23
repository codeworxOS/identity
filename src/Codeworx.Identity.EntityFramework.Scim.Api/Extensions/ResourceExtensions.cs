using System.Collections.Generic;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions
{
    public static class ResourceExtensions
    {
        public static void ApplyParameters<TEntity>(this IResource resource, TEntity entity, IEnumerable<ISchemaParameterDescription<TEntity>> parameters)
        {
            foreach (var parameter in parameters)
            {
                var propertyFunc = parameter.Property.Compile();
                if (propertyFunc == null)
                {
                    continue;
                }

                if (!resource.AdditionalProperties.ContainsKey(parameter.Display))
                {
                    resource.AdditionalProperties.Add(parameter.Display, propertyFunc(entity));
                }
                else
                {
                    resource.AdditionalProperties[parameter.Display] = propertyFunc(entity);
                }
            }
        }
    }
}
