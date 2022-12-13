using Codeworx.Identity.Login;

namespace Codeworx.Identity.Mfa.Mail
{
    public class MailRegistrationInfo : ILoginRegistrationInfo
    {
        public MailRegistrationInfo(string providerId, string emailAddress, string error = null)
        {
            ProviderId = providerId;
            EmailAddress = emailAddress;
            Error = error;

            MaskedEmailAddress = Mask(EmailAddress);
        }

        public string EmailAddress { get; }

        public string Error { get; }

        public string MaskedEmailAddress { get; }

        public string ProviderId { get; }

        public string Template => Constants.Templates.LoginMfaMail;

        public static string Mask(string emailAddress)
        {
            var separatorIndex = emailAddress.IndexOf('@');

            if (separatorIndex >= 0)
            {
                var left = emailAddress.Substring(0, separatorIndex);
                var right = emailAddress.Substring(separatorIndex + 1);
                var extension = right.Substring(right.LastIndexOf('.'));
                left = $"{left.Substring(0, 1).ToLower()}*****";

                right = $"{right.Substring(0, 1).ToLower()}*****";

                return $"{left}@{right}{extension}";
            }

            return null;
        }

        public bool HasRedirectUri(out string redirectUri)
        {
            redirectUri = null;
            return false;
        }
    }
}