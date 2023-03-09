using System.Collections.Generic;
using System.Linq;

namespace Codeworx.Identity.Login
{
    public class RedirectRegistrationInfo : ILoginRegistrationInfo
    {
        public RedirectRegistrationInfo(string providerId, string name, string cssClass, string redirectUri, string error = null)
        {
            Name = name;
            RedirectUri = redirectUri;
            ProviderId = providerId;
            CssClass = cssClass;
            Error = error;

            var builder = new UriBuilder(RedirectUri);

            RedirectPath = builder.ToString(true);
            RedirectParams = (from p in builder.Query
                              from v in p.Value
                              select new ParameterValue(p.Key, v)).ToList();
        }

        public virtual string Template => Constants.Templates.Redirect;

        public string Name { get; }

        public string RedirectUri { get; }

        public string ProviderId { get; }

        public string CssClass { get; }

        public string Error { get; }

        public string RedirectPath { get; }

        public List<ParameterValue> RedirectParams { get; }

        public bool HasRedirectUri(out string redirectUri)
        {
            redirectUri = RedirectUri;
            return true;
        }
    }
}
