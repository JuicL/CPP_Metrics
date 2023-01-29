

using Antlr4.Runtime.Tree;
using System.Diagnostics.CodeAnalysis;

namespace CPP_Metrics
{
    public  class NameVisitor : CPP14ParserBaseVisitor<bool>
    {
        public string? Name { get; private set; }
        public override bool VisitUnqualifiedId([NotNull] CPP14Parser.UnqualifiedIdContext context)
        {
            if (context.children.Count == 1)
            {
                var name = context.children.First().GetText();
                Name = name;
            }
            return false;
        }

        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
