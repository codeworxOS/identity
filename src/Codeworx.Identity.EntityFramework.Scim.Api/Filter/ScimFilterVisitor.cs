using System;
using System.Linq;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    public class ScimFilterVisitor : ScimFilterBaseVisitor<FilterNode>
    {
        public ScimFilterVisitor()
        {
        }

        public override FilterNode VisitAttrPath([NotNull] ScimFilterParser.AttrPathContext context)
        {
            var schema = context.SCHEMA()?.GetText();
            var paths = context.ATTRNAME().Select(p => p.GetText()).ToArray();

            return new PathFilterNode(paths, schema?.TrimEnd(':'));
        }

        public override FilterNode VisitAndExp([NotNull] ScimFilterParser.AndExpContext context)
        {
            var left = context.filter().First().Accept(this);
            var right = context.filter().Last().Accept(this);

            return new LogicFilterNode((BooleanFilterNode)left, (BooleanFilterNode)right, LogicOperator.Add);
        }

        public override FilterNode VisitOrExp([NotNull] ScimFilterParser.OrExpContext context)
        {
            var left = context.filter().First().Accept(this);
            var right = context.filter().Last().Accept(this);

            return new LogicFilterNode((BooleanFilterNode)left, (BooleanFilterNode)right, LogicOperator.Or);
        }

        public override FilterNode VisitValPathAndExp([NotNull] ScimFilterParser.ValPathAndExpContext context)
        {
            var left = context.valPathFilter().First().Accept(this);
            var right = context.valPathFilter().Last().Accept(this);

            return new LogicFilterNode((BooleanFilterNode)left, (BooleanFilterNode)right, LogicOperator.Add);
        }

        public override FilterNode VisitValPathOrExp([NotNull] ScimFilterParser.ValPathOrExpContext context)
        {
            var left = context.valPathFilter().First().Accept(this);
            var right = context.valPathFilter().Last().Accept(this);

            return new LogicFilterNode((BooleanFilterNode)left, (BooleanFilterNode)right, LogicOperator.Or);
        }

        public override FilterNode VisitValPathExp([NotNull] ScimFilterParser.ValPathExpContext context)
        {
            var path = (PathFilterNode)context.attrPath().Accept(this);
            ////var schema = context.attrPath().SCHEMA()?.GetText();
            ////var paths = context.attrPath().ATTRNAME().Select(p => p.GetText()).ToArray();
            var filter = (BooleanFilterNode)context.valPathFilter().Accept(this);
            var member = context.ATTRNAME()?.GetText();

            return new ArrayFilterNode(path, filter, member);
        }

        public override FilterNode VisitValPathOperatorExp([NotNull] ScimFilterParser.ValPathOperatorExpContext context)
        {
            var compare = context.COMPAREOPERATOR();
            FilterOperator op = GetOperator(compare);

            return new OperationFilterNode(
                (PathFilterNode)context.attrPath().Accept(this),
                op,
                context.VALUE().GetText().Trim('"'));
        }

        public override FilterNode VisitOperatorExp([NotNull] ScimFilterParser.OperatorExpContext context)
        {
            var compare = context.COMPAREOPERATOR();
            FilterOperator op = GetOperator(compare);

            return new OperationFilterNode(
                (PathFilterNode)context.attrPath().Accept(this),
                op,
                context.VALUE().GetText().Trim('"'));
        }

        private static FilterOperator GetOperator(ITerminalNode compare)
        {
            FilterOperator op = FilterOperator.Equal;

            switch (compare.GetText().ToLower())
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
                    throw new NotSupportedException($"Operation {compare.Symbol.Text} not supported!");
            }

            return op;
        }
    }
}