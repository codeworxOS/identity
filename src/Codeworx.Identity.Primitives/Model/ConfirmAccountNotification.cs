using System.Collections.Generic;
using Codeworx.Identity.Notification;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class ConfirmAccountNotification : INotification, IViewData
    {
        public ConfirmAccountNotification(string confirmationUrl, string code, IUser user, string company, string supportEmail)
        {
            ConfirmationUrl = confirmationUrl;
            Code = code;
            Target = user;
            Company = company;
            SupportEmail = supportEmail;
        }

        public string Company { get; }

        public string SupportEmail { get; }

        public string ConfirmationUrl { get; }

        public string Code { get; }

        public IUser Target { get; }

        public string TemplateKey => Constants.Templates.ConfirmAccountNotification;

        public string Subject => $"{Company} - Confirm your account";

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(Code), Code);
            target.Add(nameof(ConfirmationUrl), ConfirmationUrl);
            target.Add(nameof(Target), Target);
            target.Add(nameof(SupportEmail), SupportEmail);
            target.Add(nameof(Company), Company);
        }
    }
}
