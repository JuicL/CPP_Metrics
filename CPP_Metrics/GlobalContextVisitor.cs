

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.OOP;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics
{
    public static class ContextHelper
    {
        public static bool GetVariableName(this BaseContextElement contextElement, string? name)
        {
            // TODO If contextElement is functionDeclaration and this method of class, first check in class field, and after in gloabal context
            if (name == null) return false;
            for(var context = contextElement; context is not null; context = context.Paren)
            {
                if(context.VariableDeclaration.TryGetValue(name, out var variable))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool GetFunctionName(this BaseContextElement contextElement, string name)
        {
            if (name == null) return false;
            for (var context = contextElement; context is not null; context = context.Paren)
            {
                if (context.FunctionDeclaration.TryGetValue(name, out var functionInfo))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GetTypeName(this BaseContextElement contextElement, string name)
        {
            return true;
        }

    }

    public class GlobalContextVisitor : CPP14ParserBaseVisitor<bool>
    {
        public GlobalContextVisitor(BaseContextElement contextElement)
        {
            ContextElement = contextElement;
        }

        public BaseContextElement ContextElement { get; }
        //Field current context
        public BaseContextElement Current { get; }

        // Statement // AND for 

        //NameSpace
        public override bool VisitNamespaceDefinition([NotNull] CPP14Parser.NamespaceDefinitionContext context)
        {
            var namespaceInfo = context.GetNameSpaceInfo();
            var declarationseq = context.declarationseq();
            var namespaceContext = new NamespaceContext
            {
                NameSpaceInfo = namespaceInfo,
                Paren = ContextElement
            };

            ContextElement.Children.Add(namespaceContext);

            if(declarationseq is not null)
            {
                var globalContextVisitor = new GlobalContextVisitor(namespaceContext);
                Analyzer.Analyze(declarationseq, globalContextVisitor);
            }
            return true;
        }

        //Function or method
        public override bool VisitFunctionDefinition([NotNull] CPP14Parser.FunctionDefinitionContext context)
        {
            var funcVisitor = new FunctionDefinitionVisitor();
            Analyzer.Analyze(context, funcVisitor);

            var functionContext = new FunctionDeclaration();
            functionContext.FunctionInfo = funcVisitor.FunctionInfo;
            functionContext.Paren = ContextElement;
            ContextElement.Children.Add(functionContext);

            if (!ContextElement.FunctionDeclaration.TryGetValue(funcVisitor.FunctionInfo.Name,out var functionInfos))
            {
                ContextElement.FunctionDeclaration.Add(funcVisitor.FunctionInfo.Name, new List<FunctionInfo>());
            }
            ContextElement.FunctionDeclaration[funcVisitor.FunctionInfo.Name].Add(funcVisitor.FunctionInfo);

            var globalContextVisitor = new GlobalContextVisitor(functionContext);
            Analyzer.Analyze(funcVisitor.FunctionInfo.FunctionBody, globalContextVisitor);
            return false;
        }

        // Variable declaration, function declaration
        public override bool VisitSimpleDeclaration([NotNull] CPP14Parser.SimpleDeclarationContext context)
        {
            var visitor = new SimpleDeclarationContextVisitor(ContextElement);
            Analyzer.Analyze(context, visitor);

            foreach (var item in visitor.FunctionDeclaration)
            {
                if (!ContextElement.FunctionDeclaration.TryGetValue(item.Name, out var functionInfo))
                {
                    ContextElement.FunctionDeclaration.Add(item.Name, new List<FunctionInfo>());
                }
                ContextElement.FunctionDeclaration[item.Name].Add(item);
            }
            foreach (var item in visitor.VariablesDeclaration)
            {
                Variable value;
                if (ContextElement.VariableDeclaration.TryGetValue(item.Name, out value))
                {
                    throw new Exception("Переопределение имени");
                }
                ContextElement.VariableDeclaration.Add(item.Name, item);
            }

            return true;
        }

        //Class
        public override bool VisitClassSpecifier([NotNull] CPP14Parser.ClassSpecifierContext context)
        {
            var visitor = new ClassStructVisitor();
            Analyzer.Analyze(context, visitor);
            var classContext = new ClassStructDeclaration();
            classContext.ClassStructInfo = visitor.ClassStructInfo;
            classContext.Paren = ContextElement;
            ContextElement.Children.Add(classContext);

            if (!ContextElement.TypeDeclaration.TryGetValue(visitor.ClassStructInfo.Name, out var classStructInfo))
            {
                ContextElement.TypeDeclaration.Add(visitor.ClassStructInfo.Name, visitor.ClassStructInfo);
            }
            else
            {
                throw new Exception("Переопределение класса");
            }

            if(visitor.ClassStructInfo.Body is not null)
            {
                var globalContextVisitor = new GlobalContextVisitor(classContext);
                Analyzer.Analyze(visitor.ClassStructInfo.Body, globalContextVisitor);
            }

            return false;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }

    }
}
