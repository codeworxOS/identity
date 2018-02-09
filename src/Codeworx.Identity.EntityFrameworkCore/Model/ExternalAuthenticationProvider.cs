﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ExternalAuthenticationProvider
    {
        public string EndpointConfiguration { get; set; }

        [StringLength(100)]
        [Required]
        public string EndpointType { get; set; }

        public ProviderFilter Filter { get; set; }

        public Guid? FilterId { get; set; }

        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }
    }
}