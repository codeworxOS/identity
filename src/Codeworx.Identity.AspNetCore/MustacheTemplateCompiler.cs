using System;
using System.Collections.Generic;
using System.Linq;
using Codeworx.Identity.Configuration;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore
{
    public class MustacheTemplateCompiler : ITemplateCompiler, IDisposable
    {
        private readonly IDisposable _subscription;
        private readonly IHandlebars _handlebars;
        private bool _disposedValue = false;
        private IdentityOptions _options;

        public MustacheTemplateCompiler(IOptionsMonitor<IdentityOptions> optionsMonitor, IEnumerable<IPartialTemplate> partialTemplates, IEnumerable<ITemplateHelper> helpers)
        {
            _options = optionsMonitor.CurrentValue;
            _subscription = optionsMonitor.OnChange(p => _options = p);
            _handlebars = Handlebars.Create();
            _handlebars.RegisterTemplate("Favicon", GetFavicon(_options.Favicon));
            _handlebars.RegisterTemplate("Styles", GetStyles(_options.Styles));
            _handlebars.RegisterTemplate("Scripts", GetScripts(_options.Scripts));

            foreach (var helper in helpers)
            {
                _handlebars.RegisterHelper(new HelperDescriptor(helper));
            }

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

            return p => compiledTemplate(p);
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
            if (styles == null)
            {
                return string.Empty;
            }

            return string.Join("\r\n", styles.Select(p => $"<link type=\"text/css\" rel=\"stylesheet\" href=\"{p}\" >"));
        }

        private static string GetScripts(IEnumerable<string> scripts)
        {
            if (scripts == null)
            {
                return string.Empty;
            }

            return string.Join("\r\n", scripts.Select(p => $"<script type=\"text/javascript\" src=\"{p}\" ></script>"));
        }

        private static string GetFavicon(string favicon)
        {
            return $"<link rel=\"icon\" href=\"{favicon}\">";
        }

        private class HelperDescriptor : IHelperDescriptor<HelperOptions>
        {
            private ITemplateHelper _helper;

            public HelperDescriptor(ITemplateHelper helper)
            {
                _helper = helper;
            }

            public PathInfo Name => _helper.Name;

            public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
            {
                return this.ReturnInvoke(options, context, arguments);
            }

            public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
            {
                var args = arguments.ToArray();

                _helper.Process(output.CreateWrapper(), context, args);
            }
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
