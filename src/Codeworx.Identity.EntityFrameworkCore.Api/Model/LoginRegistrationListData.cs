using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class LoginRegistrationListData
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        [Required]
        public string Processor { get; set; }

        public bool Disabled { get; set; }
    }
}
