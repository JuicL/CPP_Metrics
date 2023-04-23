using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.CyclomaticComplexity;
using CPP_Metrics.OOP;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics
{
   
    public class TestVisitor : CPP14ParserBaseVisitor<bool>
    {
        public override bool VisitChildren(IRuleNode node)
        {
            return true;    
        }
        public override bool VisitClassSpecifier([NotNull] CPP14Parser.ClassSpecifierContext context)
        {
            //var visitor = new ClassStructVisitor();
            //Analyzer.Analyze(context, visitor);
            return true;
        }
        //public override bool VisitNestedNameSpecifier([NotNull] CPP14Parser.NestedNameSpecifierContext context)
        //{
        //    var visitor = new NestedNameSpecifierVisitor();
        //    Analyzer.Analyze(context, visitor);
        //    Console.WriteLine("====================");
        //    foreach(var item in visitor.NestedNames.Reverse())
        //    {
        //        Console.WriteLine(item.Name + " //" + item);
        //    }
        //    return false;
        //}
        public BaseContextElement ContextElement = BaseContextElement.GetGeneralNameSpace();

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
                    Console.WriteLine($"Переопределение имени {item.Name} {item.Type.TypeName}");
                    //throw new Exception($"Переопределение имени {item.Name}");
                }
                else
                    ContextElement.VariableDeclaration.Add(item.Name, item);
            }
            //Console.WriteLine("===========");
            //Console.WriteLine("VariableNames");
            //foreach (var variable in OutContext.outVariables)
            //    Console.WriteLine($"----type:{variable.Type?.TypeName},name:{variable.Name}");

            //Console.WriteLine("CallFuncNames");
            //foreach (var name in visitor.CallFuncNames)
            //    Console.WriteLine(name);
            //Console.WriteLine("DeclFuncNames");
            //foreach (var name in OutContext.DeclFuncNames)
            //    Console.WriteLine(name);

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
