using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Cache
{
    public class InvitationCodeAlreadyUsedException : Exception
    {
        public InvitationCodeAlreadyUsedException()
            : this("The invitation code was already used.")
        {
        }

        public InvitationCodeAlreadyUsedException(string message)
            : base(message)
        {
        }

        public InvitationCodeAlreadyUsedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvitationCodeAlreadyUsedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
