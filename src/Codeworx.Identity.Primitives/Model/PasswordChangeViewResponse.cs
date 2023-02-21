using System.Collections.Generic;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class PasswordChangeViewResponse : IViewData
    {
        public PasswordChangeViewResponse(string username, bool hasCurrentPassword, MaxLengthOption maxLength, string error = null)
        {
            Error = error;
            HasError = Error != null;
            Username = username;
            HasCurrentPassword = hasCurrentPassword;
            MaxLength = maxLength;
        }

        public string Error { get; }

        public bool HasError { get; }

        public string Username { get; }

        public bool HasCurrentPassword { get; }

        public MaxLengthOption MaxLength { get; set; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(Error), Error);
            target.Add(nameof(Username), Username);
            target.Add(nameof(HasError), HasError);
            target.Add(nameof(HasCurrentPassword), HasCurrentPassword);
            target.Add(nameof(MaxLength), MaxLength);
        }
    }
}
