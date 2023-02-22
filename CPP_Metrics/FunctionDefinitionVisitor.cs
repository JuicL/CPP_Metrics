
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics
{

    public class FunctionDefinitionVisitor : CPP14ParserBaseVisitor<bool>
    {
        public FunctionInfo FunctionInfo { get; } = new FunctionInfo();
       
        public override bool VisitFunctionDefinition([NotNull] CPP14Parser.FunctionDefinitionContext context)
        {
            string text = "";
            foreach (var item in context.children)
            {
                if (item is CPP14Parser.FunctionBodyContext)
                    break;
                text += " " + item.GetText();
            }
            FunctionInfo.Text = text;
            return true;
        }
        public override bool VisitVirtualSpecifier([NotNull] CPP14Parser.VirtualSpecifierContext context)
        {
            var virtualSpecifier = context.children.First().GetText();
            if (virtualSpecifier.Equals("override"))
            {
                FunctionInfo.Override = true;
            }
            else if (virtualSpecifier.Equals("final"))
            {
                FunctionInfo.Final = true;
            }
            return false;
        }
        public override bool VisitDeclSpecifierSeq([NotNull] CPP14Parser.DeclSpecifierSeqContext context)
        {
            var typeVisitor = new TypeVisitor();
            Analyzer.Analyze(context, typeVisitor);
            FunctionInfo.ReturnType = typeVisitor.Type;
            return false;
        }
        public override bool VisitNestedNameSpecifier([NotNull] CPP14Parser.NestedNameSpecifierContext context)
        {
            var nestedVisitor = new NestedNameSpecifierVisitor();
            Analyzer.Analyze(context, nestedVisitor);
            FunctionInfo.NestedNames = nestedVisitor.NestedNames.ToList();
            return false;
        }
        private void ParseDestructor([NotNull] CPP14Parser.UnqualifiedIdContext context)
        {
            if (FunctionInfo.ReturnType is not null)
            {
                if (FunctionInfo.ReturnType.NestedNames is not null)
                    FunctionInfo.NestedNames.AddRange(FunctionInfo.ReturnType.NestedNames.ToList());
                FunctionInfo.NestedNames.Add(FunctionInfo.ReturnType);
            }
            FunctionInfo.ReturnType = null;
            // ClassName or decltypeSpecifier
            if (context.children[1] is CPP14Parser.ClassNameContext)
            {
                var identifier = context.children.SingleOrDefault(x => x is CPP14Parser.ClassNameContext)?.
                    GetTerminalNodes().SingleOrDefault();
                if (identifier is not null)
                {
                    FunctionInfo.Name = "~" + identifier.GetText();
                }
            }
            else if (context.children[1] is CPP14Parser.DecltypeSpecifierContext)
            {
                // TODO: DecltypeSpecifierContext
            }
        }
        private void ParseOperatorFunctionId([NotNull] CPP14Parser.OperatorFunctionIdContext context)
        {
            FunctionInfo.Name = context.children.First().GetText() // operator
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
                    FunctionInfo.Name = functionName;
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
            FunctionInfo.Parameters.Add(visitor.Parameter);
            return false;
        }

        public override bool VisitFunctionBody([NotNull] CPP14Parser.FunctionBodyContext context)
        {
            /*functionBody:
            constructorInitializer? compoundStatement
	        | functionTryBlock
	        | Assign (Default | Delete) Semi;
             */
            FunctionInfo.FunctionBody = context;
            return false;
        }

        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
