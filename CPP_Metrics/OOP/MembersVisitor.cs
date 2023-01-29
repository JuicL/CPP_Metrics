
using Antlr4.Runtime.Tree;

namespace CPP_Metrics.OOP
{
    public class MembersVisitor : CPP14ParserBaseVisitor<bool>
    {
        private bool publicPrivateSelector = false; //default is private
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }

    }
}
