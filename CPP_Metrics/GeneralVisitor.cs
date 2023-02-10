﻿using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.CyclomaticComplexity;
using CPP_Metrics.OOP;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;

namespace CPP_Metrics
{
   
    public class GeneralVisitor : CPP14ParserBaseVisitor<bool>
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
        public override bool VisitSimpleDeclaration([NotNull] CPP14Parser.SimpleDeclarationContext context)
        {
            var visitor = new SimpleDeclarationContextVisitor();
            Analyzer.Analyze(context, visitor);

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
