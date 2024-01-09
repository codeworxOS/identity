namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public abstract class MultiValueResource<TValue> : MultiValueResource
    {
        public virtual TValue Value { get; set; } = default!;
    }
}