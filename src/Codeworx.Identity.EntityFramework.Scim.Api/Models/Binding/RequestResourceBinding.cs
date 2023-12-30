using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Binding
{
    public class RequestResourceBinding : IModelBinder
    {
        private readonly IHttpRequestStreamReaderFactory _reader;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<MvcOptions> _mvcOptions;
        private readonly IEnumerable<ISchemaExtension> _schemaExtensions;

        public RequestResourceBinding(IHttpRequestStreamReaderFactory reader, ILoggerFactory loggerFactory, IOptions<MvcOptions> mvcOptions, IEnumerable<ISchemaExtension> schemaExtensions)
        {
            _reader = reader;
            _loggerFactory = loggerFactory;
            _mvcOptions = mvcOptions;
            _schemaExtensions = schemaExtensions;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var binder = CreateModelBinder(bindingContext.ModelType);
            await binder.BindModelAsync(bindingContext);
        }

        private BodyModelBinder CreateModelBinder(Type type)
        {
            var options = new JsonOptions
            {
                JsonSerializerOptions =
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
                },
            };

            foreach (var extension in _schemaExtensions)
            {
                options.JsonSerializerOptions.Converters.Add(new ScimSchemaConverter(extension.Schema, extension.TargetType));
            }

            var formatter = new SystemTextJsonInputFormatter(options, _loggerFactory.CreateLogger<SystemTextJsonInputFormatter>());
            var binder = new BodyModelBinder(new IInputFormatter[] { formatter }, _reader, _loggerFactory, _mvcOptions.Value);

            return binder;
        }
    }
}