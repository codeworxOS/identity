using System;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class ValidRedirectUrlUpdateData
    {
        public Guid Id { get; set; }

        public Guid ClientId { get; set; }

        public string Url { get; set; }
    }
}
