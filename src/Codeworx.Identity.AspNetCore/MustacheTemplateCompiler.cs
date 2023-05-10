using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
        private static readonly string _defaultVersion;

        private readonly IHandlebars _handlebars;
        private readonly IDisposable _subscription;
        private bool _disposedValue = false;
        private IdentityOptions _options;

        static MustacheTemplateCompiler()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(typeof(IdentityService).Assembly.Location).FileVersion;

            _defaultVersion = versionInfo;
        }

        public MustacheTemplateCompiler(IOptionsMonitor<IdentityOptions> optionsMonitor, IEnumerable<IPartialTemplate> partialTemplates, IEnumerable<ITemplateHelper> helpers)
        {
            _options = optionsMonitor.CurrentValue;
            _subscription = optionsMonitor.OnChange(p => _options = p);
            _handlebars = Handlebars.Create();
            _handlebars.RegisterTemplate("Favicon", GetFavicon(_options.Favicon));
            _handlebars.RegisterTemplate("Styles", GetStyles(_options.Preloads, _options.Styles));
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

        public Func<object, string> Compile(string template)
        {
            var compiledTemplate = _handlebars.Compile(template);

            return p => compiledTemplate(p);
        }

        public void Dispose()
        {
            Dispose(true);
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

        private static string ComposeUrl(string url, string version)
        {
            if (version != null)
            {
                version = version.Replace("{version}", _defaultVersion);

                var appendChar = url.Contains("?") ? "&" : "?";

                return $"{url}{appendChar}v={version}";
            }

            return url;
        }

        private static string GetFavicon(string favicon)
        {
            return $"<link rel=\"icon\" href=\"{favicon}\">";
        }

        private static string GetScripts(IEnumerable<string> scripts)
        {
            if (scripts == null)
            {
                return string.Empty;
            }

            return string.Join("\r\n", scripts.Select(p => $"<script type=\"text/javascript\" src=\"{p}\" ></script>"));
        }

        private static string GetStyles(IDictionary<string, PreloadOption> preloads, IEnumerable<string> styles)
        {
            string result = string.Empty;

            if (preloads != null && preloads.Where(p => p.Value.Enable).Any())
            {
                var preloadItems = from p in preloads
                                   from f in p.Value.Files
                                   where p.Value.Enable
                                   select $"<link rel=\"preload\" href=\"{ComposeUrl(f.Key, p.Value.Version)}\" as=\"{p.Value.Type.ToString().ToLower()}\" type=\"{f.Value}\" crossorigin=\"anonymous\" fetchpriority=\"high\">";

                result = string.Join("\r\n", preloadItems) + "\r\n";
            }

            if (styles != null && styles.Any())
            {
                result += string.Join("\r\n", styles.Select(p => $"<link type=\"text/css\" rel=\"stylesheet\" href=\"{p}\" >"));
            }

            return result;
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
}
