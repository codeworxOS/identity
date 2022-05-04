using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class LoginRegistrationInsertData
    {
        public string DisplayName { get; set; }

        [Required]
        public string Processor { get; set; }

        public int? SortOrder { get; set; }

        public Dictionary<string, object> Configuration { get; set; }

        public bool Disabled { get; set; }
    }
}
