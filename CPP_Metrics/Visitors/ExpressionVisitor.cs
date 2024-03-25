using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Reflection;

namespace CPP_Metrics.Visitors
{
    public class ExpressionVisitor : CPP14ParserBaseVisitor<bool>
    {
        public List<Variable> VariableNames = new();

        public List<string> CallFuncNames = new();

        public override bool VisitUnqualifiedId([NotNull] CPP14Parser.UnqualifiedIdContext context)
        {
            var name = context.GetTerminalNodes().FirstOrDefault()?.GetText();
            if (name is null) return true;
            if (IsFucntionPostfix(context))
            {
                CallFuncNames.Add(name);
                return true;
            }
            VariableNames.Add(new Variable() { Name = name });
            return true;
        }

        /// <summary>
        /// Проверка что идет после оператора доступа <.> метод или поле класса
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsFucntionPostfix(IParseTree context)// Доходим до первого Postfix смотрим у него родителя postfix(если он таковым является и есть ли у него дочерние узлы с "()"
        {
            var parent = context.Parent;
            while (parent != null)
            {
                if (parent is CPP14Parser.PostfixExpressionContext)
                {
                    if (parent?.Parent is CPP14Parser.PostfixExpressionContext parentParentContext)
                    {
                        var braces = parentParentContext.children.Where(x => x.GetText() == "(" || x.GetText() == ")");
                        if (braces.Count() == 2)
                        {
                            return true;
                        }
                        break;
                    }
                }
                parent = parent.Parent;
            }

            return false;
        }

        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
