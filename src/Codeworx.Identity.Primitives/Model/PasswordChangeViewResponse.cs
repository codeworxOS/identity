using System.Collections.Generic;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class PasswordChangeViewResponse : IViewData
    {
        public PasswordChangeViewResponse(string username, string error = null)
        {
            Error = error;
            HasError = Error != null;
            Username = username;
        }

        public string Error { get; }

        public bool HasError { get; }

        public string Username { get; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(Error), Error);
            target.Add(nameof(Username), Username);
            target.Add(nameof(HasError), HasError);
        }
    }
}
