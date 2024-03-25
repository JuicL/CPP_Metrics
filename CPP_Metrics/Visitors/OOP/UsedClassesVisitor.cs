using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Reflection;

namespace CPP_Metrics.Visitors.OOP
{
    public class UsedClassesVisitor : CPP14ParserBaseVisitor<bool>
    {
        public List<CPPType> CPPTypes { get; set; } = new();
        public override bool VisitIdExpression([NotNull] CPP14Parser.IdExpressionContext context)
        {
            // Вызов статической функции

            var qualifiedId = context.qualifiedId();
            if (qualifiedId == null) return true;

            var nested = qualifiedId.nestedNameSpecifier();

            var nestedVisitor = new NestedNameSpecifierVisitor();
            Analyzer.Analyze(nested, nestedVisitor);

            var unqualifiedId = qualifiedId.unqualifiedId();
            var identifier = unqualifiedId.Identifier();
            var templateId = unqualifiedId.templateId();

            if (identifier != null)
            {
                var name = identifier.GetText();
                CPPTypes.Add(new CPPType() { TypeName = name, NestedNames = nestedVisitor.NestedNames });
            }
            else if (templateId.simpleTemplateId() != null)
            {
                var type = SimpleTemplateId(templateId.simpleTemplateId());
                type.NestedNames = nestedVisitor.NestedNames;
                CPPTypes.Add(type);
            }
            return true;
        }

        public override bool VisitSimpleTypeSpecifier([NotNull] CPP14Parser.SimpleTypeSpecifierContext context)
        {

            List<CPPType>? nestedList = new();

            var nested = context.nestedNameSpecifier();
            if (nested is not null)
            {
                var nestedVisitor = new NestedNameSpecifierVisitor();
                Analyzer.Analyze(nested, nestedVisitor);
                nestedList = nestedVisitor.NestedNames;
            }

            var theTypeName = context.theTypeName();
            if (theTypeName != null)
            {
                var type = SimpleTypeSpecifierTheTypeName(theTypeName);
                type.NestedNames = nestedList;
                CPPTypes.Add(type);
            }
            else if (context.simpleTemplateId() is not null)
            {
                var type = SimpleTemplateId(context.simpleTemplateId());
                type.NestedNames = nestedList;
                CPPTypes.Add(type);
            }

            return true;
        }

        public CPPType SimpleTemplateId(CPP14Parser.SimpleTemplateIdContext simpleTemplateIdContext)
        {

            var templateVisitor = new TemplateArgumentVisitor();
            var templateArgumentList = simpleTemplateIdContext.templateArgumentList();
            if (templateArgumentList is not null)
            {
                Analyzer.Analyze(templateArgumentList, templateVisitor);
            }

            return new CPPType()
            {
                TypeName = simpleTemplateIdContext.templateName().GetText(),
                TemplateNames = templateVisitor.Types
            };
        }

        public CPPType SimpleTypeSpecifierTheTypeName(CPP14Parser.TheTypeNameContext theTypeNameContext)
        {
            CPPType type = new CPPType();
            var className = theTypeNameContext.className();
            var simpleTemplate = theTypeNameContext.simpleTemplateId();
            if (className != null)
            {
                var identifier = className.Identifier();
                if (identifier != null)
                    type.TypeName = identifier.GetText();
                else
                    type = SimpleTemplateId(className.simpleTemplateId());
            }
            else if (simpleTemplate is not null)
            {
                type = SimpleTemplateId(simpleTemplate);
            }
            else
            {
                var identifier = theTypeNameContext.children.First().GetChildren().First();
                type.TypeName = identifier.GetText();
            }


            return type;
        }
        public override bool VisitElaboratedTypeSpecifier([NotNull] CPP14Parser.ElaboratedTypeSpecifierContext context)
        {
            if (context.classKey() is null)
                return false;
            var nested = context.nestedNameSpecifier();
            var nestedVisitor = new NestedNameSpecifierVisitor();
            if (nested is not null)
            {
                Analyzer.Analyze(nested, nestedVisitor);
            }

            var indetifier = context.Identifier();
            var simplatemplate = context.simpleTemplateId();
            var type = new CPPType();

            if (indetifier is not null)
            {
                type.TypeName = indetifier.GetText();
            }
            else if (simplatemplate is not null)
            {
                type = SimpleTemplateId(simplatemplate);
            }

            type.NestedNames = nestedVisitor.NestedNames;

            CPPTypes.Add(type);

            return false;
        }


        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
