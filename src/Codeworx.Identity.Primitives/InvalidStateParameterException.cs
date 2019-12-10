using System;

namespace Codeworx.Identity
{
    public class InvalidStateParameterException : Exception
    {
        public InvalidStateParameterException()
        : base("The current state parameter does not equal the requested.")
        {
        }
    }
}