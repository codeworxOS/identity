using System.Collections.Generic;
using Codeworx.Identity.Notification;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class ForgotPasswordNotification : INotification, IViewData
    {
        public ForgotPasswordNotification(string invitationUrl, IUser user, string company, string supportEmail)
        {
            InvitationUrl = invitationUrl;
            Target = user;
            Company = company;
            SupportEmail = supportEmail;
        }

        public string Company { get; }

        public string SupportEmail { get; }

        public string InvitationUrl { get; }

        public IUser Target { get; }

        public string TemplateKey => Constants.Templates.ForgotPasswordNotification;

        public string Subject => $"{Company} - Reset Password";

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(InvitationUrl), InvitationUrl);
            target.Add(nameof(Target), Target);
            target.Add(nameof(SupportEmail), SupportEmail);
            target.Add(nameof(Company), Company);
        }
    }
}
