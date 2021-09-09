using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.View;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity
{
    public class DefaultViewTemplate :
        ILoginViewTemplate,
        ITenantViewTemplate,
        IRedirectViewTemplate,
        IFormPostResponseTypeTemplate,
        IInvitationViewTemplate,
        IPasswordChangeViewTemplate,
        IProfileViewTemplate,
        IDisposable
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

        public async Task<string> GetChallengeResponse()
        {
            return await GetTemplateAsStringAsync("Codeworx.Identity.assets.challenge_response.html");
        }

        public async Task<string> GetFormPostTemplate()
        {
            return await GetTemplateAsStringAsync("Codeworx.Identity.assets.form_post.html");
        }

        public async Task<string> GetInvitationTemplate()
        {
            return await GetTemplateAsStringAsync("Codeworx.Identity.assets.account.invitation.html");
        }

        public async Task<string> GetLoginTemplate()
        {
            return await GetTemplateAsStringAsync("Codeworx.Identity.assets.account.login.html");
        }

        public async Task<string> GetPasswordChangeTemplate()
        {
            return await GetTemplateAsStringAsync("Codeworx.Identity.assets.account.password_change.html");
        }

        public async Task<string> GetProfileViewTemplate()
        {
            return await GetTemplateAsStringAsync("Codeworx.Identity.assets.account.profile.html");
        }

        public async Task<string> GetRedirectTemplate()
        {
            return await GetTemplateAsStringAsync("Codeworx.Identity.assets.account.redirect.html");
        }

        public async Task<string> GetTenantSelectionTemplate()
        {
            return await GetTemplateAsStringAsync("Codeworx.Identity.assets.account.tenant.html");
        }

        internal static string GetTemplateAsString(string resourceName)
        {
            using (var stream = typeof(DefaultViewTemplate)
                                .GetTypeInfo().Assembly
                                .GetManifestResourceStream(resourceName))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                return Encoding.UTF8.GetString(buffer);
            }
        }

        internal static async Task<string> GetTemplateAsStringAsync(string resourceName)
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

        private string GetStyles()
        {
            return string.Join("\r\n", _options.Styles.Select(p => $"<link type=\"text/css\" rel=\"stylesheet\" href=\"{p}\" >"));
        }
    }
}