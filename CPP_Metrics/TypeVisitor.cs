

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;

namespace CPP_Metrics
{
    public class TypeVisitor : CPP14ParserBaseVisitor<bool>
    {
        public string? Name { get; set;}
        public IList<IType>? Templates { get; set; }
        public string? ClassMarker { get; set; }



        public override bool VisitSimpleTemplateId([NotNull] CPP14Parser.SimpleTemplateIdContext context)
        {
            var templateName = context.templateName(); // typeName actually
            Name = templateName.GetText();
            // TODO: visitor template argumentList mb based on Type visitor 
            Templates = new List<IType>();
            return false; 
        }

        public override bool VisitElaboratedTypeSpecifier([NotNull] CPP14Parser.ElaboratedTypeSpecifierContext context)
        {
            var classKey = context.classKey();
            if(classKey is not null)
            {
                /*attributeSpecifierSeq? nestedNameSpecifier? Identifier
		        | simpleTemplateId
		        | nestedNameSpecifier Template? simpleTemplateId //!!!!TODO */
                ClassMarker = classKey.children.First().GetText(); // Class or struct
                var identifier = context.Identifier();
                if (identifier is not null)
                    Name = identifier.GetText();

            }
            else if (context.Enum() is not null)
            {
                ClassMarker = context.Enum().GetText(); // Enum
                var identifier = context.Identifier();
                Name = identifier.GetText();
            }
            return true;
        }

        public override bool VisitClassName([NotNull] CPP14Parser.ClassNameContext context)
        {
            var identifier = context.Identifier();
            if (identifier is not null)
                Name = identifier.GetText();
            return true;
        }
        public override bool VisitTheTypeName([NotNull] CPP14Parser.TheTypeNameContext context)
        {
            var thetypeName = context.GetTheTypeName();
            if(thetypeName is TemplateIdentifier templateIdentifier)
            {
                Name = templateIdentifier.Name;
                Templates = new List<IType>();
            }
            else if(thetypeName is Identifier identifier)
            { Name = identifier.Name; 
            }
            return false;
        }
        public override bool VisitSimpleTypeSpecifier([NotNull] CPP14Parser.SimpleTypeSpecifierContext context)
        {
            //| nestedNameSpecifier Template simpleTemplateId : !!!!TODO
            if (context.nestedNameSpecifier() is not null || context.theTypeName() is not null 
                || context.decltypeSpecifier() is not null)
                return true;
            var simpleTypeSignednessModifier = context.simpleTypeSignednessModifier();
            var simpleTypeLengthModifiers = context.simpleTypeLengthModifier().ToList();
            var terminalNodes = context.GetTerminalNodes();

            if(simpleTypeSignednessModifier is not null)
                Name += simpleTypeSignednessModifier.children.First().GetText();
            
            foreach (var simpleTypeLengthModifier in simpleTypeLengthModifiers)
                Name += simpleTypeLengthModifier.children.First().GetText();

            foreach (var terminalNode in terminalNodes)
                Name += terminalNode.GetText();
            return false;
        }
        public override bool VisitClassSpecifier([NotNull] CPP14Parser.ClassSpecifierContext context)
        {
            return false;
        }
        public override bool VisitEnumSpecifier([NotNull] CPP14Parser.EnumSpecifierContext context)
        {
            return false;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }

}
