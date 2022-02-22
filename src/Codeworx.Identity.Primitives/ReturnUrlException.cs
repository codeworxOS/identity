using System;

namespace Codeworx.Identity
{
    public class ReturnUrlException : Exception, IErrorWithReturnUrl
    {
        public ReturnUrlException(string message, Exception innerException, string returnUrl)
            : base(message, innerException)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; }

        public string GetMessage()
        {
            if (InnerException is IEndUserErrorMessage endUserError)
            {
                return endUserError.GetMessage();
            }

            return null;
        }
    }
}
