using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace CPP_Metrics.Visitors
{
    public class PostfixExpressionVisitor : CPP14ParserBaseVisitor<bool>
    {
        private void HandeDotArrowAccess(List<CPP14Parser.PostfixExpressionContext> list)
        {

        }
        void HandlePostfixExpression(CPP14Parser.PostfixExpressionContext context)
        {
            var currentPostfix = context;
            List<CPP14Parser.PostfixExpressionContext> list = new();
            while (currentPostfix is not null)
            {
                list.Add(currentPostfix);
                currentPostfix = currentPostfix.postfixExpression();
            }
            list.Reverse();

            if (list.Any(x => x.Dot() is not null || x.Arrow() is not null))
            {
                HandeDotArrowAccess(list);
            }
        }

        public override bool VisitUnaryExpression([NotNull] CPP14Parser.UnaryExpressionContext context)
        {
            var firstPostfix = context.postfixExpression();
            if (firstPostfix is not null)
            {
                HandlePostfixExpression(firstPostfix);
            }




            return true;
        }
        public override bool VisitPostfixExpression([NotNull] CPP14Parser.PostfixExpressionContext context)
        {
            if (context.Dot() is not null || context.Arrow() is not null)
            {
                var idExp = context.idExpression();

            }
            else if (context.primaryExpression() is not null)
            {
                var primaryExp = context.primaryExpression();
                if (primaryExp.idExpression() is not null)
                {

                }
            }
            return true;
        }



        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
