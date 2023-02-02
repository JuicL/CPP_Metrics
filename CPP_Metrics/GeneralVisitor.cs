using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.CyclomaticComplexity;
using CPP_Metrics.OOP;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;

namespace CPP_Metrics
{
    public class GeneralVisitor : CPP14ParserBaseVisitor<bool>
    {
        private List<Variable> variable = new List<Variable>();
        public override bool VisitChildren(IRuleNode node)
        {
            return true;    
        }
        public override bool VisitClassSpecifier([NotNull] CPP14Parser.ClassSpecifierContext context)
        {
            var visitor = new ClassStructVisitor();
            Analyzer.Analyze(context, visitor);
            return true;
        }
        public override bool VisitSimpleDeclaration([NotNull] CPP14Parser.SimpleDeclarationContext context)
        {
            //var visitor = new SimpleDeclarationContextVisitor(variable);
            //Analyzer.Analyze(context, visitor);

            //variable.AddRange(visitor.VariableNames);
            //Console.WriteLine("===========");
            //Console.WriteLine("VariableNames");
            //foreach (var variable in visitor.VariableNames)
            //    Console.WriteLine($"----type:{variable.Type},name:{variable.Name}");
            //Console.WriteLine("CallFuncNames");
            //foreach (var name in visitor.CallFuncNames)
            //    Console.WriteLine(name);
            //Console.WriteLine("DeclFuncNames");
            //foreach (var name in visitor.DeclFuncNames)
            //    Console.WriteLine(name);

            return true;
        }
        //public override bool VisitFunctionBody([NotNull] CPP14Parser.FunctionBodyContext context)
        //{
        //    CyclomaticComplexityMetric cyclomatic = new CyclomaticComplexityMetric();

        //    cyclomatic.Analyze(context);

        //    return false;
        //}
    }
}
