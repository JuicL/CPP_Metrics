
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace CPP_Metrics
{
    // For ClassSpecifierContext
    public class ClassDefinitionVisitor : CPP14ParserBaseVisitor<bool>
    {
        public string ClassKey;
        public override bool VisitClassHead([NotNull] CPP14Parser.ClassHeadContext context)
        {
            var classKey = context.children.FirstOrDefault(x => x is CPP14Parser.ClassKeyContext);
            if(classKey != null)
            {
                ClassKey = classKey.GetText();
            }

            return base.VisitClassHead(context);
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
