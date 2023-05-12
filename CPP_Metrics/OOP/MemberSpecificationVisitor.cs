
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics.OOP
{
    public enum AccesSpecifier
    {                
        Public,
        Private,
        Protected,
    }
    public static class AccesSpecifierHelper
    {
        public static AccesSpecifier GetAccesSpecifier(string specifier)
        {
            AccesSpecifier accesSpecifier;
            switch (specifier)
            {
                case "public":
                    accesSpecifier = AccesSpecifier.Public;
                    break;
                case "private":
                    accesSpecifier = AccesSpecifier.Private;
                    break;
                case "protected":
                    accesSpecifier = AccesSpecifier.Protected;
                    break;
                default:
                    throw new Exception("Recognized error");
            }
            return accesSpecifier;
        }
    }
    public class MemberSpecificationVisitor : CPP14ParserBaseVisitor<bool>
    {
        public Queue<FunctionInfo> FunctionDefinition { get; private set; } = new();

        private AccesSpecifier AccesSpecifierSelector; //default value

        public ClassStructDeclaration ContextElement { get; }


        public MemberSpecificationVisitor(string type, BaseContextElement contextElement)
        {
            ContextElement = (ClassStructDeclaration)contextElement;
            switch (type)
            {
                case "class":
                    AccesSpecifierSelector = AccesSpecifier.Private;
                    break;
                case "struct":
                    AccesSpecifierSelector = AccesSpecifier.Public;
                    break;
                case "union":
                    break;
                default:
                    throw new Exception("Error recognize type");
            }
        }
        
        public override bool VisitAccessSpecifier([NotNull] CPP14Parser.AccessSpecifierContext context)
        {
            var accessSpecifier = context.children.First().GetText();
            AccesSpecifierSelector = AccesSpecifierHelper.GetAccesSpecifier(accessSpecifier);
            return false;
        }
        public override bool VisitMemberdeclaration([NotNull] CPP14Parser.MemberdeclarationContext context)
        {
            var memberDeclarationVisitor = new MemberDeclarationVisitor(ContextElement, AccesSpecifierSelector);
            Analyzer.Analyze(context,memberDeclarationVisitor);

            ContextElement.ClassStructInfo.Fields.AddRange(memberDeclarationVisitor.VariablesDeclaration);
            ContextElement.ClassStructInfo.Methods.AddRange(memberDeclarationVisitor.FunctionDeclaration);
            foreach (var item in memberDeclarationVisitor.FunctionDefinition)
            {
                FunctionDefinition.Enqueue(item);
            }

            return false;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }

    }
}
