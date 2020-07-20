using System;
using System.Collections.Generic;
using System.Linq;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
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

        public MustacheTemplateCompiler(IOptionsMonitor<IdentityOptions> optionsMonitor, IEnumerable<IPartialTemplate> partialTemplates)
        {
            _options = optionsMonitor.CurrentValue;
            _subscription = optionsMonitor.OnChange(p => _options = p);
            _handlebars = Handlebars.Create();
            _handlebars.RegisterTemplate("Styles", GetStyles(_options.Styles));
            _handlebars.RegisterHelper("RegistrationTemplate", (writer, context, parameters) =>
            {
                if (context is ILoginRegistrationGroup info)
                {
                    writer.WriteSafeString(info.Template);
                }
            });

            foreach (var partial in partialTemplates)
            {
                _handlebars.RegisterTemplate(partial.Name, partial.Template);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public Func<object, string> Compile(string template)
        {
            var compiledTemplate = _handlebars.Compile(template);

            return compiledTemplate;
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
