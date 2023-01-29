
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace CPP_Metrics.CyclomaticComplexity
{
    public class ConditionCyclomaticVisitor : CPP14ParserBaseVisitor<bool>
    {
        public int CountLogicalExpression { get; private set; } = 0;
        public override bool VisitLogicalAndExpression([NotNull] CPP14Parser.LogicalAndExpressionContext context)
        {
            var logicalTeminal = context.GetTerminalNodes();
            CountLogicalExpression += logicalTeminal.Count;
            return true;
        }
        public override bool VisitLogicalOrExpression([NotNull] CPP14Parser.LogicalOrExpressionContext context)
        {
            var logicalTeminal = context.GetTerminalNodes();
            CountLogicalExpression += logicalTeminal.Count;
            return true;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
