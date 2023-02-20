

using Antlr4.Runtime.Misc;
using CPP_Metrics.Metrics;
using CPP_Metrics.OOP;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics.CyclomaticComplexity
{
    public class CyclomaticComplexityFunctionVisitor : CPP14ParserBaseVisitor<bool>
    {
        
        public List<CyclomaticComplexityInfo> Cyclomatic = new List<CyclomaticComplexityInfo>();
        public ClassStructInfo? ClassStructInfo { get; set; }
        public CyclomaticComplexityFunctionVisitor()
        {

        }
        public CyclomaticComplexityFunctionVisitor(ClassStructInfo classStructInfo)
        {
            ClassStructInfo = classStructInfo;
        }

        public override bool VisitClassSpecifier([NotNull] CPP14Parser.ClassSpecifierContext context)
        {
            var visitor = new ClassStructVisitor();
            Analyzer.Analyze(context, visitor);
            ClassStructInfo = visitor.ClassStructInfo;
            return false;
        }
        public override bool VisitFunctionDefinition([NotNull] CPP14Parser.FunctionDefinitionContext context)
        {
            var functionInfoVisitor = new FunctionDefinitionVisitor();
            Analyzer.Analyze(context, functionInfoVisitor);
            var functionInfo = functionInfoVisitor.FunctionInfo;
            if(ClassStructInfo is not null)
            {
                functionInfo.NestedNames.Add(new CPPType() { TypeName = ClassStructInfo.Name });
            }
            CyclomaticGraph graph = new CyclomaticGraph();

            CyclomaticComplexityVisitor visitor = new CyclomaticComplexityVisitor(graph, null, null);
            Analyzer.AnalyzeR(context, visitor);
            graph.CreateEdge(graph.Head, visitor.Last is null ? graph.Tail : visitor.Last);
            
            CyclomaticComplexityInfo cyclomaticComplexityInfo = new()
            { 
                FunctionInfo = functionInfo,
                CyclomaticGraph = graph,
                CyclomaticComplexityValue = graph.GetCyclomaticComplexity()
            };

            Cyclomatic.Add(cyclomaticComplexityInfo);

            return false;
        }

    }
}
