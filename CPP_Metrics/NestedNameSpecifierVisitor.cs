using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics
{
    public class NestedNameSpecifierVisitor : CPP14ParserBaseVisitor<bool>
    {
        public List<CPPType> NestedNames 
        { 
            get { return _NestedNames.Reverse().ToList(); } 
            private set { } 
        }

        private IList<CPPType> _NestedNames = new List<CPPType>();

        public override bool VisitNestedNameSpecifier([NotNull] CPP14Parser.NestedNameSpecifierContext context)
        {
            if (context.children.First() is CPP14Parser.NestedNameSpecifierContext)
            {
                /*nestedNameSpecifier (
		            Identifier | Template? simpleTemplateId
	                ) Doublecolon;*/
                var identifier = context.Identifier();
                if(identifier is not null)
                {
                    var name = identifier.GetText();
                    _NestedNames.Add(new CPPType() { TypeName = name});
                    return true;
                }
                var simpleTemplate = context.simpleTemplateId();
                if(simpleTemplate is not null)
                {
                    var name = simpleTemplate.templateName().GetText();
                    var templateArguments = simpleTemplate.templateArgumentList();

                    var templateType = new CPPType();
                    templateType.TypeName = name;
                    templateType.TemplateNames = new List<CPPType>();
                    _NestedNames.Add(templateType);
                    return true;
                }

            }
            else
            {
                if (context.children.Count == 1) return true; // just doublecolon sign
                var firstElem = context.children.First();
                if(firstElem is CPP14Parser.TheTypeNameContext typeName)
                {
                    var name = typeName.GetTheTypeName();
                    _NestedNames.Add(name);
                }
                else if(firstElem is CPP14Parser.NamespaceNameContext nameSpaceName)
                {
                    //originalNamespaceName | namespaceAlias//
                    var name = nameSpaceName.children.First().GetChildren().First().GetText();
                    _NestedNames.Add(new CPPType() {TypeName = name });

                }
                else if(firstElem is CPP14Parser.DecltypeSpecifierContext)
                {// 
                }

               

            }
            return true;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
