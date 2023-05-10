using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics
{
    public class TemplateArgumentVisitor : CPP14ParserBaseVisitor<bool>
    {
        public List<CPPType> Types { get; set; } = new List<CPPType>();

        public override bool VisitTemplateArgumentList([NotNull] CPP14Parser.TemplateArgumentListContext context)
        {
            var templateArguments = context.templateArgument();
            if (templateArguments is null || templateArguments.Length == 0)
                return false;
            // Only types
            var argumentContainsType = templateArguments.Where(x => x.theTypeId() is not null);
            foreach (var argument in argumentContainsType)
            {
                var typeVisitor = new TypeVisitor();
                Analyzer.Analyze(argument, typeVisitor);
                if(typeVisitor.Type != null)
                    Types.Add(typeVisitor.Type);
            }


            return false;
        }

        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
