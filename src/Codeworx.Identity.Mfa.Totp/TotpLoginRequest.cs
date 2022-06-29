namespace Codeworx.Identity.Mfa.Totp
{
    public class TotpLoginRequest
    {
        public string OneTimeCode { get; set; }

        public string SharedSecret { get; set; }
    }
}