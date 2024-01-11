namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public interface IFilterParser
    {
        FilterNode Parse(string filter);
    }
}
