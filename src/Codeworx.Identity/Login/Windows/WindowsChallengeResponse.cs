using System.Collections.Generic;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Login.Windows
{
    public class WindowsChallengeResponse : IViewData
    {
        public WindowsChallengeResponse(string returnUrl, bool doChallenge = true)
        {
            ReturnUrl = returnUrl;
            DoChallenge = doChallenge;
        }

        public string ReturnUrl { get; }

        public bool DoChallenge { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(ReturnUrl), ReturnUrl);
        }
    }
}
