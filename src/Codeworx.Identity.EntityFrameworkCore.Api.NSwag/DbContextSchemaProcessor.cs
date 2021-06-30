using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Codeworx.Identity.EntityFrameworkCore.Api.NSwag
{
    public class DbContextSchemaProcessor<TContext> : ISchemaProcessor
        where TContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ImmutableDictionary<Type, Type> _mapping;

        public DbContextSchemaProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _mapping = _serviceProvider.GetServices<IAdditionalDataEntityMapping>().ToImmutableDictionary(p => p.Target, p => p.Entity);
        }

        public void Process(SchemaProcessorContext context)
        {
            if (_mapping.TryGetValue(context.Type, out var entity))
            {
                IEntityType entityType;

                using (var scope = _serviceProvider.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<TContext>();
                    entityType = ctx.Model.FindEntityType(entity);
                }

                foreach (var property in entityType.GetProperties().Where(p => p.IsShadowProperty()))
                {
                    if (property == entityType.GetDiscriminatorProperty())
                    {
                        continue;
                    }

                    var jsonProperty = context.Generator.Generate<JsonSchemaProperty>(property.ClrType, context.Resolver);

                    jsonProperty.IsNullableRaw = property.IsNullable;
                    jsonProperty.IsRequired = !property.IsNullable;
                    jsonProperty.MaxLength = property.GetMaxLength();

                    context.Schema.Properties.Add(property.Name.ToLower(), jsonProperty);
                }

                context.Schema.AllowAdditionalProperties = false;
                context.Schema.AdditionalPropertiesSchema = null;
            }
        }
    }
}