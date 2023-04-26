using Antlr4.Runtime.Tree;

namespace CPP_Metrics
{
    public class TemplateArgumentVisitor : CPP14ParserBaseVisitor<bool>
    {


        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
