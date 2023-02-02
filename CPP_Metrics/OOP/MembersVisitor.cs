
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace CPP_Metrics.OOP
{
    enum AccesSpecifier
    {                
        Public,
        Private,
        Protected,
    }

    public class MembersVisitor : CPP14ParserBaseVisitor<bool>
    {
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
