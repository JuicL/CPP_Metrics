

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace CPP_Metrics
{
    public class TypeVisitor : CPP14ParserBaseVisitor<bool>
    {
        public string Type { get; private set; } = "";
        public bool ClassMarker { get; private set; } = false;

        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
        
        public override bool VisitSimpleTemplateId([NotNull] CPP14Parser.SimpleTemplateIdContext context)
        {
            var templateName = context.children.First(); // typeName actually 
            Type += templateName;

            return false; // TODO: visitor template argumentList mb based on Type visitor 
        }

        public override bool VisitElaboratedTypeSpecifier([NotNull] CPP14Parser.ElaboratedTypeSpecifierContext context)
        {
            var terminalNodes = context.GetTerminalNodes();
            if (terminalNodes.Count > 0)
            {
                Type += terminalNodes.First();
            }

            return true;
        }
        public override bool VisitClassKey([NotNull] CPP14Parser.ClassKeyContext context)
        {
            ClassMarker = true;
            return base.VisitClassKey(context);
        }
        public override bool VisitClassName([NotNull] CPP14Parser.ClassNameContext context)
        {
            var terminalNodes = context.GetTerminalNodes();

            if (terminalNodes.Count != 0)
            {
                Type += terminalNodes.First();
            }

            return base.VisitClassName(context);
        }
        public override bool VisitTheTypeName([NotNull] CPP14Parser.TheTypeNameContext context)
        {
            context.children.First().Accept(this);
            return false;
        }
        public override bool VisitSimpleTypeSpecifier([NotNull] CPP14Parser.SimpleTypeSpecifierContext context)
        {
            var terminalNodes = context.GetTerminalNodes();
            if (terminalNodes.Count > 0)
            {
                Type += terminalNodes.First();
            }
            return base.VisitSimpleTypeSpecifier(context);
        }

    }
}
