

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics
{
    public class TypeVisitor : CPP14ParserBaseVisitor<bool>
    {
        public CPPType? Type 
        { 
            get 
            { 
                if (_Type.TypeName is null) 
                    return null; 
                else return _Type; 
            } 
        }
        private CPPType _Type { get; set;} = new CPPType();
        public override bool VisitFunctionSpecifier([NotNull] CPP14Parser.FunctionSpecifierContext context)
        {
            _Type.FunctionSpecifier = context.children.First().GetText();
            return false;
        }
        public override bool VisitNestedNameSpecifier([NotNull] CPP14Parser.NestedNameSpecifierContext context)
        {
            var nestedVisitor = new NestedNameSpecifierVisitor();
            Analyzer.Analyze(context, nestedVisitor);
            _Type.NestedNames = nestedVisitor.NestedNames;
            return false;
        }

        public override bool VisitSimpleTemplateId([NotNull] CPP14Parser.SimpleTemplateIdContext context)
        {
            var templateName = context.templateName(); // typeName actually
            _Type.TypeName = templateName.GetText();
            // TODO: visitor template argumentList mb based on Type visitor 
            _Type.TemplateNames = new List<CPPType>();
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
                _Type.ClassMarker = classKey.children.First().GetText(); // Class or struct
                var identifier = context.Identifier();
                if (identifier is not null)
                    _Type.TypeName = identifier.GetText();


            }
            else if (context.Enum() is not null)
            {
                _Type.ClassMarker = context.Enum().GetText(); // Enum
                var identifier = context.Identifier();
                _Type.TypeName = identifier.GetText();
            }
            return true;
        }

        public override bool VisitClassName([NotNull] CPP14Parser.ClassNameContext context)
        {
            var identifier = context.Identifier();
            if (identifier is not null)
                _Type.TypeName = identifier.GetText();
            return true;
        }
        public override bool VisitTheTypeName([NotNull] CPP14Parser.TheTypeNameContext context)
        {
            var thetypeName = context.GetTheTypeName();
            _Type.TypeName = thetypeName.TypeName;
            _Type.TemplateNames = thetypeName.TemplateNames;
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
                _Type.TypeName += simpleTypeSignednessModifier.children.First().GetText();
            
            foreach (var simpleTypeLengthModifier in simpleTypeLengthModifiers)
                _Type.TypeName += simpleTypeLengthModifier.children.First().GetText();

            foreach (var terminalNode in terminalNodes)
                _Type.TypeName += terminalNode.GetText();
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
