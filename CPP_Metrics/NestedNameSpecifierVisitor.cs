using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;

namespace CPP_Metrics
{
    public class NestedNameSpecifierVisitor : CPP14ParserBaseVisitor<bool>
    {
        public IList<INestedName> NestedNames 
        { 
            get { return _NestedNames.Reverse().ToList(); } 
            private set { } 
        }

        private IList<INestedName> _NestedNames = new List<INestedName>();
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
                    _NestedNames.Add(new SimpleNestedName(name));
                    return true;
                }
                var simpleTemplate = context.simpleTemplateId();
                if(simpleTemplate is not null)
                {
                    var name = simpleTemplate.templateName().GetText();
                    _NestedNames.Add(new TemplateNestedName(name));
                    var templateArguments = simpleTemplate.templateArgumentList();
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
                    _NestedNames.Add(new SimpleNestedName(name));

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
