
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;

namespace CPP_Metrics
{

    public class FunctionDefinitionVisitor : CPP14ParserBaseVisitor<bool>
    {
        public string? ReturnType { get; private set; }
        public string? FunctionName { get; private set; }
        public string? NestedName { get; private set; }
        public List<Parameter> Parameters { get; private set; } = new List<Parameter>();
       
        public override bool VisitFunctionDefinition([NotNull] CPP14Parser.FunctionDefinitionContext context)
        {
            return true;
        }
        public override bool VisitDeclSpecifierSeq([NotNull] CPP14Parser.DeclSpecifierSeqContext context)
        {
            var typeVisitor = new TypeVisitor();
            Analyzer.Analyze(context, typeVisitor);
            ReturnType = typeVisitor.Type;
            return false;
        }
        
        private void ParseDestructor([NotNull] CPP14Parser.UnqualifiedIdContext context)
        {
            NestedName = ReturnType;
            ReturnType = null;
            // ClassName or decltypeSpecifier
            if (context.children[1] is CPP14Parser.ClassNameContext)
            {
                var identifier = context.children.SingleOrDefault(x => x is CPP14Parser.ClassNameContext)?.
                    GetTerminalNodes().SingleOrDefault();
                if (identifier is not null)
                {
                    FunctionName = "~" + identifier.GetText();
                }
            }
            else if (context.children[1] is CPP14Parser.DecltypeSpecifierContext)
            {
                // TODO: DecltypeSpecifierContext
            }
        }
        private void ParseOperatorFunctionId([NotNull] CPP14Parser.OperatorFunctionIdContext context)
        {
            FunctionName = context.children.First().GetText() // operator
                + context.children.Last().GetTerminalNodes().First().GetText(); // theOperator 
            
        }
        /*
         Identifier+
	    |   operatorFunctionId+
	    | conversionFunctionId
	    | literalOperatorId(?)
	    | Tilde (className+ | decltypeSpecifier)
	    | templateId;
         */
        public override bool VisitUnqualifiedId([NotNull] CPP14Parser.UnqualifiedIdContext context)
        {
            var type = context.children.Last();
            switch (type)
            {
                case TerminalNodeImpl:
                    var terminalNodes = context.GetTerminalNodes();
                    if (terminalNodes.Count == 0) return false;
                    var functionName = terminalNodes.First().GetText();
                    FunctionName = functionName;
                    break;
                case CPP14Parser.ClassNameContext:
                    ParseDestructor(context);
                    break;
                case CPP14Parser.DecltypeSpecifierContext:
                    break;
                case CPP14Parser.OperatorFunctionIdContext:
                    ParseOperatorFunctionId((CPP14Parser.OperatorFunctionIdContext)context.children.First());
                    break;
                default:
                    break;
            }
            
            return false;
        }
        
        public override bool VisitSimpleTemplateId([NotNull] CPP14Parser.SimpleTemplateIdContext context)
        {
            return base.VisitSimpleTemplateId(context);
        }
        // Nestedname class
        public override bool VisitClassName([NotNull] CPP14Parser.ClassNameContext context)
        {
            //var name = context.GetTerminalNodes().FirstOrDefault()?.GetText();
            //if(name is null) return true;
            
            //NestedName = name;
            return false;
        }
        //VisitNested
        public override bool VisitTemplateName([NotNull] CPP14Parser.TemplateNameContext context)
        {
            //NestedName = context.GetTerminalNodes().First().GetText();
            return false;
        }

        public override bool VisitNoPointerDeclarator([NotNull] CPP14Parser.NoPointerDeclaratorContext context)
        {
            var noPointerDecl = context.children.FirstOrDefault(x => x is CPP14Parser.NoPointerDeclaratorContext);
            if(noPointerDecl is null) 
                return true;

            return true;
        }

        public override bool VisitParameterDeclaration([NotNull] CPP14Parser.ParameterDeclarationContext context)
        {
            var visitor = new ParameterVisitor();
            Analyzer.Analyze(context,visitor);
            Parameters.Add(visitor.Parameter);
            return false;
        }

        public override bool VisitFunctionBody([NotNull] CPP14Parser.FunctionBodyContext context)
        {
            /*
            constructorInitializer? compoundStatement
	        | functionTryBlock
	        | Assign (Default | Delete) Semi;
             */
            return false;
        }

        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
