using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Cache
{
    public class InvitationExpiredException : Exception
    {
        public InvitationExpiredException()
            : this("Invitation expired!")
        {
        }

        protected InvitationExpiredException(string message)
            : base(message)
        {
        }

        protected InvitationExpiredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvitationExpiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
