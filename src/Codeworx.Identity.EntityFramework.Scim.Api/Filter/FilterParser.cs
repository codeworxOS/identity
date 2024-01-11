using Antlr4.Runtime;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public class FilterParser : IFilterParser
    {
        public FilterNode Parse(string filter)
        {
            var visitor = new ScimFilterVisitor();

            var inputStream = new AntlrInputStream(filter);
            var lexer = new ScimFilterLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ScimFilterParser(tokenStream);

            var tree = parser.parse();

            return tree.Accept(visitor);
        }
    }
}
