using System;
using System.Security.Cryptography;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using static QRCoder.PayloadGenerator;

namespace Codeworx.Identity.Mfa.Totp
{
    internal class RegisterTotpInfo : ILoginRegistrationInfo
    {
        public RegisterTotpInfo(IUser user, IdentityOptions options, string providerId, string error = null)
        {
            var rng = RandomNumberGenerator.Create();
            byte[] key = new byte[10];
            rng.GetBytes(key, 0, key.Length);

            ProviderId = providerId;
            Error = error;
            SharedSecret = Base32Encoding.ToString(key);

            var coder = new QRCoder.QRCodeGenerator();
            var payload = new OneTimePassword
            {
                AuthAlgorithm = OneTimePassword.OneTimePasswordAuthAlgorithm.SHA1,
                Digits = 6,
                Issuer = options.CompanyName,
                Secret = SharedSecret.ToString(),
                Type = OneTimePassword.OneTimePasswordAuthType.TOTP,
                Label = user.Name,
            };

            var result = coder.CreateQrCode(payload);

            var data = new QRCoder.PngByteQRCode(result);
            var graphic = data.GetGraphic(4);

            this.QrCode = $"data:image/png;base64,{Convert.ToBase64String(graphic)}";
            this.RegistrationUri = payload.ToString();
        }

        public string Template => TotpConstants.Templates.RegisterTotp;

        public string Error { get; }

        public string SharedSecret { get; }

        public string ProviderId { get; }

        public string QrCode { get; }

        public string RegistrationUri { get; }

        public bool HasRedirectUri(out string redirectUri)
        {
            redirectUri = null;
            return false;
        }
    }
}