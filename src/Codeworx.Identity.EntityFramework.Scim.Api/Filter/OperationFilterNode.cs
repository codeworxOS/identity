namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public class OperationFilterNode : FilterNode
    {
        public OperationFilterNode(string path, FilterOperator op, string value, string? schema = null)
        {
            Path = path;
            Op = op;
            Value = value;
            Schema = schema;
        }

        public string Path { get; }

        public FilterOperator Op { get; }

        public string Value { get; }

        public string? Schema { get; }
    }
}
