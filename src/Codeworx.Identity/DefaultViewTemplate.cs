using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public class DefaultViewTemplate : IViewTemplate
    {
        public async Task<string> GetLoggedInTemplate(string returnUrl)
        {
            return (await GetTemplateAsString("Codeworx.Identity.assets.loggedin.html"))
                .Replace("{{returnUrl}}", returnUrl);
        }

        public async Task<string> GetLoginTemplate(string returnUrl)
        {
            return (await GetTemplateAsString("Codeworx.Identity.assets.login.html"))
                .Replace("{{returnUrl}}", returnUrl);
        }

        public async Task<string> GetTenantSelectionTemplate(string returnUrl, bool showDefault)
        {
            return (await GetTemplateAsString("Codeworx.Identity.assets.tenant.html"))
                .Replace("{{returnUrl}}", returnUrl)
                .Replace("{{showDefault}}", showDefault.ToString());
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
    }
}