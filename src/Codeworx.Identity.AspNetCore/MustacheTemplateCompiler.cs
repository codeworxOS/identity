using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using HandlebarsDotNet;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore
{
    public class MustacheTemplateCompiler : ITemplateCompiler, IDisposable
    {
        private readonly IDisposable _subscription;
        private readonly IHandlebars _handlebars;
        private bool _disposedValue = false;
        private IdentityOptions _options;

        public MustacheTemplateCompiler(IOptionsMonitor<IdentityOptions> optionsMonitor)
        {
            _options = optionsMonitor.CurrentValue;
            _subscription = optionsMonitor.OnChange(p => _options = p);
            _handlebars = Handlebars.Create();
            ////_handlebars.RegisterTemplate("Styles", (w, d) => GetStyles(w, d));
            _handlebars.RegisterTemplate("Styles", GetStyles(_options.Styles));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public Task<string> RenderAsync(string template, object data)
        {
            var compiledTemplate = _handlebars.Compile(template);

            var result = compiledTemplate(data);

            return Task.FromResult(result);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _subscription.Dispose();
                }

                _disposedValue = true;
            }
        }

        private static string GetStyles(IEnumerable<string> styles)
        {
            return string.Join("\r\n", styles.Select(p => $"<link type=\"text/css\" rel=\"stylesheet\" href=\"{p}\" >"));
        }
    }

    ////private void GetStyles(TextWriter writer, object data)
    ////{
    ////    foreach (var item in _options.Styles)
    ////    {
    ////        writer.WriteLine($"<link type=\"text/css\" rel=\"stylesheet\" href=\"{item}\" >");
    ////    }
    ////}
}
