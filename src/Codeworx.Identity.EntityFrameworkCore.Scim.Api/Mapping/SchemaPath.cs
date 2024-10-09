namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class SchemaPath
    {
        public SchemaPath(string name, bool isMulti)
        {
            Name = name;
            IsMulti = isMulti;
        }

        public string Name { get; }

        public bool IsMulti { get; }
    }
}