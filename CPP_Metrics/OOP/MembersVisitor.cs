
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

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
    public class MembersVisitor : CPP14ParserBaseVisitor<bool>
    {
        Queue<IParseTree> FunctionDefinition = new Queue<IParseTree>();

        private AccesSpecifier AccesSpecifierSelector; //default value
        
        public MembersVisitor(string type)
        {
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
            }
        }

        public override bool VisitAccessSpecifier([NotNull] CPP14Parser.AccessSpecifierContext context)
        {
            var accessSpecifier = context.children.First().GetText();
            switch (accessSpecifier)
            {
                case "public":
                    AccesSpecifierSelector = AccesSpecifier.Private;
                    break;
                case "private":
                    AccesSpecifierSelector = AccesSpecifier.Public;
                    break;
                case "protected":
                    AccesSpecifierSelector = AccesSpecifier.Public;
                    break;
            }
            return false;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }

    }
}
