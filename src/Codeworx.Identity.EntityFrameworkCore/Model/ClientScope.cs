using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ClientScope : Scope
    {
        public ClientConfiguration Client { get; set; }

        public Guid ClientId { get; set; }
    }
}