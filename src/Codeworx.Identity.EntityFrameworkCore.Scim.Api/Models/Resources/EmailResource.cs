namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class EmailResource : MultiValueResource<string>
    {
        public string? Whatever { get; set; }
    }
}