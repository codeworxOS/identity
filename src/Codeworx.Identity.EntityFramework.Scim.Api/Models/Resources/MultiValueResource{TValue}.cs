namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public class MultiValueResource<TValue> : MultiValueResource
    {
        public TValue Value { get; set; } = default!;
    }
}