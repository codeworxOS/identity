using System.Collections.Generic;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Account
{
    public class RedirectViewResponse : IViewData
    {
        public RedirectViewResponse(string error = null, string errorDescription = null)
        {
            Error = error;
            ErrorDescription = errorDescription;
        }

        public string Error { get; }

        public string ErrorDescription { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(Error), Error);
            target.Add(nameof(ErrorDescription), ErrorDescription);
        }
    }
}
