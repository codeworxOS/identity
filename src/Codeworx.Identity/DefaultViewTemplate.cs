using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public class DefaultViewTemplate : IViewTemplate
    {
        public Task<string> GetConsentTemplate()
        {
            return Task.FromResult(string.Empty);
        }

        public async Task<string> GetLoggedInTemplate(string returnUrl)
        {
            using (var stream = typeof(DefaultViewTemplate).GetTypeInfo().Assembly
                .GetManifestResourceStream("Codeworx.Identity.assets.loggedin.html"))
            {
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);

                return Encoding.UTF8.GetString(buffer).Replace("{{returnUrl}}", returnUrl);
            }
        }

        public async Task<string> GetLoginTemplate(string returnUrl)
        {
            using (var stream = typeof(DefaultViewTemplate).GetTypeInfo().Assembly
                .GetManifestResourceStream("Codeworx.Identity.assets.login.html"))
            {
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);

                return Encoding.UTF8.GetString(buffer).Replace("{{returnUrl}}", returnUrl);
            }
        }
    }
}