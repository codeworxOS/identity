using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity
{
    public class DefaultViewTemplate : IViewTemplate, IDisposable
    {
        private readonly IDisposable _optionsMonitor;
        private bool _disposedValue = false;
        private IdentityOptions _options;

        public DefaultViewTemplate(IOptionsMonitor<IdentityOptions> options)
        {
            _optionsMonitor = options.OnChange(p => _options = p);
            _options = options.CurrentValue;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public async Task<string> GetLoggedInTemplate(string returnUrl)
        {
            return (await GetTemplateAsString("Codeworx.Identity.assets.loggedin.html"))
                .Replace("{{returnUrl}}", returnUrl)
                .Replace("{{styles}}", GetStyles());
        }

        public async Task<string> GetLoginTemplate(string returnUrl, string username = null, string error = null)
        {
            return (await GetTemplateAsString("Codeworx.Identity.assets.login.html"))
                .Replace("{{returnUrl}}", returnUrl)
                .Replace("{{username}}", username)
                .Replace("{{error}}", error)
                .Replace("{{styles}}", GetStyles());
        }

        public async Task<string> GetTenantSelectionTemplate()
        {
            return await GetTemplateAsString("Codeworx.Identity.assets.tenant.html");
        }

        public async Task<string> GetFormPostTemplate(string redirectUrl, string code, string state)
        {
            return (await GetTemplateAsString("Codeworx.Identity.assets.form_post.html"))
                .Replace("{{redirectUrl}}", redirectUrl)
                .Replace("{{code}}", code)
                .Replace("{{state}}", state);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _optionsMonitor.Dispose();
                }

                _disposedValue = true;
            }
        }

        private static async Task<string> GetTemplateAsString(string resourceName)
        {
            using (var stream = typeof(DefaultViewTemplate)
                                .GetTypeInfo().Assembly
                                .GetManifestResourceStream(resourceName))
            {
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);

                return Encoding.UTF8.GetString(buffer);
            }
        }

        private string GetStyles()
        {
            return string.Join("\r\n", _options.Styles.Select(p => $"<link type=\"text/css\" rel=\"stylesheet\" href=\"{p}\" >"));
        }
    }
}