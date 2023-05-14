

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics
{

    public class UsedClasses : CPP14ParserBaseVisitor<bool>
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
                CPPTypes.Add(new CPPType() {TypeName = name, NestedNames = nestedVisitor.NestedNames });
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
            else if(context.simpleTemplateId() is not null)
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
            if(className != null)
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
            Analyzer.Analyze(nested, nestedVisitor);

            var indetifier = context.Identifier();
            var simplatemplate = context.simpleTemplateId();
            var type = new CPPType();
            
            if(indetifier is not null)
            {
                type.TypeName = indetifier.GetText();
            }
            else if(simplatemplate is not null)
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

    public class UsedVariables : CPP14ParserBaseVisitor<bool>
    {
        public UsedVariables(BaseContextElement baseContextElement)
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
                if(unqualifiedId is not null)
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
                    if(name is not null)
                        Identifiers.Add("this." + name);

                }
                
                return true;
            }
            var idExpression = context.primaryExpression()?.idExpression();
            var Identifier = idExpression?.unqualifiedId()?.Identifier();
            if(Identifier is not null)
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

            //var qualified = idExpression?.qualifiedId();
            //if (qualified is not null && funcCallBrace)
            //{
            //    qualifiedIdCallFunc(qualified);
            //}

                
                
            
            return true;
        }

        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }



        public class PostfixElement : CPP14ParserBaseVisitor<bool>
    {
        public string Name { get; set; }
        // Nested
        public IList<CPPType> _NestedNames { get; set; } = new List<CPPType>();
        public List<CPPType> TemplateTypes { get; set; } = new List<CPPType>();

        public override bool VisitUnqualifiedId([NotNull] CPP14Parser.UnqualifiedIdContext context)
        {
            var identifier = context.Identifier();
            Name = identifier.GetText();
            return true;
        }
        public override bool VisitSimpleTemplateId([NotNull] CPP14Parser.SimpleTemplateIdContext context)
        {
            Name = context.templateName().GetText();
            
            var arguments = context.templateArgumentList();
            var templateVisitor = new TemplateArgumentVisitor();
            Analyzer.Analyze(arguments, templateVisitor);
            TemplateTypes = templateVisitor.Types;

            return true;
        }

        public override bool VisitNestedNameSpecifier([NotNull] CPP14Parser.NestedNameSpecifierContext context)
        {
            var visitor = new NestedNameSpecifierVisitor();
            Analyzer.Analyze(context, visitor);
            _NestedNames = visitor.NestedNames;
            return false;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
