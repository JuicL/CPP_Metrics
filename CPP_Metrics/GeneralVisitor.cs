using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.CyclomaticComplexity;


namespace CPP_Metrics
{
    public class GeneralVisitor : CPP14ParserBaseVisitor<bool>
    {
        private List<String> variable = new List<string>();
        public override bool VisitChildren(IRuleNode node)
        {
            return true;    
        }
        public override bool VisitSimpleDeclaration([NotNull] CPP14Parser.SimpleDeclarationContext context)
        {
            var visitor = new SimpleDeclarationContextVisitor(variable);
            Analyzer.Analyze(context, visitor);

            variable.AddRange(visitor.VariableNames);
            Console.WriteLine("===========");
            Console.WriteLine("VariableNames");
            foreach (var name in visitor.VariableNames)
                Console.WriteLine(name);
            Console.WriteLine("CallFuncNames");
            foreach (var name in visitor.CallFuncNames)
                Console.WriteLine(name);
            Console.WriteLine("DeclFuncNames");
            foreach (var name in visitor.DeclFuncNames)
                Console.WriteLine(name);

            return false;
        }
        //public override bool VisitFunctionBody([NotNull] CPP14Parser.FunctionBodyContext context)
        //{
        //    CyclomaticComplexityMetric cyclomatic = new CyclomaticComplexityMetric();

        //    cyclomatic.Analyze(context);

        //    return false;
        //}
    }
}
