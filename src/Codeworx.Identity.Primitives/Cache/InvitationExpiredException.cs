using System;
using System.Runtime.Serialization;
using Codeworx.Identity.Invitation;

namespace Codeworx.Identity.Cache
{
    public class InvitationExpiredException : Exception
    {
        public InvitationExpiredException(InvitationItem invitation)
            : this("Invitation expired!")
        {
            Invitation = invitation;
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

        public InvitationItem Invitation { get; }
    }
}
