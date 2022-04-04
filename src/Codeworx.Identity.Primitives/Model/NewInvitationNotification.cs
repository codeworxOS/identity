using System.Collections.Generic;
using Codeworx.Identity.Notification;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class NewInvitationNotification : INotification, IViewData
    {
        public NewInvitationNotification(string invitationUrl, string code, IUser user, string company, string supportEmail)
        {
            InvitationUrl = invitationUrl;
            Code = code;
            Target = user;
            Company = company;
            SupportEmail = supportEmail;
        }

        public string Company { get; }

        public string SupportEmail { get; }

        public string InvitationUrl { get; }

        public string Code { get; }

        public IUser Target { get; }

        public string TemplateKey => Constants.Templates.NewInvitationNotification;

        public string Subject => $"{Company} - New Invitation";

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(Code), Code);
            target.Add(nameof(InvitationUrl), InvitationUrl);
            target.Add(nameof(Target), Target);
            target.Add(nameof(SupportEmail), SupportEmail);
            target.Add(nameof(Company), Company);
        }
    }
}
