using System;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    public class ScimFilterVisitor : ScimFilterBaseVisitor<FilterNode>
    {
        public ScimFilterVisitor()
        {
        }

        public override FilterNode VisitOperatorExp([NotNull] ScimFilterParser.OperatorExpContext context)
        {
            FilterOperator op = FilterOperator.Equal;

            switch (context.COMPAREOPERATOR().GetText().ToLower())
            {
                case "eq":
                    op = FilterOperator.Equal;
                    break;
                case "ne":
                    op = FilterOperator.NotEqual;
                    break;
                case "lt":
                    op = FilterOperator.LessThan;
                    break;
                case "le":
                    op = FilterOperator.LessThanOrEqual;
                    break;
                case "gt":
                    op = FilterOperator.GreaterThan;
                    break;
                case "ge":
                    op = FilterOperator.GreaterThanOrEqual;
                    break;
                case "co":
                    op = FilterOperator.Contains;
                    break;
                default:
                    throw new NotSupportedException($"Operation {context.COMPAREOPERATOR().Symbol.Text} not supported!");
            }

            return new OperationFilterNode(
                string.Join(".", context.attrPath().ATTRNAME().Select(p => p.GetText())),
                op,
                context.VALUE().GetText(),
                context.attrPath().SCHEMA()?.GetText());
        }
    }
}