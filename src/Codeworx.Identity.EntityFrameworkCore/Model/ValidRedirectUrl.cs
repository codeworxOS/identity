using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ValidRedirectUrl
    {
        public Guid Id { get; set; }

        public string Url { get; set; }
    }
}