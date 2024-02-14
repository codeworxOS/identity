﻿namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class CommonRequestResource : ICommonRequestResource
    {
        public string[] Schemas { get; set; } = new string[] { };

        public string? ExternalId { get; set; }
    }
}