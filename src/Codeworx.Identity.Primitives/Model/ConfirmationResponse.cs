using System.Collections.Generic;
using System.Security.Claims;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class ConfirmationResponse : IViewData
    {
        public ConfirmationResponse(
            IUser user,
            ClaimsIdentity identity = null,
            string message = null,
            string error = null,
            bool rememberMe = false)
        {
            Identity = identity;
            User = user;
            Message = message;
            Error = error;
            RememberMe = rememberMe;
            HasError = Error != null;
        }

        public ClaimsIdentity Identity { get; }

        public IUser User { get; }

        public string Message { get; }

        public string Error { get; }

        public bool RememberMe { get; }

        public bool HasError { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(User), User);
            target.Add(nameof(Error), Error);
            target.Add(nameof(Message), Message);
            target.Add(nameof(HasError), HasError);
            target.Add(nameof(RememberMe), RememberMe);
            target.Add(nameof(Identity), Identity);
        }
    }
}
