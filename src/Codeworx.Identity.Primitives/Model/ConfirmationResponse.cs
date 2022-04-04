using System.Collections.Generic;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class ConfirmationResponse : IViewData
    {
        public ConfirmationResponse(IUser user, string message = null, string error = null)
        {
            User = user;
            Message = message;
            Error = error;
            HasError = Error != null;
        }

        public IUser User { get; }

        public string Message { get; }

        public string Error { get; }

        public bool HasError { get; set; }

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(User), User);
            target.Add(nameof(Error), Error);
            target.Add(nameof(Message), Message);
            target.Add(nameof(HasError), HasError);
        }
    }
}
