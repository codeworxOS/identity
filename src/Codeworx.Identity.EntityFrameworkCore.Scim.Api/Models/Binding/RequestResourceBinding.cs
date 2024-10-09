using System;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Binding
{
    public class RequestResourceBinding : IModelBinder
    {
        private readonly IHttpRequestStreamReaderFactory _reader;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<MvcOptions> _mvcOptions;
        private readonly ISerializationSetup _serializationSetup;

        public RequestResourceBinding(IHttpRequestStreamReaderFactory reader, ILoggerFactory loggerFactory, IOptions<MvcOptions> mvcOptions, ISerializationSetup serializationSetup)
        {
            _reader = reader;
            _loggerFactory = loggerFactory;
            _mvcOptions = mvcOptions;
            _serializationSetup = serializationSetup;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var binder = CreateModelBinder(bindingContext.ModelType);
            await binder.BindModelAsync(bindingContext);
        }

        private BodyModelBinder CreateModelBinder(Type type)
        {
            var options = _serializationSetup.GetJsonFormatterOptions();

            var formatter = new SystemTextJsonInputFormatter(options, _loggerFactory.CreateLogger<SystemTextJsonInputFormatter>());
            var binder = new BodyModelBinder(new IInputFormatter[] { formatter }, _reader, _loggerFactory, _mvcOptions.Value);

            return binder;
        }
    }
}