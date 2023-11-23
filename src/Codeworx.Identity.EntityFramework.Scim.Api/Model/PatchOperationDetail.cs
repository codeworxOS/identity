namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model
{
    public class PatchOperationDetail
    {
        public string Op { get; set; }

        public string Path { get; set; }

        public object Value { get; set; }
    }
}
