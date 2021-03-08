using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Cache
{
    public class InvitationNotFoundException : Exception
    {
        public InvitationNotFoundException()
            : this("Invitation not found!")
        {
        }

        protected InvitationNotFoundException(string message)
            : base(message)
        {
        }

        protected InvitationNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvitationNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
