using System;

namespace Codeworx.Identity
{
    public class PasswordChangeException : Exception
    {
        public PasswordChangeException(string message)
            : base(message)
        {
        }
    }
}