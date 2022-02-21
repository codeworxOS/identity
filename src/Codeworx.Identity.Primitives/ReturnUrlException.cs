using System;

namespace Codeworx.Identity
{
    public class ReturnUrlException : Exception, IWithReturnUrl
    {
        public ReturnUrlException(string message, Exception innerException, string returnUrl)
            : base(message, innerException)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; }
    }
}
