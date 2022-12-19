using System.Collections.Generic;
using Codeworx.Identity.Notification;
using Codeworx.Identity.View;

namespace Codeworx.Identity.Model
{
    public class MfaMailNotification : INotification, IViewData
    {
        public MfaMailNotification(string code, IUser user, string company, string supportEmail)
        {
            Code = code;
            Target = user;
            Company = company;
            SupportEmail = supportEmail;
        }

        public string Company { get; }

        public string SupportEmail { get; }

        public string Code { get; }

        public IUser Target { get; }

        public string TemplateKey => Constants.Templates.MfaMailNotification;

        public string Subject => $"{Company} - MFA code";

        public void CopyTo(IDictionary<string, object> target)
        {
            target.Add(nameof(Code), Code);
            target.Add(nameof(Target), Target);
            target.Add(nameof(SupportEmail), SupportEmail);
            target.Add(nameof(Company), Company);
        }
    }
}
