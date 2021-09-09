using System.Collections.Generic;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class ForgotPasswordResponse : IViewData
    {
        public ForgotPasswordResponse(string loginUrl)
        {
            LoginUrl = loginUrl;
        }

        public string LoginUrl { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(LoginUrl), LoginUrl);
        }
    }
}
