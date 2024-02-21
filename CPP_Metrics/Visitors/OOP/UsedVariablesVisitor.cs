

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;
using CPP_Metrics.Visitors;

namespace CPP_Metrics.Visitors.OOP
{
    public class UsedVariablesVisitor : CPP14ParserBaseVisitor<bool>
    {
        public UsedVariablesVisitor(BaseContextElement baseContextElement)
        {
            BaseContextElement = baseContextElement;
        }
        public List<string> Identifiers = new List<string>();

        public BaseContextElement BaseContextElement { get; set; }
        private void qualifiedIdCallFunc([NotNull] CPP14Parser.QualifiedIdContext context)
        {
            var qualifiedIdIdentifier = context.unqualifiedId()?.Identifier();
            if (qualifiedIdIdentifier is not null)
            {
                var nestedVisitor = new NestedNameSpecifierVisitor();
                Analyzer.Analyze(context.nestedNameSpecifier(), nestedVisitor);

                var isType = BaseContextElement.GetTypeName(qualifiedIdIdentifier.GetText(), nestedVisitor.NestedNames);
                if (isType is null)
                {

                }
            }
        }
        public override bool VisitPostfixExpression([NotNull] CPP14Parser.PostfixExpressionContext context)
        {

            // Проверка на вызов функции + this.vari
            var thisPrimary = context.primaryExpression()?.This();
            if (thisPrimary is not null)
            {

                CPP14Parser.UnqualifiedIdContext? unqualifiedId = (CPP14Parser.UnqualifiedIdContext?)(context.Parent.GetChildren().
                    FirstOrDefault(x => x is CPP14Parser.IdExpressionContext)?.GetChildren().FirstOrDefault(x => x is CPP14Parser.UnqualifiedIdContext));
                if (unqualifiedId is not null)
                {
                    var name = unqualifiedId.Identifier()?.GetText();
                    if (context.Parent.Parent is CPP14Parser.PostfixExpressionContext postfixParentParent)
                    {
                        if (postfixParentParent.LeftParen() is not null || postfixParentParent.RightParen() is not null)
                        {
                            //CAllFUNC
                            return true;
                        }
                    }
                    if (name is not null)
                        Identifiers.Add("this." + name);

                }

                return true;
            }
            var idExpression = context.primaryExpression()?.idExpression();
            var Identifier = idExpression?.unqualifiedId()?.Identifier();
            if (Identifier is not null)
            {

                if (context.Parent is CPP14Parser.PostfixExpressionContext postfixParent)
                {
                    if (postfixParent.LeftParen() is not null || postfixParent.LeftParen() is not null)
                    {
                        //CallFUNC
                        return true;
                    }
                }

                var name = Identifier.GetText();

                Identifiers.Add(name);
                return true;
            }
            return true;
        }

        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
